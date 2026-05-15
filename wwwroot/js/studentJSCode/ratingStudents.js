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

        if (!studentId || !classId || !academicId || !termId) {
            console.log("Не все данные заполнены");
            return;
        }

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
}