document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("resultBtnRating").addEventListener("click", async function () {
        const classId = document.getElementById("classIdForRatingParent").value;
        const studentId = document.getElementById("parentStudentIdForRating").value;
        const academicId = document.getElementById("academicIdForRatingParent").value;
        const termId = document.getElementById("termIdForRatingParent").value;
        const resultBlock = document.getElementById("resultRatingForParent");
        const params = new URLSearchParams({
            studentId,
            classId,
            academicId,
            termId
        });
        if (!params) {
            console.log("Данные пусты для передачи в сервер");
            return;
        }
        const res = await fetch(`/StudentPerAcc/GetClassRating?${params}`);
        if (!res.ok) {
            console.log("Данные с сервера пусты", res);
            return;
        }
        const data = await res.json();
        let top3Html = "";
        let top3Parallel = "";
        data.top3.forEach(itm => {
            top3Html += `
            <div class="rating-row">
            <span>${itm.place}</span>
            <span>${itm.studentName}</span>
            <span>${itm.score}</span>
            </div>
            `
        });
        data.ratingParallel.top3Parallel.forEach(itm => {
            top3Parallel += `
            <div class="rating-row">
            <span>${itm.place}</span>
            <span>${itm.studentFullName}(${itm.classNum}${itm.classLetter})</span>
            <span>${itm.score}</span>
            </div>
            `
        });
        let html = `<h4>Рейтинг класса</р4>
        <div class="result-item">
        <span>Место вашего ребенка в классе:</span>
        <span>${data.currentUserRating.place}/${data.totalStudent}</span>
        </div>
        <div class="result-item">
        <span>Средний балл:</span>
        <span>${data.currentUserRating.avgGrade}</span>
        </div>
        <div class="result-item">
        <span>Процент посещаймости:</span>
        <span>${data.currentUserRating.attendancePercent}</span>
        </div>
        <div class="result-item">
         <span>Итоговый балл</span>
         <span>${data.currentUserRating.score}</span>
        </div>
        </hr>
        <h5>Топ 3 ученика класса</h5>
        <div class="rating-list">
        ${top3Html}
        </div>
        <h4>Рейтинг среди параллельных классов</р4>
        <div class="result-item">
        <span>Место в среди параллельных классов вашего ребенка:</span>
        <span>${data.ratingParallel.currentUser.place}/${data.ratingParallel.totalStudentParallel}</span>
        </div>
        <div class="result-item">
        <span>Средний балл:</span>
        <span>${data.ratingParallel.currentUser.avgGrade}</span>
        </div>
        <div class="result-item">
        <span>Процент посещаймости:</span>
        <span>${data.currentUserRating.attendancePercent}</span>
        </div>
        <div class="result-item">
         <span>Итоговый балл</span>
         <span>${data.ratingParallel.currentUser.score}</span>
        </div>
        </hr>
        <h5>Топ 3 ученика параллельных классов</h5>
        <div class="rating-list">
        ${top3Parallel}
        </div>
        `
        resultBlock.innerHTML = html;
        resultBlock.classList.add("showRating");
    });
});