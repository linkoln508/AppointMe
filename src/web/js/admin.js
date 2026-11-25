// Показать ошибку над таблицами
function showAdminError(text) {
  $("#admin-error").text(text || "Не вдалося виконати дію.");
}

function loadUsers() {
  getAdminUsers()
    .done(function (users) {
      let rows = "";
      users.forEach(function (u) {
        const status = u.isBlocked ? "Заблоковано" : "Активний";
        const button = u.isBlocked
          ? `<button class="button" data-unblock="${u.id}">Розблокувати</button>`
          : `<button class="button" data-block="${u.id}">Заблокувати</button>`;

        rows += `
          <tr>
            <td>${escapeHtml(u.email)}</td>
            <td>${escapeHtml(u.fullName)}</td>
            <td>${escapeHtml(u.roles.join(", "))}</td>
            <td>${status}</td>
            <td>${button}</td>
          </tr>`;
      });
      $("#users-body").html(rows);
    })
    .fail(function () {
      showAdminError("Не вдалося завантажити користувачів.");
    });
}

$("#users-body").on("click", "[data-block]", function () {
  const id = $(this).attr("data-block");
  if (!confirm("Заблокувати цього користувача?")) {
    return;
  }
  blockUser(id)
    .done(loadUsers)
    .fail(function (xhr) {
      if (xhr.status === 400) {
        showAdminError("Не можна заблокувати самого себе.");
      } else {
        showAdminError();
      }
    });
});

$("#users-body").on("click", "[data-unblock]", function () {
  const id = $(this).attr("data-unblock");
  if (!confirm("Розблокувати цього користувача?")) {
    return;
  }
  unblockUser(id).done(loadUsers).fail(function () { showAdminError(); });
});


function loadBusinesses() {
  getAdminBusinesses()
    .done(function (businesses) {
      let rows = "";
      businesses.forEach(function (b) {
        const status = b.isActive ? "Активний" : "Вимкнено";
        const button = b.isActive
          ? `<button class="button" data-deactivate="${b.id}">Вимкнути</button>`
          : `<button class="button" data-activate="${b.id}">Увімкнути</button>`;

        rows += `
          <tr>
            <td>${escapeHtml(b.name)}</td>
            <td>${escapeHtml(b.categoryName)}</td>
            <td>${escapeHtml(b.city)}</td>
            <td>${escapeHtml(b.ownerEmail || "—")}</td>
            <td>${status}</td>
            <td>${button}</td>
          </tr>`;
      });
      $("#businesses-body").html(rows);
    })
    .fail(function () {
      showAdminError("Не вдалося завантажити бізнеси.");
    });
}

$("#businesses-body").on("click", "[data-deactivate]", function () {
  const id = $(this).attr("data-deactivate");
  if (!confirm("Вимкнути цей бізнес? Він зникне з головної сторінки.")) {
    return;
  }
  deactivateBusiness(id).done(loadBusinesses).fail(function () { showAdminError(); });
});

$("#businesses-body").on("click", "[data-activate]", function () {
  const id = $(this).attr("data-activate");
  if (!confirm("Увімкнути цей бізнес?")) {
    return;
  }
  activateBusiness(id).done(loadBusinesses).fail(function () { showAdminError(); });
});


$(function () {
  loadUsers();
  loadBusinesses();
});