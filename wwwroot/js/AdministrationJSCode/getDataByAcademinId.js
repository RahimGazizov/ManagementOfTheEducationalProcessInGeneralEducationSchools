document.addEventListener("DOMContentLoaded", function () {

    const selectAcademic = document.querySelector(".academicListForAnalitical");
    const selectTerm = document.querySelector(".termListForAnalitical");
    const selectClass = document.querySelector(".classListForAnalitical");
    const selectSubject = document.querySelector(".subjectListForAnalitical");
    const selectStudents = document.querySelector(".studentsForAnalitical");
    const error = document.getElementById("errorMessage");

    if (!selectAcademic || !selectTerm || !selectClass || !selectSubject || !selectStudents || !error) {
        console.log("Один из селекторов не загрузился");
        return;
    }

    let academicId = selectAcademic.value;

    // =============================
    // 🔥 ОТЛИЧНАЯ ФУНКЦИЯ ОШИБОК
    // =============================
    function showError(message) {
        error.innerText = message;
        error.style.display = "block";

        clearTimeout(error._timer);

        error._timer = setTimeout(() => {
            error.style.display = "none";
            error.innerText = "";
        }, 5000);
    }

    // =============================
    // 🔥 placeholder
    // =============================
    function GetOptionDefaultText(text) {
        const placeholder = document.createElement("option");
        placeholder.textContent = text;
        placeholder.value = "";
        placeholder.disabled = true;
        placeholder.selected = true;
        return placeholder;
    }

    // =============================
    // 🔥 загрузка четвертей/классов/предметов
    // =============================
    async function loadTerms(academicId) {

        if (!academicId) {
            showError("Айди учебного года пуст");
            return;
        }

        const res = await fetch(`/AdministrationSchoolPerAcc/GetDataByAcademinYear?academicId=${academicId}`);

        let data;

        try {
            data = await res.json();
        } catch {
            showError("Ошибка сервера (ответ не JSON)");
            return;
        }

        if (!res.ok) {
            showError(data.message || "Ошибка загрузки данных");
            return;
        }

        // очищаем
        selectTerm.innerHTML = "";
        selectClass.innerHTML = "";
        selectSubject.innerHTML = "";

        // placeholders
        selectTerm.appendChild(GetOptionDefaultText("Выберите четверть"));
        selectClass.appendChild(GetOptionDefaultText("Выберите класс"));
        selectSubject.appendChild(GetOptionDefaultText("Выберите предмет"));

        // terms
        if (!data.terms || data.terms.length === 0) {
            showError("Список четвертей пуст");
        } else {
            data.terms.forEach(term => {
                const option = document.createElement("option");
                option.value = term.value;
                option.text = term.text;
                selectTerm.appendChild(option);
            });
        }

        // classes
        if (!data.classes || data.classes.length === 0) {
            showError("Список классов пуст");
        } else {
            data.classes.forEach(cls => {
                const option = document.createElement("option");
                option.value = cls.value;
                option.text = cls.text;
                selectClass.appendChild(option);
            });
        }

        // subjects
        if (!data.subjects || data.subjects.length === 0) {
            showError("Список предметов пуст");
        } else {
            data.subjects.forEach(sub => {
                const option = document.createElement("option");
                option.value = sub.value;
                option.text = sub.text;
                selectSubject.appendChild(option);
            });
        }
    }

    // =============================
    // 🔥 загрузка студентов
    // =============================
    async function loadStudents(classId) {

        const res = await fetch(`/AdministrationSchoolPerAcc/GetStudentsByClass?classId=${classId}`);

        let data;

        try {
            data = await res.json();
        } catch {
            showError("Ошибка сервера (ответ не JSON)");
            return;
        }

        if (!res.ok) {
            showError(data.message || "Ошибка загрузки студентов");
            return;
        }

        if (!data || data.length === 0) {
            showError("Список студентов пуст");
            return;
        }

        selectStudents.innerHTML = "";
        selectStudents.appendChild(GetOptionDefaultText("Выберите студента"));

        data.forEach(stu => {
            const option = document.createElement("option");
            option.value = stu.value;
            option.text = stu.text;
            selectStudents.appendChild(option);
        });
    }

    // =============================
    // 🔥 стартовая загрузка
    // =============================
    loadTerms(academicId);

    // =============================
    // 🔥 события
    // =============================
    selectAcademic.addEventListener("change", function () {
        academicId = selectAcademic.value;
        loadTerms(academicId);
    });

    selectClass.addEventListener("change", function () {
        const classId = selectClass.value;
        if (!classId) return;

        loadStudents(classId);
    });

});