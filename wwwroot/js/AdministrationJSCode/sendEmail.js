document.addEventListener("DOMContentLoaded", function () {

    const buttons = document.querySelectorAll(".notifyParents");
    const error = document.getElementById("errorMessageEmail");
    const success = document.getElementById("successMessageEmail");

    buttons.forEach(btn => {
        btn.addEventListener("click", async function () {

            const params = new URLSearchParams();

            params.append("studentId", btn.dataset.student);
            params.append("academicId", btn.dataset.academic);
            params.append("termId", btn.dataset.term);
            params.append("classId", btn.dataset.class);
            params.append("subjectId", btn.dataset.subject);

            const res = await fetch("/AdministrationSchoolPerAcc/SendEmail", {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                },
                body: params
            });

            const result = await res.json();
            console.log("SERVER RESULT:", result);

            if (res.ok) {
                success.innerText = result.results.message ?? "Успешно отправлено";
            } else {
                error.innerText = result.results.message ?? "Ошибка";
            }
        });
    });

});