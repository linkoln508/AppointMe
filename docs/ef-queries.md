# EF Core queries review (W02-T01)

SQL взят из логов (`Microsoft.EntityFrameworkCore.Database.Command: Information`).

## 1. GET /api/categories

```sql
 SELECT c.id, c.name, c.slug
      FROM categories AS c
      ORDER BY c.name
```
- `OrderBy` → `ORDER BY`: сортирует база.
- `AsNoTracking()` в SQL не виден — влияет только на change tracker.

## 2. GET /api/categories/{id}

```sql
 SELECT c.id, c.name, c.slug
      FROM categories AS c
      WHERE c.id = @id
      LIMIT 1
```
- `FirstOrDefaultAsync` → `LIMIT 1`.
- `@id` — параметр, защита от SQL-инъекции.

## 3. GET /api/businesses

```sql
SELECT b.id AS "Id", b.category_id AS "CategoryId", c.name AS "CategoryName", b.name AS "Name", b.description AS "Description", b.city AS "City", b.address AS "Address", b.phone AS "Phone", b.is_active AS "IsActive", b.created_at AS "CreatedAt"
      FROM businesses AS b
      INNER JOIN categories AS c ON b.category_id = c.id
      WHERE b.is_active
      ORDER BY b.name
```
- Проекция в `new Business { ... }`: только нужные колонки, `CategoryName` через JOIN в одном запросе.
- `Where`/`OrderBy` до `ToListAsync` → фильтр и сортировка в базе.

## 4. GET /api/businesses/{id}

> Было 2 запроса — исправлено, см. раздел «Деталка бизнеса: Include или Select».

Два запроса:

```sql
SELECT b.id AS "Id", b.category_id AS "CategoryId", c.name AS "CategoryName", b.name AS "Name", b.description AS "Description", b.city AS "City", b.address AS "Address", b.phone AS "Phone", b.is_active AS "IsActive", b.created_at AS "CreatedAt"
      FROM businesses AS b
      INNER JOIN categories AS c ON b.category_id = c.id
      WHERE b.id = @id
      LIMIT 1
```

```sql
SELECT s.id, s.business_id, s.description, s.duration_minutes, s.is_active, s.name, s.price
      FROM services AS s
      WHERE s.business_id = @id AND s.is_active
      ORDER BY s.name
```
- Неактивный бизнес по прямому id отдаётся (нет фильтра `is_active`).

## 5. GET /api/businesses/{businessId}/services

Два запроса: проверка бизнеса(как в предыдущем) + услуги.

```sql
SELECT s.id, s.business_id, s.description, s.duration_minutes, s.is_active, s.name, s.price
      FROM services AS s
      WHERE s.business_id = @businessId
      ORDER BY s.name
```
- Нет фильтра `is_active` — отдаются и неактивные услуги.

## 6. GET /api/businesses/{businessId}/services/{serviceId}

```sql
SELECT s.id, s.business_id, s.description, s.duration_minutes, s.is_active, s.name, s.price
      FROM services AS s
      WHERE s.id = @serviceId AND s.business_id = @businessId
      LIMIT 1
```

## N+1: пример и исправление

Временная ветка `experiment/W02-T01-n-plus-one` (не мержится).
В `GetAllAsync` услуги грузились в `foreach` для каждого бизнеса:

```csharp
foreach (var business in businesses)
{
    business.Services = await _db.Services
        .AsNoTracking()
        .Where(s => s.BusinessId == business.Id)
        .ToListAsync();   // запрос в базу на каждом круге цикла
}
```
Результат при 4 бизнесах: 1 запрос за бизнесами + 4 одинаковых запроса за услугами = 5 запросов.

```
      Executed DbCommand (2ms) [Parameters=[@business_Id='?' (DbType = Guid)], CommandType='Text', CommandTimeout='30']
      SELECT s.id, s.business_id, s.description, s.duration_minutes, s.is_active, s.name, s.price
      FROM services AS s
      WHERE s.business_id = @business_Id

      Executed DbCommand (1ms) [Parameters=[@business_Id='?' (DbType = Guid)], CommandType='Text', CommandTimeout='30']
      SELECT s.id, s.business_id, s.description, s.duration_minutes, s.is_active, s.name, s.price
      FROM services AS s
      WHERE s.business_id = @business_Id

... (+ 2 таких же блока)
```

**Почему плохо:** число запросов растёт вместе с данными (1000 бизнесов = 1001 запрос).
**Исправление:** цикл убран — список бизнесов услуги не загружает (1 запрос).
Если связанные данные нужны — грузить их одним запросом (`Include` или `Select`), а не в цикле.

## AsNoTracking vs Select

- **`AsNoTracking()`** — загружает целые сущности (все колонки), но не кладёт их в change tracker.
  Используется в: `GetAll`/`GetById` категорий и услуг.
  
- **`Select` (проекция)** — загружает только перечисленные колонки в объект, собранный вручную;
  change tracker не участвует. Можно взять поле из другой таблицы (JOIN).
  Используется в: список бизнесов (`CategoryName` из `categories`).
- Трекинг нужен только когда объект будем менять и вызывать `SaveChangesAsync()`
  (поэтому `UpdateAsync`/`DeleteAsync` грузят через `FindAsync` — с трекингом).
- В `BusinessesWithCategoryName()` вызов `AsNoTracking()` ничего не меняет — проекция и так не отслеживается.

## Деталка бизнеса: Include или Select

Выбран **`Select`** (проекция), один запрос вместо двух.

- `Include` не подходит: `CategoryName` не колонка (`Ignore` в конфигурации),
  а навигации `Business.Category` нет — через Include имя категории не получить.
- `Select` берёт только нужные колонки, `CategoryName` через JOIN, услуги через `b.Services`.
- `AsNoTracking()` здесь нужен: услуги внутри проекции — настоящие сущности `Service`.

```sql
SELECT s0.id, s0.category_id, s0.name, s0.name0, s0.description, s0.city, s0.address, s0.phone, s0.is_active, s0.created_at, s0.id0, s1.id, s1.business_id, s1.description, s1.duration_minutes, s1.is_active, s1.name, s1.price
FROM (
    SELECT b.id, b.category_id, c.name, b.name AS name0, b.description, b.city, b.address, b.phone, b.is_active, b.created_at, c.id AS id0
    FROM businesses AS b
    INNER JOIN categories AS c ON b.category_id = c.id
    WHERE b.id = @id
    LIMIT 1
) AS s0
LEFT JOIN (
    SELECT s.id, s.business_id, s.description, s.duration_minutes, s.is_active, s.name, s.price
    FROM services AS s
    WHERE s.is_active
) AS s1 ON s0.id = s1.business_id
ORDER BY s0.id, s0.id0, s1.name
```

- `LEFT JOIN` — бизнес без услуг тоже вернётся.
- Данные бизнеса повторяются в каждой строке услуги; для многих коллекций — `AsSplitQuery()`.
