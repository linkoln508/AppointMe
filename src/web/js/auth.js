// Достать токен из хранилища (или null, если его нет)
function getToken() {
  return localStorage.getItem("token");
}

// Достать данные пользователя как объект
function getUser() {
  const text = localStorage.getItem("user");
  return text ? JSON.parse(text) : null;
}

function isLoggedIn() {
  const token = getToken();
  const expiresAt = localStorage.getItem("expiresAt");
  if (!token || !expiresAt) {
    return false;
  }
  return new Date(expiresAt) > new Date();
}

// Стереть всё, что сохранили при входе
function logout() {
  localStorage.removeItem("token");
  localStorage.removeItem("expiresAt");
  localStorage.removeItem("user");
}

// приложить токен перед запросом (если вошли)
$.ajaxSetup({
  beforeSend: function (xhr) {
    if (isLoggedIn())
    {
      xhr.setRequestHeader("Authorization", "Bearer " + getToken());
    }
  }});

$(document).ajaxError(function (event, xhr) {
  if (xhr.status === 401 && getToken())
    {
    logout();
    window.location.href = "/pages/login.html";
    }
});

// Меню в шапке. В зависимости от того, кто вошёл
function renderNav() {
  const nav = $("header nav");

  if (!isLoggedIn()) {
    nav.html(`
      <a href="/index.html">Головна</a>
      <a href="/pages/login.html">Вхід</a>
      <a href="/pages/register.html">Реєстрація</a>
    `);
    return;
  }

  const user = getUser();
  let links = `<a href="/index.html">Головна</a>`;

  if (user.roles.includes("Customer") || user.roles.includes("BusinessOwner")) {
    links += `<a href="/pages/my-appointments.html">Мої записи</a>`;
  }
  if (user.roles.includes("BusinessOwner")) {
    links += `<a href="/pages/owner.html">Кабінет власника</a>`;
  }
  if (user.roles.includes("Admin")) {
    links += `<a href="/pages/admin.html">Адмін-панель</a>`;
  }

  links += `<span class="user-name"></span> <a href="#" id="logout-link">Вийти</a>`;
  nav.html(links);

  $(".user-name").text(user.fullName);

  $("#logout-link").on("click", function (event) {
    event.preventDefault();
    logout();
    window.location.href = "/index.html";
  });
}

$(renderNav);

function requireRole(allowedRoles) {
  if (!isLoggedIn()) {
    window.location.href = "/pages/login.html?returnUrl=" + encodeURIComponent(window.location.pathname);
    return;
  }

  const user = getUser();
  const allowed = allowedRoles.some(function (role) {
    return user.roles.includes(role);
  });

  if (!allowed) {
    window.location.href = "/pages/forbidden.html";
  }
}