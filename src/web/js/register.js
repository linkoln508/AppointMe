$("#register-form").on("submit", function (event) {
  event.preventDefault(); // не давать браузеру перезагрузить страницу
  $("#form-error").text("");

  const password = $("#password").val();

  if (password !== $("#confirmPassword").val()) {
    $("#form-error").text("Паролі не збігаються.");
    return;
  }

  registerUser({
    fullName: $("#fullName").val(),
    email: $("#email").val(),
    password: password,
    accountType: $("#accountType").val()
  })
    .done(function () {
      window.location.href = "login.html";
    })
    .fail(function (xhr) {
      if (xhr.status === 409) {
        $("#form-error").text("Цей email вже зареєстровано.");
      } else if (xhr.status === 400) {
        $("#form-error").text("Перевірте пароль: мінімум 8 символів + хоча б одна цифра.");
      } else {
        $("#form-error").text("Не вдалося зареєструватися.");
      }
    });
});