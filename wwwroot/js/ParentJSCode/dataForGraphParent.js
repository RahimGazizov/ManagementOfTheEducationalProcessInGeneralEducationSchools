document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("resultBtnGraph").addEventListener("click", async function () {
        const studentId = document.getElementById("parentStudentIdForGraph").value;
        const classId = document.getElementById("classIdForGraphParent").value;
        const academicId = document.getElementById("academicIdForGraphParent").value;
        const termId = document.getElementById("termIdForGraphParent").value;
        const params = new URLSearchParams({
            studentId,
            classId,
            academicId,
            termId
        });
        const url = `/StudentPerAcc/GraphShow?${params}`;
        window.location.href = url;
    });

});