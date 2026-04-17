document.addEventListener("DOMContentLoaded", function () {
    const studentList = document.getElementById("parentStudentIdForAvgScore");
    const subjectListener = document.getElementById("parentStudentSubjectIdForAngScore");
    const classList = document.getElementById("classIdForAvgScoreParent");
    const academicId = document.getElementById("academicIdForAvgScoreParent");
    const academic = document.getElementById("academicForAvgScoreParent");
    const term = document.getElementById("termIdForAvgScoreParent");
    studentList.addEventListener("change", async () => {
        classList.innerHTML = `<option value="">Выберите класс</option>`;
        const studentId = studentList.value;
        const res = await fetch(`/ParentPerAccount/ClassList?studentId=${studentId}`);
        if (!res.ok) { console.log("Не удалось загрузить классы"); return; }
        const data = await res.json();
        data.forEach(cls => {
            const option = document.createElement("option");
            option.value = cls.id;
            option.text = cls.name;
            classList.appendChild(option);
        });
        classList.disabled = false;
    });
    classList.addEventListener("change", async function () {
        subjectListener.innerHTML = `<option value="">Выберите предмет</option>`
        term.innerHTML = `<option value="">Выберите четверть</option>`
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
        data.subjects.forEach(sub => {
            const option = document.createElement("option");
            option.value = sub.id;
            option.text = sub.name;
            subjectListener.appendChild(option);
        });
        subjectListener.disabled = false;

        
        data.terms.forEach(t => {
            const option = document.createElement("option");
            option.value = t.id;
            option.text = t.name;
            term.appendChild(option);
        });
        term.disabled = false;
    });
    document.getElementById("resultBtn").addEventListener("click", async function () {
        console.log("btn click");
        const params = new URLSearchParams();
        params.append("classId", classList.value);
        params.append("subjectId", subjectListener.value);
        params.append("academicId", academicId.value);
        params.append("termId", term.value);
        params.append("studentId", studentList.value);

        const res = await fetch(`/StudentPerAcc/GetResultInfo?${params.toString()}`);
        if (!res.ok) { console.log("Ошибка вывода информации о среднем балле"); return; }
        const data = await res.json();

        document.getElementById("resSubjectStudent").textContent = data.name;
        document.getElementById("resAverageStudent").textContent = data.average;
        document.getElementById("resAttendanceStudent").textContent = data.percent.toFixed(1) + "%";

        document.getElementById("attendanceBarStudentForParent").style.width = data.percent + "%";
    });
});