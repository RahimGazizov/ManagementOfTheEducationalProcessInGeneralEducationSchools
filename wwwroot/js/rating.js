document.addEventListener("DOMContentLoaded", function () {
    const classId = document.getElementById("classIdForRating");
    if (!classId) { console.log("Элемент класса не найден"); return; }

    classId.addEventListener("change", async function () {
        const classData = this.value;

        const res = await fetch(`/StudentPerAcc/GetClassData?classId=${classData}`);
        if (!res.ok) {
            console.log("Ошибка данные не загружены");
            return;
        }

        const data = await res.json();
        console.log("DATA", data);
        if (data.academicYear) {
            document.getElementById("academicForRating").value = data.academicYear.name;
            document.getElementById("academicIdForRating").value = data.academicYear.id;
        }

        const termsList = document.getElementById("termIdForRating");

        termsList.innerHTML = "";

        data.terms.forEach(t => {
            const option = document.createElement("option");
            option.value = t.id;
            option.text = t.name;
            termsList.appendChild(option);
        });
        termsList.disabled = false;
    });
})