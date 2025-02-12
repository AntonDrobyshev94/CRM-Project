document.getElementById("uploadForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var form = event.target;
    var formData = new FormData(form);
    var redirectUrl = form.getAttribute("data-redirect-url");

    fetch(form.action, {
        method: "POST",
        body: formData,
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        }
    })
        .then(response => response.json())
        .then(data => {
            if (!data.success) {
                if (data.redirectUrl != null) {
                    alert(data.message);
                    window.location.href = data.redirectUrl;
                }
                else {
                    alert(data.message);
                }
            }
            else {
                window.location.href = redirectUrl;
            }
        })
        .catch(error => console.error("Ошибка:", error));
});