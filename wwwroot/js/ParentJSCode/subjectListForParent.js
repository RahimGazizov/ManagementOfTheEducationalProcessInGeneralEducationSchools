document.addEventListener("DOMContentLoaded", () => {
    const studentList = document.getElementById("parentStudentId");
    const subjectListener = document.getElementById("parentStudentSubjectId");
    studentList.addEventListener("change", async () => {
        subjectListener.innerHTML = `<option value="">Выберите предмет</option>`;
        const studentId = studentList.value;
        const res = await fetch(`/StudentPerAcc/SubjectList?studentId=${studentId}`);
        if (!res.ok) { console.log("Не удалось загрузить предметы"); return; }
        const data = await res.json();
        data.forEach(sub => {
            const option = document.createElement("option");
            option.value = sub.id;
            option.text = sub.name;
            subjectListener.appendChild(option);
        });
        subjectListener.disabled = false;
    });
});