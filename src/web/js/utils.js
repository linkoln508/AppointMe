// Теги в буквы
function escapeHtml(text) {
  return $("<div>").text(text ?? "").html();
}