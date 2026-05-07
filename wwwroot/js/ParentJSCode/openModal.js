document.addEventListener("DOMContentLoaded", () => {
    initModal("btnOpenModalJournal", "modelJournalView");
    initModal("btnOpenModalAvgScore", "modelAvgGradeForParent");
    initModal("btnOpenModalGraph", "graphModalForParent");
    initModal("btnOpenModalRating", "ratingModalForParent");
    initModal("scheduleStudentForParent", "scheduleModal");
    initModal("btnForRestoreBackup", "modalForRestoreBackup");
    initModal("btnAnaliticalReport", "modalAnaliticalReport");
});

function initModal(btnId, modalId) {
    const button = document.getElementById(btnId);
    const modal = document.getElementById(modalId);

    // 🔥 защита от ошибок
    if (!button || !modal) return;

    // открыть модалку
    button.addEventListener("click", () => {
        modal.classList.add("show");
    });

    // закрыть по клику вне окна
    window.addEventListener("click", (event) => {
        if (event.target === modal) {
            modal.classList.remove("show");
        }
    });
}