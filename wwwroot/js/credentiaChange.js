document.addEventListener("DOMContentLoaded", function () {

    const userLogin = document.getElementById("loginUser");
    const currentPassword = document.getElementById("currentPasswordUser");
    const newPassword = document.getElementById("newPasswordUser");
    const btn = document.getElementById("btnChange");
    const error = document.getElementById("errorMessage");
    const success = document.getElementById("successMessage");

    if (!userLogin || !currentPassword || !newPassword || !btn || !error || !success) {
        console.log("Элементы не найдены");
        return;
    }
    function delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    btn.addEventListener("click", async function () {

        const model = {
            userName: userLogin.value,
            currentPassword: currentPassword.value,
            newPassword: newPassword.value
        };

        const res = await fetch("/UpdateCredential/CredentialsChange", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(model)
        });

        const data = await res.json();

        if (!res.ok) {
            error.innerText = data.message;
            setTimeout(function () {
                error.innerText = "";
            }, 5000);
            return;
        }

        success.innerText = data.message;

        await delay(1500);
        window.location.href = "/Authoriz/Index";
     
    });
});