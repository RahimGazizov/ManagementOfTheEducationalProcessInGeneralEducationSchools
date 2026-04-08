document.addEventListener("DOMContentLoaded", function () {

    // ✅ глобально храним выбранного родителя
    let selectedParentId = null;

    async function GetListLetter() {
        const numClass = document.getElementById("listNumClass").value;
        if (!numClass) {
            console.log("Номер класса пуст");
            return;
        }

        const res = await fetch(`/Parents/GetLetterClass?numClass=${numClass}`);
        if (!res.ok) {
            console.log("Буквы не загрузились");
            return;
        }

        const data = await res.json();

        const letterSelect = document.getElementById("listLetterClass");
        letterSelect.innerHTML = "";

        data.forEach(l => {
            const option = document.createElement("option");
            option.value = l.id;
            option.text = l.letter;
            letterSelect.appendChild(option);
        });

        letterSelect.disabled = false;

        if (letterSelect.options.length > 0) {
            letterSelect.selectedIndex = 0;
            GetNameStudents();
        }
    }

    async function GetNameStudents() {
        const classId = document.getElementById("listLetterClass").value;

        if (!classId) {
            console.log("Айди класса пуст");
            return;
        }

        const responce = await fetch(`/Parents/GetNameStudents?classId=${classId}`);
        if (!responce.ok) {
            console.log("Ошибка загрузки студентов");
            return;
        }

        const data = await responce.json();

        const studentSelect = document.getElementById("listStudentsClass");
        studentSelect.innerHTML = "";

        data.forEach(stu => {
            const option = document.createElement("option");
            option.value = stu.id;
            option.text = stu.name;
            studentSelect.appendChild(option);
        });

        studentSelect.disabled = false;
    }

    // ✅ 1. Клик по "Добавить ребенка" → сохраняем parentId + открываем модалку
    document.querySelectorAll(".btnAdChild").forEach(btn => {
        btn.addEventListener("click", function () {
            selectedParentId = this.dataset.parentId;


            // открыть модалку
            document.getElementById("modalAddChild").classList.add("show");
        });
    });

    // ✅ 2. Кнопка внутри модалки → отправка
    document.getElementById("btnSendData").addEventListener("click", async function () {

        const studentId = document.getElementById("listStudentsClass").value;

        if (!selectedParentId || !studentId) {
            console.log("Айди родителя или студента равны null");
            console.log("Айди родителя", selectedParentId);
            return;
        }

        const res = await fetch(`/Parents/AddChild?parentId=${selectedParentId}&studentId=${studentId}`);

        if (!res.ok) {
            console.log("Ошибка сохранения ученика родителю");
            return;
        }

        // ✅ успех → редирект
        window.location.href = "/Parents/Index";
    });

    document.getElementById("listNumClass").addEventListener("change", GetListLetter);
    document.getElementById("listLetterClass").addEventListener("change", GetNameStudents);

});