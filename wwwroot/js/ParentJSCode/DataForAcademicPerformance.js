document.addEventListener("DOMContentLoaded", function () {
    Data("parentStudentIdForGraph", "classIdForGraphParent", "academicIdForGraphParent", "academicForGraphParent", "termIdForGraphParent");
    Data("parentStudentIdForRating", "classIdForRatingParent", "academicIdForRatingParent", "academicForRatingParent", "termIdForRatingParent");
});

function Data(student, classL, academicI, academicV, termI) {
    const studentList = document.getElementById(student);
    const classList = document.getElementById(classL);
    const academicId = document.getElementById(academicI);
    const academic = document.getElementById(academicV);
    const term = document.getElementById(termI);

    if (!studentList || !classList || !academicId || !academic || !term) return;

    studentList.onchange = async function () {
        classList.innerHTML = `<option value="">Выберите класс</option>`;
        term.innerHTML = `<option value="">Выберите четверть</option>`;
        classList.disabled = true;
        term.disabled = true;
        academicId.value = "";
        academic.value = "";

        const studentId = studentList.value;
        if (!studentId) return;

        const res = await fetch(`/ParentPerAccount/ClassList?studentId=${studentId}`);
        if (!res.ok) {
            console.log("Не удалось загрузить классы");
            return;
        }

        const data = await res.json();

        data.forEach(cls => {
            const option = document.createElement("option");
            option.value = cls.id;
            option.text = cls.name;
            classList.appendChild(option);
        });

        classList.disabled = false;
    };

    classList.onchange = async function () {
        term.innerHTML = `<option value="">Выберите четверть</option>`;
        term.disabled = true;
        academicId.value = "";
        academic.value = "";

        if (!classList.value) return;

        const res = await fetch(`/ParentPerAccount/SubjectList?classId=${classList.value}`);
        if (!res.ok) {
            console.log("Не удалось загрузить предметы");
            return;
        }

        const data = await res.json();

        if (!data.academic) {
            console.log("Не удалось загрузить учебный год");
            return;
        }

        academicId.value = data.academic.id;
        academic.value = data.academic.name;

        data.terms.forEach(t => {
            const option = document.createElement("option");
            option.value = t.id;
            option.text = t.name;
            term.appendChild(option);
        });

        term.disabled = false;
    };
}