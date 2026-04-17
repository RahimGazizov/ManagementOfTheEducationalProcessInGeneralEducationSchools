document.addEventListener("DOMContentLoaded", function () {

    console.log("JS loaded");

    const classSelect = document.getElementById("classStudent");

    // ❗ проверка элемента
    if (!classSelect) {
        console.error("❌ classStudent element not found");
        return;
    }

    classSelect.addEventListener("change", async function () {

        const classId = this.value;
        console.log("Selected classId:", classId);

        if (!classId) {
            console.warn("⚠ classId is empty");
            return;
        }

        try {
            const response = await fetch(`/StudentPerAcc/GetClassData?classId=${classId}`);

            // ❗ проверка ответа сервера
            if (!response.ok) {
                console.error("❌ Server error:", response.status);
                return;
            }

            const data = await response.json();
            console.log("Server response:", data);

            // ❗ academicYear
            if (data.academicYear) {
                document.getElementById("academicYear").value = data.academicYear.name;
                document.getElementById("academicYearId").value = data.academicYear.id;
            } else {
                console.warn("⚠ academicYear is null");
            }

            // ❗ subjects
            const subject = document.getElementById("listSubject");

            if (!subject) {
                console.error("❌ listSubject not found");
                return;
            }

            subject.innerHTML = "";

            if (!data.subjects || data.subjects.length === 0) {
                console.warn("⚠ subjects is empty");
            } else {
                data.subjects.forEach(s => {
                    const option = document.createElement("option");
                    option.value = s.id;
                    option.textContent = s.name;
                    subject.appendChild(option);
                });
                subject.disabled = false;
            }

            // ❗ terms
            const term = document.getElementById("listTerms");

            if (!term) {
                console.error("❌ listTerms not found");
                return;
            }

            term.innerHTML = "";

            if (!data.terms || data.terms.length === 0) {
                console.warn("⚠ terms is empty");
            } else {
                data.terms.forEach(t => {
                    const option = document.createElement("option");
                    option.value = t.id;
                    option.textContent = t.name;
                    term.appendChild(option);
                });
                term.disabled = false;
            }

        } catch (error) {
            console.error("❌ Fetch error:", error);
        }
    });
    document.getElementById("btnResult").addEventListener("click", async function () {

        const classId = document.getElementById("classStudent").value;
        const subjectId = document.getElementById("listSubject").value;
        const academicId = document.getElementById("academicYearId").value;
        const termId = document.getElementById("listTerms").value;
        const studentId = document.getElementById("studentId").value;
        if (!classId || !subjectId || !academicId || !termId || !studentId) return;

        const params = new URLSearchParams();
        params.append("classId", classId);
        params.append("subjectId", subjectId);
        params.append("academicId", academicId);
        params.append("termId", termId);
        params.append("studentId", studentId);
        const response = await fetch(`/StudentPerAcc/GetResultInfo?${params.toString()}`);
        if (!response.ok) {
            console.error("❌ Server error:", response.status);
            return;
        }
        const data = await response.json();
        document.getElementById("resSubject").textContent = data.name;
        document.getElementById("resAverage").textContent = data.average;

        const percent = data.percent;

        document.getElementById("resAttendance").textContent = percent.toFixed(1) + "%";

        // если есть progress bar
        document.getElementById("attendanceBar").style.width = percent + "%";
    });
});
