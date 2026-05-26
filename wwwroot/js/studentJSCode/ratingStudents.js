document.addEventListener("DOMContentLoaded", function () {
    ShowRating(
        "classIdForRating",
        "academicForRating",
        "academicIdForRating",
        "termIdForRating",
        "showRatingBtn",
        "resultRatingForStudent",
        "studentIdForRating"
    );
});

async function ShowRating(classI, academicI, academic, term, btn, result, student) {

    const classvalue = document.getElementById(classI);
    const academicForRating = document.getElementById(academicI);
    const academicIdForRating = document.getElementById(academic);
    const termsList = document.getElementById(term);
    const btnShow = document.getElementById(btn);
    const resultBlock = document.getElementById(result);
    const studentEl = document.getElementById(student);
    const error = document.getElementById("errorMessage");
    if (!classvalue || !academicForRating || !academicIdForRating || !termsList || !btnShow || !resultBlock || !studentEl) {
        console.log("Один из элементов не найден");
        return;
    }

    const studentId = studentEl.value;

    classvalue.addEventListener("change", async function () {

        const classData = this.value;

        const res = await fetch(`/StudentPerAcc/GetClassData?classId=${classData}`);

        if (!res.ok) {
            console.log("Ошибка: данные не загружены");
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

        const classId = classvalue.value;
        const academicId = academicIdForRating.value;
        const termId = termsList.value;


        const params = new URLSearchParams({
            studentId,
            classId,
            academicId,
            termId
        });
        let res;
        let data;

        try {
            res = await fetch(`/StudentPerAcc/GetClassRating?${params}`);
        }
        catch {
            console.log("Ошибка запроса к серверу");
            error.innerText = "Ошибка получение рейтинга"
            setTimeout(() => error.innerText = "", 3000);
            return;
        }

        try {
            data = await res.json();
        }
        catch {
            console.log("Данные пришли не в формате JSON");
            error.innerText = "Ошибка получение рейтинга"
            setTimeout(() => error.innerText = "", 3000);
            return;
        }

        if (!res.ok || data.success === false) {
            error.innerText = data.message;
            setTimeout(() => error.innerText = "", 3000);
            return;
        }

        console.log("Ответ рейтинга:", data);

        if (!data.success) {
            console.error("Ошибка:", data.message);
            return;
        }

        const ratingCurrentClass = data.ratingCurrentClass;
        const ratingParallelClass = data.ratingParallelClass;

        const currentClassStudent = ratingCurrentClass.ratingStudent;

        let top3Html = "";
        ratingCurrentClass.top3.forEach(itm => {
            top3Html += `
        <div class="rating-row">
            <span>${itm.place}</span>
            <span>${itm.studentName}</span>
            <span>${itm.score}</span>
        </div>
    `;
        });

        let parallelHtml = "";
        let top3ParallelHtml = "";
        let currentParallelStudent = null;

        if (ratingParallelClass && ratingParallelClass.ratingStudent) {

            currentParallelStudent = ratingParallelClass.ratingStudent;

            ratingParallelClass.top3Parallel.forEach(itm => {
                top3ParallelHtml += `
            <div class="rating-row">
                <span>${itm.place}</span>
                <span>${itm.studentName} (${itm.classNum}${itm.classLetter ?? ""})</span>
                <span>${itm.score}</span>
            </div>
        `;
            });

            parallelHtml = `
        <h4>Рейтинг среди параллельных классов</h4>

        <div class="result-item">
            <span>Ваше место:</span>
            <span>${currentParallelStudent.place}/${ratingParallelClass.totalStudent}</span>
        </div>

        <div class="result-item">
            <span>Средний балл:</span>
            <span>${currentParallelStudent.average}</span>
        </div>

        <div class="result-item">
            <span>Посещаемость:</span>
            <span>${currentParallelStudent.percent}</span>
        </div>

        <div class="result-item">
            <span>Итоговый балл:</span>
            <span>${currentParallelStudent.score}</span>
        </div>

        <hr>

        <h5>Топ 3 параллели</h5>
        <div class="rating-list">
            ${top3ParallelHtml}
        </div>
    `;
        }
        else {
            parallelHtml = `
        <h4>Рейтинг среди параллельных классов</h4>
        <p>${data.message ?? "Рейтинг параллели пока недоступен"}</p>
    `;
        }

        let html = `
<h4>Рейтинг класса</h4>

<div class="result-item">
    <span>Ваше место:</span>
    <span>${currentClassStudent.place}/${ratingCurrentClass.totalStudent}</span>
</div>

<div class="result-item">
    <span>Средний балл:</span>
    <span>${currentClassStudent.average}</span>
</div>

<div class="result-item">
    <span>Посещаемость:</span>
    <span>${currentClassStudent.percent}</span>
</div>

<div class="result-item">
    <span>Итоговый балл:</span>
    <span>${currentClassStudent.score}</span>
</div>

<hr>

<h5>Топ 3 класса</h5>
<div class="rating-list">
    ${top3Html}
</div>

${parallelHtml}
`;

        document.getElementById("resultContainer").innerHTML = html;
        resultBlock.innerHTML = html;
        resultBlock.classList.add("showRating");
    });
}