$("#login-form").on("submit", function (event) {
  event.preventDefault();
  $("#form-error").text("");

  loginUser({
    email: $("#email").val(),
    password: $("#password").val()
  })
    .done(function (response) {
      localStorage.setItem("token", response.accessToken);
      localStorage.setItem("expiresAt", response.expiresAt);
      localStorage.setItem("user", JSON.stringify(response.user));
      window.location.href = "../index.html";
    })
    .fail(function (xhr) {
      if (xhr.status === 401) {
        $("#form-error").text("Невірний email або пароль.");
      } else if (xhr.status === 403) {
        $("#form-error").text("Користувача заблоковано.");
      } else {
        $("#form-error").text("Не вдалося увійти.");
      }
    });
});