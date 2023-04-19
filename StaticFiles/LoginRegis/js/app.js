const sign_in_btn = document.querySelector("#sign-in-btn");
const sign_up_btn = document.querySelector("#sign-up-btn");
const container = document.querySelector(".container");
const form = document.querySelector("#my-form");

sign_up_btn.addEventListener("click", () => {
  container.classList.add("sign-up-mode");
});

sign_in_btn.addEventListener("click", () => {
  container.classList.remove("sign-up-mode");
});

function validatePassword(event) {
    var password = document.querySelector("#password");
    var confirmPassword = document.querySelector("#confirm-password");
    var username = document.querySelector('input[name="Username"]');
    var email = document.querySelector('input[name="Email"]');

    if (username.value === '' || email.value === '' || password.value === '' || confirmPassword.value === '') {
        alert("Please fill in all the fields!");
        event.preventDefault();
        return false;
    }
    if (password.value != confirmPassword.value) {
        alert("Passwords do not match!");
        event.preventDefault();
        return false;
    }
    return true;
}

