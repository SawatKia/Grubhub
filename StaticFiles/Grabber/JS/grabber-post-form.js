const closeOption = document.querySelector('#CloseOption');
const closeTimeInput = document.querySelector('#CloseTime');
const form = document.querySelector('form');

closeOption.addEventListener('change', (event) => {
    if (event.target.checked) {
        closeTimeInput.disabled = true;
        closeTimeInput.value = 'default';
    } else {
        closeTimeInput.disabled = false;
    }
});

var postForm = document.getElementById("post-form");
var postButton = document.getElementById("post-button");
var isFormVisible = false;
postButton.addEventListener("click", function () {
    if (isFormVisible) {
        postForm.style.display = "none";
        isFormVisible = false;
    } else {
        postForm.style.display = "block";
        isFormVisible = true;
    }
});
