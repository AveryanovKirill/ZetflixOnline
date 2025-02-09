const filters = document.querySelectorAll('.xfsort-select');
const results = document.getElementById('results');
const clearButton = document.querySelector('.xfsort-clear');
let currentOpenFilter = null;

filters.forEach(filter => {
    filter.querySelector('.xfsort-selected').addEventListener('click', function () {
        const ul = this.nextElementSibling;

        // Если текущая секция уже открыта, закрыть её
        if (currentOpenFilter === ul) {
            ul.style.display = 'none';
            currentOpenFilter = null;
        } else {
            // Скрыть все открытые списки
            filters.forEach(f => {
                f.querySelector('.xfsort-ul').style.display = 'none';
            });
            // Открыть текущий список
            ul.style.display = 'block';
            currentOpenFilter = ul;
        }
    });

    filter.querySelectorAll('.xfsort-ul li').forEach(item => {
        item.addEventListener('click', function () {
            const selectedValue = this.getAttribute('data-val');
            const field = this.parentElement.getAttribute('data-field');
            filter.querySelector('.xfsort-selected').textContent = this.textContent;
            filter.querySelector('.xfsort-ul').style.display = 'none';
            currentOpenFilter = null;
            applyFilters();
        });
    });
});

clearButton.addEventListener('click', function () {
    filters.forEach(filter => {
        filter.querySelector('.xfsort-selected').textContent = filter.querySelector('.xfsort-ul li.active').textContent;
        filter.querySelector('.xfsort-ul').style.display = 'none';
    });
    currentOpenFilter = null;
    applyFilters();
});


