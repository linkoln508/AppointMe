let allBusinesses = [];

function drawBusinesses(list) {
  if (list.length === 0) {
    $("#content").text("У цій категорії поки немає бізнесів.");
    return;
  }
  $("#content").html("<div id='businesses'></div>");
      list.forEach(function (b) {
           $("#businesses").append(`
          <div class='card'>
            <h3><a href='pages/business.html?id=${b.id}'>${escapeHtml(b.name)}</a></h3>
            <p>${escapeHtml(b.categoryName)} · ${escapeHtml(b.city)}</p>
          </div>
        `);
      });
}

$(function () {
  getCategories()
    .done(function (categories) {
      let buttons = `<button class="button" data-category="all">Усі</button>`;
      categories.forEach(function (c) {
        buttons += `<button class="button" data-category="${c.id}">${escapeHtml(c.name)}</button>`;
      });
      $("#categories").html(buttons);
    });

  getBusinesses()
    .done(function (businesses) {
      allBusinesses = businesses;
      drawBusinesses(allBusinesses);
    })
    .fail(function () {
      $("#content").text("Не вдалося завантажити дані");
    });
});

$("#categories").on("click", "[data-category]", function () {
  const categoryId = $(this).attr("data-category");
  if (categoryId === "all"){
    drawBusinesses(allBusinesses);
    return;
  }
  const filtered = allBusinesses.filter(function (b) {
    return b.categoryId === Number(categoryId);
  });
  drawBusinesses(filtered);
});