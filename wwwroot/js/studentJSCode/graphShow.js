document.addEventListener("DOMContentLoaded", async function () {
    const params = new URLSearchParams(window.location.search);
    const studentId = params.get("studentId");
    const classId = params.get("classId");
    const academicId = params.get("academicId");
    const termId = params.get("termId");

    const response = await fetch(`/StudentPerAcc/GetQuarterStats?studentId=${studentId}&classId=${classId}&academicId=${academicId}&termId=${termId}`);
    const data = await response.json();

    console.log("Data:", data);

    const labels = data.map(x => x.subject); // получения всех названий предметов из массива
    const averages = data.map(x => Number(x.average.toFixed(2))); // получения всех значений среднего балл по предметам

    const backgroundColors = averages.map(s => {
        if (s <= 3.5) return "rgba(255, 99, 132, 0.7)";
        else if (s <= 4.5) return "rgba(255, 205, 86, 0.7)";
        else return "rgba(75, 192, 92, 0.7)";
    });
    const borderColors = averages.map(avg => {
        if (avg <= 3.5) return "red";
        if (avg <= 4.5) return "yellow";
        else return "green";
    });
    const ctx = document.getElementById("chart").getContext("2d");

    new Chart(ctx, { // создает новы график и ставляет его в canvas
        type: "bar", // тип столбчатый
        data: { // задаються данные для графика 
            labels: labels, // навазние предметов (подписи снизу)
            datasets: [ // наборы данных, которые рисуются на графике.
                {
                    label: "Средний балл",
                    data: averages, // средний балл (высота столбца)
                    backgroundColor: backgroundColors, // цвет столбца
                    borderColor: borderColors, // цвет рамки столбца
                    borderWidth: 1, // толщина рамки
                    barPercentage: 0.6, // ширина столбца
                    categoryPercentage: 0.7 // сколько места дать предмету целиком
                }
            ]
        },
        options: { // настройки графика
            responsive: true, // график подстраивается под размер экрана или блока
            maintainAspectRatio: false, // Значит не держать строго старые пропорции.
            plugins: { // Это дополнительные части графика
                legend: {
                    display: false // не показывать встроенную легенду Легенда — это маленькое пояснение сверху, типа:синий — продажи красный — расходы
                },
                title: { // заголовок графика
                    display: true, // показать название заголовка
                    text: "Успеваемость по предметам"
                }
            },
            scales: { // Это настройка осей графика.
                y: {
                    beginAtZero: true, // начать шкаолу с 0
                    max: 5 // максимальное число
                }
            }
        }
    });
});