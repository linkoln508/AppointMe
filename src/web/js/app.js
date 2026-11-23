$(function () {
  getBusinesses()
    .done(function (businesses) {
      $("#content").html("<div id='businesses'></div>");
      businesses.forEach(function (b) {
        $("#businesses").append(`
          <div class='card'>
            <h3><a href='pages/business.html?id=${b.id}'>${b.name}</a></h3>
            <p>${b.categoryName} · ${b.city}</p>
          </div>
        `);
      });
    })
    .fail(function () {
      $("#content").text("Не вдалося завантажити дані");
    });
});