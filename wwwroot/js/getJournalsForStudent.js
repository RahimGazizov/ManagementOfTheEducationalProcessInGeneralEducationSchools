

document.addEventListener("DOMContentLoaded", () => {
    const subjectSelect = document.getElementById("subjectSelectHistory");
    const dateFrom = document.getElementById("dateFrom");
    const dateTo = document.getElementById("dateTo");
    const classID = document.getElementById("classID");
    const studentId = document.getElementById("studentID");
    const journalList = document.getElementById("journalsList");

    if (!subjectSelect || !classID || !studentId) return;

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
        params.append("classId", classID.value);
        if (dateFrom.value) params.append("dateFrom", dateFrom.value);
        if (dateTo.value) params.append("dateTo", dateTo.value);

        selectListText("Загрузка...");

        try {
            const res = await fetch(`/StudentPerAcc/JournalSet?${params.toString()}`);
            if (!res.ok) throw new Error(`HTTP ${res.status}`);

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
                info.textContent = `Дата создания: ${formatDate(j.lastDate)}`;

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
    dateFrom.addEventListener("change", loadJournals);
    dateTo.addEventListener("change", loadJournals);
});