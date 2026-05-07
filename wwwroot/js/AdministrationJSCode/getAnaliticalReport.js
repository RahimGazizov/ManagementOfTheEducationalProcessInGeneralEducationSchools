document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("showReport").addEventListener("click", async function () {
        const academicId = document.querySelector(".academicListForAnalitical").value;
        const termId = document.querySelector(".termListForAnalitical").value;
        const classId = document.querySelector(".classListForAnalitical").value;
        const subjectId = document.querySelector(".subjectListForAnalitical").value;
        const studentId = document.querySelector(".studentsForAnalitical").value;
        const error = document.getElementById("errorMessage");
        if (!academicId) {
            error.innerText = "Учебный год пуст выберите значение";
            return;
        }
        if (!termId) {
            error.innerText = "Четверть пуста выберите значение";
            return;
        }
        if (!classId) {
            error.innerText = "Класс пуст выберите значение";
            return;
        }
        if (!subjectId) {
            error.innerText = "Предмет пуст выберите значение";
            return;
        }
        const url = `/AdministrationSchoolPerAcc/AnaliticalReport?academicId=${academicId}&termId=${termId}&classId=${classId}&subjectId=${subjectId}&studentId=${studentId}`;
        window.location.href = url;
    });
});