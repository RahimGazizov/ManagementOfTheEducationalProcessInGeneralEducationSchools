function bindSubjectToClasses(subjectId, classId) {
    const subjectSelect = document.getElementById(subjectId);
    const classSelect = document.getElementById(classId);
    if (!subjectSelect || !classSelect) return;

    subjectSelect.addEventListener("change", async () => {
        const subjectValue = subjectSelect.value;

        classSelect.disabled = true;
        classSelect.innerHTML = "";

        if (!subjectValue) {
            classSelect.innerHTML = '<option value="">Сначала выберите предмет</option>';
            return;
        }

        classSelect.innerHTML = '<option value="">Загрузка...</option>';

        try {
            const res = await fetch(`/TeacherPerAcc/GetClassesBySubject?subjectId=${encodeURIComponent(subjectValue)}`);
            if (!res.ok) throw new Error(`HTTP ${res.status}`);

            const classes = await res.json();

            classSelect.innerHTML = '<option value="">Выберите класс</option>';

            if (!classes || classes.length === 0) {
                classSelect.innerHTML = '<option value="">Нет списка классов</option>';
                return;
            }

            for (const c of classes) {
                const opt = document.createElement("option");
                opt.value = c.id;         // или c.Id, смотри как отдаёшь JSON
                opt.textContent = c.name; // или c.Name
                classSelect.appendChild(opt);
            }   

            classSelect.disabled = false;
        } catch (err) {
            classSelect.innerHTML = '<option value="">Ошибка загрузки</option>';
            console.error(err);
        }
    });
}

document.addEventListener("DOMContentLoaded", () => {
    bindSubjectToClasses("subjectSelectCreate", "classSelectCreate");
    bindSubjectToClasses("subjectSelectHistory", "classSelectHistory");
});
