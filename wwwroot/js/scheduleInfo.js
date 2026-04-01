document.addEventListener("DOMContentLoaded", function () {

    const buttons = document.querySelectorAll(".btn");
    const conteiner = document.getElementById("conteinerSchedule");
    const studentId = document.getElementById("studentIdForSchedule").value;
    if (!buttons) return;
    buttons.forEach(btn => {
        btn.addEventListener("click", async function () {
            const day = this.dataset.day;
            const res = await fetch(`/ScheduleForStudent/ScheduleLesson?dayOfWeek=${day}&studentId=${studentId}`);
            if (!res.ok) {
                console.log("Ошибка получения данных");
                return;
            }
            const lessons = await res.json();
            console.log("Schedule", lessons);
            conteiner.innerHTML = "";
            if (lessons.length === 0) {
                conteiner.innerHTML = "<p>На этот день расписания нет</p>";
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
                        <td>${lesson.lessonNumber}</td>
                        <td>${lesson.subject}</td>
                        <td>${lesson.teacher}</td>
                        <td>${lesson.timeStart} - ${lesson.timeEnd}</td>
                        <td>${lesson.room}</td>
                    </tr>
                `;
            });

            html += `</tbody></table>`;
            console.log("container:", conteiner);
            console.log("html:", html);
            conteiner.innerHTML = html;
        })
    })
});