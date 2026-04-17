document.addEventListener("DOMContentLoaded", function () {

    const classResult = document.getElementById("studentClassId");

    if (classResult) {
        classResult.addEventListener("change", async function () {
            const classId = this.value;

            const response = await fetch(`/StudentPerAcc/GetClassData?classId=${classId}`);
            const data = await response.json();

            document.getElementById("academicValue").value = data.academicYear.name;
            document.getElementById("academicValueId").value = data.academicYear.id;

            const terms = document.getElementById("termList");
            terms.innerHTML = "";

            data.terms.forEach(t => {
                const option = document.createElement("option");
                option.value = t.id;
                option.text = t.name;
                terms.appendChild(option);
            });

            terms.disabled = false;
        });
    }

    const btn = document.getElementById("showGraphBtn");

    if (btn) {
        btn.addEventListener("click", async function () {

            console.log("BUTTON CLICKED");

            const studentId = document.getElementById("studentID").value;
            const classId = document.getElementById("studentClassId").value;
            const academicId = document.getElementById("academicValueId").value;
            const termId = document.getElementById("termList").value;


            const params = new URLSearchParams({
                studentId,
                classId,
                academicId,
                termId
            });

            const url = `/StudentPerAcc/GraphShow?${params}`;
            window.location.href = url;
        });
    }
});
