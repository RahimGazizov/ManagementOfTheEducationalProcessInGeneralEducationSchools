document.addEventListener("DOMContentLoaded", function () {
    ShowRating("classIdForRating", "academicForRating", "academicIdForRating", "termIdForRating", "showRatingBtn", "resultRatingForStudent", "studentIdForRating");
});
async function ShowRating(classI, academicI, academic, term, btn, result, student) {
    const classId = document.getElementById(classI);
    const academicForRating = document.getElementById(academicI);
    const academicIdForRating = document.getElementById(academic);
    const termsList = document.getElementById(term);
    const btnShow = document.getElementById(btn);
    const resultBlock = document.getElementById(result);
    const studentId = document.getElementById(student).value;

    if (!classId) { console.log("Элемент класса не найден"); return; }

    classId.addEventListener("change", async function () {
        const classData = this.value;

        const res = await fetch(`/StudentPerAcc/GetClassData?classId=${classData}`);
        if (!res.ok) {
            console.log("Ошибка данные не загружены");
            return;
        }

        const data = await res.json();
        if (data.academicYear) {
            academicForRating.value = data.academicYear.name;
            academicIdForRating.value = data.academicYear.id;
        }


        termsList.innerHTML = "";

        data.terms.forEach(t => {
            const option = document.createElement("option");
            option.value = t.id;
            option.text = t.name;
            termsList.appendChild(option);
        });
        termsList.disabled = false;
    });
    btnShow.addEventListener("click", async function () {
        console.log("ButtonClick");
        const classId = classId.value;
        const academicId = academicIdForRating.value;
        const termId = termsList.value;
        //if (!studentId || !classId || !academicId || !termId) {
        //    console.log("Не все данные заполнены");
        //    return;
        //}
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
        <span>Ваше место в классе:</span>
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
        <span>Ваше место в среди параллельных классов:</span>
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
}
