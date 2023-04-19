function toggleForm(postId) {
    var form = document.getElementById("order-form-" + postId);
    if (form.style.display === "none") {
        form.style.display = "block";
    } else {
        form.style.display = "none";
    }
}
function validateForm(maxQuantity, maxPrice, postId) {
    var numBoxes = parseInt(document.getElementById("numBoxes-" + postId).value);
    var totalPrice = parseInt(document.getElementById("totalPrice-" + postId).value);
    var warning = "";

    if (numBoxes > maxQuantity) {
        warning += "Number of boxes cannot exceed " + maxQuantity + ". ";
    }
    if (warning !== "") {
        warning += " and "
    }
    if (totalPrice > maxPrice) {
        warning += "Estimated total price cannot exceed " + maxPrice + ". ";
    }
    if (warning !== "") {
        warning += ", please try again"
        alert(warning);
    }
}
