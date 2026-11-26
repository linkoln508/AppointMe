function getBusinesses() {
  return $.getJSON(API_URL + "/api/businesses");
}

function getCategories() {
  return $.getJSON(API_URL + "/api/categories");
}

function getBusinessById(id) {
  return $.getJSON(API_URL + "/api/businesses/" + id);
}

function registerUser(data) {
  return $.ajax({
    url: API_URL + "/api/auth/register",
    method: "POST",
    contentType: "application/json",
    data: JSON.stringify(data)
  });
}

function loginUser(data) {
  return $.ajax({
    url: API_URL + "/api/auth/login",
    method: "POST",
    contentType: "application/json",
    data: JSON.stringify(data)
  });
}

function getAdminUsers() {
  return $.getJSON(API_URL + "/api/admin/users");
}

function blockUser(id) {
  return $.ajax({ url: API_URL + "/api/admin/users/" + id + "/block", method: "POST" });
}

function unblockUser(id) {
  return $.ajax({ url: API_URL + "/api/admin/users/" + id + "/unblock", method: "POST" });
}

function getAdminBusinesses() {
  return $.getJSON(API_URL + "/api/admin/businesses");
}

function deactivateBusiness(id) {
  return $.ajax({ url: API_URL + "/api/admin/businesses/" + id + "/deactivate", method: "POST" });
}

function activateBusiness(id) {
  return $.ajax({ url: API_URL + "/api/admin/businesses/" + id + "/activate", method: "POST" });
}