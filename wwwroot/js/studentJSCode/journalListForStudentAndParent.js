function JournalsInfo(student, subjectList, journalListForStudent) {
    const studentId = document.getElementById(student);
    const journalList = document.getElementById(journalListForStudent);
    const subjectSelect = document.getElementById(subjectList);
    if (!studentId || !subjectSelect || !journalList) {
        console.log("studentId, subjectSelect или journalList не найден");
        return;
    }
    else {
        LoadSub();
    }

    async function LoadSub() {
        subjectSelect.innerHTML = `<option value="">Выберите предмет</option>`;
        const res = await fetch(`/StudentPerAcc/SubjectList?studentId=${studentId.value}`);
        if (!res.ok) { console.log("Список предметов не загрузились"); return; }
        const dataSub = await res.json();
        dataSub.forEach(data => {
            const option = document.createElement("option");
            option.value = data.id;
            option.text = data.name;
            subjectSelect.appendChild(option);
        });
    }
    function selectListText(text) {
        journalList.innerHTML = text;
    }

    function formatDate(d) {
        if (!d) return "-";
        return new Date(d).toLocaleDateString();
    }

    async function loadJournals() {
        const subject = subjectSelect.value;
        if (!subject) {
            selectListText("Выберите предмет чтобы увидеть список журналов");
            return;
        }

        const params = new URLSearchParams();
        params.append("studentId", studentId.value);
        params.append("subjectId", subject);

        selectListText("Загрузка...");

        try {
            const res = await fetch(`/StudentPerAcc/JournalSet?${params.toString()}`);
            if (!res.ok) {
                const errorText = await res.text();
                console.error("Ошибка сервера:", errorText);
                throw new Error(`HTTP ${res.status}`);
            }

            const journals = await res.json();
            if (!journals || journals.length === 0) {
                selectListText("Журналы не найдены по заданным параметрам");
                return;
            }

            journalList.innerHTML = "";

            for (const j of journals) {
                const item = document.createElement("div");
                item.className = "journal_item";

                const title = document.createElement("div");
                title.textContent = `Журнал: ${j.subjectName}`;

                const info = document.createElement("div");
                info.textContent = `Дата создания: ${formatDate(j.date)}`;

                item.appendChild(title);
                item.appendChild(info);

                const link = document.createElement("a");
                link.className = "journal_open";
                link.textContent = "Открыть";
                link.href = `/StudentPerAcc/JournalInfo?id=${j.id}`;
                item.appendChild(link);

                journalList.appendChild(item);
            }
        } catch (err) {
            console.error(err);
            selectListText("Ошибка загрузки журналов");
        }
    }

    subjectSelect.addEventListener("change", loadJournals);
}
document.addEventListener("DOMContentLoaded", () => {
    JournalsInfo("studentID", "subjectSelectHistory", "journalListForStudent");
    JournalsInfo("parentStudentId", "parentStudentSubjectId", "journalListForParent");
});