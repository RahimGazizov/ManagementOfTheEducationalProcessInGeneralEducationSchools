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
        let res;
        let data;
        try {
            res = await fetch(`/StudentPerAcc/GetClassRating?${params}`);
        }
        catch {
            error.innerText = "Ошибка запроса к серверу";
            setTimeout(function () {
                error.innerText = "";
            }, 3000);
            return;
        }
        try {
            data = await res.json();
        }
        catch {
            error.innerText = "Данные пришли не в JSON формате";
            setTimeout(function () {
                error.innerText = "";
            }, 3000);
            return;
        }
        if (!res.ok || data.success === false) {
            error.innerText = data.message;
            setTimeout(function () {
                error.innerText = "";
            }, 3000);
            return;
        }
        console.log("Ответ рейтинга:", data);

        if (!data.success) {
            console.error("Ошибка:", data.message);
            return;
        }

        let top3Html = "";
        let top3Parallel = "";

        const ratingCurrentClass = data.ratingCurrentClass;
        const ratingParallelClass = data.ratingParallelClass;

        const currentClassStudent = ratingCurrentClass.ratingStudent;

        ratingCurrentClass.top3.forEach(itm => {
            top3Html += `
        <div class="rating-row">
            <span>${itm.place}</span>
            <span>${itm.studentName}</span>
            <span>${itm.score}</span>
        </div>
    `;
        });

        let currentParallelStudent = null;

        if (ratingParallelClass) {
            currentParallelStudent = ratingParallelClass.ratingStudent;

            ratingParallelClass.top3Parallel.forEach(itm => {
                top3Parallel += `
            <div class="rating-row">
                <span>${itm.place}</span>
                <span>${itm.studentName} (${itm.classNum}${itm.classLetter ?? ""})</span>
                <span>${itm.score}</span>
            </div>
        `;
            });
        } else {
            top3Parallel = `
        <p>${data.message ?? "Рейтинг параллели пока недоступен"}</p>
    `;
        }

        let html = `
    <h4>Рейтинг класса</h4>

    <div class="result-item">
        <span>Ваше место в классе:</span>
        <span>${currentClassStudent.place}/${ratingCurrentClass.totalStudent}</span>
    </div>

    <div class="result-item">
        <span>Средний балл:</span>
        <span>${currentClassStudent.average}</span>
    </div>

    <div class="result-item">
        <span>Процент посещаемости:</span>
        <span>${currentClassStudent.percent}</span>
    </div>

    <div class="result-item">
        <span>Итоговый балл:</span>
        <span>${currentClassStudent.score}</span>
    </div>

    <hr>

    <h5>Топ 3 ученика класса</h5>
    <div class="rating-list">
        ${top3Html}
    </div>

    <h4>Рейтинг среди параллельных классов</h4>

    <div class="result-item">
        <span>Ваше место среди параллельных классов:</span>
        <span>${currentParallelStudent.place}/${ratingParallelClass.totalStudent}</span>
    </div>

    <div class="result-item">
        <span>Средний балл:</span>
        <span>${currentParallelStudent.average}</span>
    </div>

    <div class="result-item">
        <span>Процент посещаемости:</span>
        <span>${currentParallelStudent.percent}</span>
    </div>

    <div class="result-item">
        <span>Итоговый балл:</span>
        <span>${currentParallelStudent.score}</span>
    </div>

    <hr>

    <h5>Топ 3 ученика параллельных классов</h5>
    <div class="rating-list">
        ${top3Parallel}
    </div>
`;
        resultBlock.innerHTML = html;
        resultBlock.classList.add("showRating");
    });
});