document.addEventListener("DOMContentLoaded", () => {

    const subjectHistory = document.getElementById("subjectSelectHistory");
    const classHistory = document.getElementById("classSelectHistory");

    const journalsList = document.getElementById("journalsList");

    // если модалка/элементы не на этой странице — просто выходим
    if (!subjectHistory || !classHistory || !journalsList) {
        return;
    }

    function setListText(text) {
        journalsList.innerHTML = text;
    }

    function formatDate(d) {
        if (!d) return "-";
        return new Date(d).toLocaleDateString();
    }

    async function loadJournals() {
        const subjectId = subjectHistory.value;
        const classId = classHistory.value;

        if (!subjectId || !classId) {
            setListText("Выберите предмет и класс, чтобы увидеть журналы");
            return;
        }

        const params = new URLSearchParams();
        params.append("subjectId", subjectId);
        params.append("classId", classId);

        setListText("Загрузка...");

        try {
            const res = await fetch(`/TeacherPerAcc/JournalHistory?${params.toString()}`); // см. пункт ниже
            if (!res.ok) throw new Error(`HTTP ${res.status}`);

            const journals = await res.json();

            if (!journals || journals.length === 0) {
                setListText("Журналы не найдены по выбранным фильтрам");
                return;
            }

            journalsList.innerHTML = "";

            for (const j of journals) {
                const item = document.createElement("div");
                item.className = "journal_item";

                const meta = document.createElement("div");
                meta.className = "journal_meta";

                const title = document.createElement("div");
                title.textContent = `Журнал #${j.id}`;

                const info = document.createElement("div");
                info.textContent = `Дата создания: ${formatDate(j.date)}`;

                meta.appendChild(title);
                meta.appendChild(info);

                const link = document.createElement("a");
                link.className = "journal_open";
                link.textContent = "Открыть";
                link.href = `/Journal/Edit?id=${j.id}`;

                item.appendChild(meta);
                item.appendChild(link);

                journalsList.appendChild(item);
            }
        } catch (err) {
            console.error(err);
            setListText("Ошибка загрузки журналов");
        }
    }

    classHistory.addEventListener("change", loadJournals);
});