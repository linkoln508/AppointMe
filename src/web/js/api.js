function getBusinesses() {
  return $.getJSON(API_URL + "/api/businesses");
}

function getBusinessById(id) {
  return $.getJSON(API_URL + "/api/businesses/" + id);
}