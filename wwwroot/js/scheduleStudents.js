document.addEventListener("DOMContentLoaded", function () {

    const buttons = document.querySelectorAll(".btn_content .btn");
    const container = document.getElementById("conteinerSchedule");
    const studentInput = document.getElementById("studentIdForSchedule");

    async function loadSchedule(dayName) {
        const studentId = studentInput.value;

        const response = await fetch(`/ScheduleForStudent/ScheduleLesson?dayOfWeek=${dayName}&studentId=${studentId}`);

        const lessons = await response.json();

        container.innerHTML = "";

        if (!lessons || lessons.length === 0) {
            container.innerHTML = "<p>На этот день расписания нет</p>";
            return;
        }

        let html = `
    <table class="table">
        <thead>
            <tr>
                <th>Номер урока</th>
                <th>Предмет</th>
                <th>Учитель</th>
                <th>Время</th>
                <th>Кабинет</th>
            </tr>
        </thead>
        <tbody>
`;

        lessons.forEach(lesson => {
            html += `
        <tr>
            <td>${lesson.lessonNumber ?? ""}</td>
            <td>${lesson.subject ?? ""}</td>
            <td>${lesson.teacher ?? ""}</td>
            <td>${lesson.timeStart ?? ""} - ${lesson.timeEnd ?? ""}</td>
            <td>${lesson.room ?? ""}</td>
        </tr>
    `;
        });

        html += `
        </tbody>
    </table>
`;

        container.innerHTML = html;
    }

    buttons.forEach(button => {
        button.addEventListener("click", function () {

            buttons.forEach(btn => {
                btn.classList.remove("active-day");
            });

            this.classList.add("active-day");

            const dayName = this.dataset.day;

            loadSchedule(dayName);
        });
    });

    const days = [
        "воскресенье",
        "понедельник",
        "вторник",
        "среда",
        "четверг",
        "пятница",
        "суббота"
    ];

    let today = days[new Date().getDay()];

    if (today === "воскресенье") {
        today = "понедельник";
    }

    const todayButton = document.querySelector(`.btn_content .btn[data-day="${today}"]`);

    if (todayButton) {
        todayButton.click();
    }
});