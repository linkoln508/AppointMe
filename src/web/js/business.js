const id = new URLSearchParams(window.location.search).get("id");

$("#content").text("Загрузка…");

getBusinessById(id)
  .done(function (business) {
    // Список услуг в одну строку HTML
    let servicesHtml = "";
    business.services.forEach(function (s) {
      servicesHtml += `
        <div class='service'>
          <div>
            <strong>${s.name}</strong>
            <p>${s.price.toFixed(2)} PLN · ${s.durationMinutes} мин</p>
          </div>
          <a class='button' href='book.html?businessId=${business.id}&serviceId=${s.id}'>Забронировать</a>
        </div>`;
    });

    if (servicesHtml === "") {
      servicesHtml = "<p>Пока нет услуг.</p>";
    }

    $("#content").html(`
      <div class='card'>
        <h2>${business.name}</h2>
        <p>${business.categoryName} · ${business.city}</p>
        <p>${business.address}</p>
        <p>Телефон: ${business.phone || "не указан"}</p>
        <p>${business.description || ""}</p>
      </div>
      <h3>Услуги</h3>
      ${servicesHtml}
    `);
  })
  .fail(function (xhr) {
    if (xhr.status === 404) {
      $("#content").text("Бизнес не найден.");
    } else {
      $("#content").text("Не удалось загрузить данные.");
    }
  });