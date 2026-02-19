const numClassSelect = document.getElementById("numClassSelect");
const letterClassSelect = document.getElementById("letterClassSelect");

// берем текущее значение select, если оно есть (для редактирования)
const currentClassId = letterClassSelect.value;

function loadLettersForSelectedClass() {
    const numClass = numClassSelect.value;
    if (!numClass) return;

    letterClassSelect.innerHTML = '<option value="">Загрузка...</option>';

    fetch(`/Users/GetLetterClass?numClass=${numClass}`)
        .then(res => res.json())
        .then(data => {
            letterClassSelect.innerHTML = '<option value="">Выберите букву</option>';
            data.forEach(c => {
                const opt = document.createElement("option");
                opt.value = c.id;
                opt.textContent = c.letterClass;
                // отмечаем текущую букву, если совпадает с value
                if (c.id == currentClassId) {
                    opt.selected = true;
                }
                letterClassSelect.appendChild(opt);
            });
        })
        .catch(() => {
            letterClassSelect.innerHTML = '<option value="">Ошибка загрузки</option>';
        });
}

// сразу вызываем, чтобы выбрать текущую букву
loadLettersForSelectedClass();
numClassSelect.addEventListener("change", loadLettersForSelectedClass);
