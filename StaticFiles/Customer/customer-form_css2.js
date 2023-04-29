function toggleForm(postId) {
    var form = document.getElementById("order-form-" + postId);
    if (form.style.display === 'none') {
        form.style.display = 'flex';
    } else {
        form.style.display = 'none';
    }

}

//form.querySelector('.close').addEventListener('click',
//        function () {
//            form.querySelector('.bg-model').style.display = 'none';
//        });

var bgModels = document.querySelectorAll('.bg-model');
bgModels.forEach(function (bgModel) {
    bgModel.addEventListener('click', function (event) {
        // ตรวจสอบว่าคลิกที่ .close หรือไม่
        if (event.target.classList.contains('close')) {
            bgModel.style.display = 'none';
        }
    });
});

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
