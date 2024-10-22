function calcHeight(value) {
  let numberOfLineBreaks = (value.match(/\n/g) || []).length;
  // min-height + lines x line-height + padding
  let newHeight = 20 + numberOfLineBreaks * 30 + 10;
  return newHeight;
}

let textarea = document.querySelector(".comment-input");
textarea.addEventListener("keyup", () => {
  textarea.style.height = calcHeight(textarea.value) + "px";
});