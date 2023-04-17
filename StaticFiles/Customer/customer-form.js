function toggleForm(postId) {
    var form = document.getElementById("order-form-" + postId);
    if (form.style.display === "none") {
        form.style.display = "block";
    } else {
        form.style.display = "none";
    }
}