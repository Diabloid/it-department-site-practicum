// ПРИМУСОВЕ ПРОКРУЧУВАННЯ ВГОРУ (для чистого старту анімацій)
window.onbeforeunload = function () {
    window.scrollTo(0, 0);
};


// -------------------------------------------------------------
// --- Header hide/show on scroll (ОНОВЛЕНО: завжди видно на верху) ---
// -------------------------------------------------------------
let scrollPrev = 0;
const header = document.querySelector('.header');

if (header) { // Додана перевірка для уникнення помилок
    window.addEventListener('scroll', () => {
        const scrolled = window.pageYOffset || document.documentElement.scrollTop;

        // НОВЕ: Хедер завжди висовується, коли позиція прокрутки 0
        if (scrolled === 0) {
            header.classList.remove('header-out');
            scrollPrev = scrolled;
            return; // Виходимо, щоб не застосовувати інші правила
        }

        // Скролимо вниз і прокрутили більше 100px від верху: ховаємо хедер
        if (scrolled > 100 && scrolled > scrollPrev) {
            header.classList.add('header-out');
        }
        // Скролимо вгору: показуємо хедер
        else if (scrolled < scrollPrev - 5) {
            header.classList.remove('header-out');
        }

        scrollPrev = scrolled;
    });
} else {
    console.error("Елемент .header не знайдено.");
}
// -------------------------------------------------------------

// Функція для визначення, чи є елемент у видимій області
const observer = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
        if (entry.isIntersecting) {
            // Додаємо клас 'show', щоб спрацювала CSS-анімація
            entry.target.classList.add('show');
            // Припиняємо спостереження, щоб анімація не повторювалася
            observer.unobserve(entry.target);
        }
    });
}, {
    // Елемент вважається видимим, коли 20% його висоти знаходиться у вьюпорті
    threshold: 0.2
});

// -----------------------------------
// 1. Анімація при запуску сторінки (Start Banner)
// -----------------------------------
const startBanner = document.querySelector('.start-banner');

// Запускаємо анімацію одразу, як тільки DOM завантажився
if (startBanner) {
    // Використовуємо setTimeout, щоб дати CSS застосуватися
    setTimeout(() => {
        startBanner.classList.add('show');
    }, 100);
}

// Показати банер як fade-in після завантаження
window.addEventListener('DOMContentLoaded', () => {
    const banner = document.querySelector('.banner-container');
    if (banner) {
        // невелика затримка для плавності
        setTimeout(() => banner.classList.add('in'), 80);
    }
});

// Показ нижніх елементів при трохи більшим скроллі (поріг +200px)
function revealContentOnScroll() {
    const el = document.querySelector('.content-part');
    if (!el) return;
    const rect = el.getBoundingClientRect();
    if (rect.top < window.innerHeight - 200) { // більший поріг для пізнішого показу
        el.classList.add('in');
        window.removeEventListener('scroll', revealContentOnScroll);
    }
}
window.addEventListener('scroll', revealContentOnScroll);
window.addEventListener('DOMContentLoaded', revealContentOnScroll);

// -----------------------------------
// 2. Scroll Reveal для основних секцій
// -----------------------------------

// ourlife-area (вилітає знизу) та news-area (вилітає справа)
const ourLifeAndNews = [
    document.querySelector('.ourlife-area'),
    document.querySelector('.news-area')
].filter(el => el); // Фільтруємо null, якщо елемент не знайдено

ourLifeAndNews.forEach(el => {
    observer.observe(el);
});

// reviews-general-title (з'являється нізвідки)
const reviewsTitle = document.querySelector('.reviews-general-title');
if (reviewsTitle) {
    observer.observe(reviewsTitle);
}

// partners-area (з'являється нізвідки)
const partnersArea = document.querySelector('.partners-area');
if (partnersArea) {
    observer.observe(partnersArea);
}


// -----------------------------------
// 3. Послідовна анімація відгуків (pfp-name-status, rating-stars, comments)
// -----------------------------------

const reviewElements = [
    document.querySelector('.pfp-name-status'),
    document.querySelector('.rating-stars'),
    document.querySelector('.comments')
].filter(el => el);

// Створюємо окремий Observer для послідовної появи
const sequenceObserver = new IntersectionObserver((entries, obs) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            obs.unobserve(entry.target);

            // Запускаємо послідовну анімацію
            reviewElements.forEach((el, index) => {
                // Затримка: 0с, 0.5с, 1.0с
                const delay = index * 500;
                setTimeout(() => {
                    el.classList.add('show');
                }, delay);
            });
        }
    });
}, {
    threshold: 0.1
});

// Спостерігаємо за першим елементом в послідовності (pfp-name-status)
if (reviewElements.length > 0) {
    sequenceObserver.observe(reviewElements[0]);
}