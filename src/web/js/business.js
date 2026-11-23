const id = new URLSearchParams(window.location.search).get("id");

$("#content").text("Завантаження…");

getBusinessById(id)
  .done(function (business) {
    // Список услуг в одну строку HTML
    let servicesHtml = "";
    business.services.forEach(function (s) {
      servicesHtml += `
        <div class='service'>
          <div>
            <strong>${s.name}</strong>
            <p>${s.price.toFixed(2)} грн · ${s.durationMinutes} хв</p>
          </div>
          <a class='button' href='book.html?businessId=${business.id}&serviceId=${s.id}'>Забронювати</a>
        </div>`;
    });

    if (servicesHtml === "") {
      servicesHtml = "<p>Поки що немає послуг.</p>";
    }

    $("#content").html(`
      <div class='card'>
        <h2>${business.name}</h2>
        <p>${business.categoryName} · ${business.city}</p>
        <p>${business.address}</p>
        <p>Телефон: ${business.phone || "не вказано"}</p>
        <p>${business.description || ""}</p>
      </div>
      <h3>Послуги</h3>
      ${servicesHtml}
    `);
  })
  .fail(function (xhr) {
    if (xhr.status === 404) {
      $("#content").text("Бізнес не знайдено.");
    } else {
      $("#content").text("Не вдалося завантажити дані.");
    }
  });