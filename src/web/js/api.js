function getBusinesses() {
  return $.getJSON(API_URL + "/api/businesses");
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