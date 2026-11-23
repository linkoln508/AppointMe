$(function () { 
  $.getJSON(API_URL + "/api/categories")
    .done(function (categories) {
      $("#content").html("<ul id='categories'></ul>");
      categories.forEach(function (c) {
        $("#categories").append("<li>" + c.name + "</li>");
      });
    })

    .fail(function () {
      $("#content").text("Не удалось загрузить данные");
    });

});