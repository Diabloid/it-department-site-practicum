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

document.querySelectorAll('.nav-toggle').forEach(burgerBtn => {
    burgerBtn.addEventListener('click', () => {
        burgerBtn.classList.toggle('is-open');
    })
})

document.querySelectorAll('.main-nav-item.group').forEach(menuItem => {
    menuItem.addEventListener('click', () => {
        const isNotOpen = !menuItem.classList.contains('is-open');
        document.querySelectorAll('.main-nav-item.group.is-open').forEach((d) => {
            d.classList.remove('is-open');
         });
        menuItem.classList.toggle('is-open', isNotOpen);
    })
})

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

    const slider = document.querySelector('.js-main-slider');
    if (!slider) return;

    const track = slider.querySelector('.banner-track');
    const slides = Array.from(slider.querySelectorAll('.banner-slide'));
    const dots = Array.from(slider.querySelectorAll('.banner-dot'));
    const prevBtn = slider.querySelector('.banner-arrow-prev');
    const nextBtn = slider.querySelector('.banner-arrow-next');

    if (!track || slides.length <= 1) return;

    let currentIndex = 0;
    const autoplayMs = parseInt(slider.getAttribute('data-autoplay-ms') || '5000', 10);
    let timerId = null;

    const updateUi = () => {
        track.style.transform = `translateX(-${currentIndex * 100}%)`;
        dots.forEach((dot, index) => {
            const active = index === currentIndex;
            dot.classList.toggle('is-active', active);
            dot.setAttribute('aria-selected', active ? 'true' : 'false');
        });
    };

    const goTo = (index) => {
        const maxIndex = slides.length - 1;
        if (index < 0) currentIndex = maxIndex;
        else if (index > maxIndex) currentIndex = 0;
        else currentIndex = index;
        updateUi();
    };

    const startAutoplay = () => {
        if (timerId) clearInterval(timerId);
        timerId = setInterval(() => goTo(currentIndex + 1), autoplayMs);
    };

    prevBtn?.addEventListener('click', () => {
        goTo(currentIndex - 1);
        startAutoplay();
    });

    nextBtn?.addEventListener('click', () => {
        goTo(currentIndex + 1);
        startAutoplay();
    });

    dots.forEach((dot, index) => {
        dot.addEventListener('click', () => {
            goTo(index);
            startAutoplay();
        });
    });

    slider.addEventListener('mouseenter', () => timerId && clearInterval(timerId));
    slider.addEventListener('mouseleave', startAutoplay);

    updateUi();
    startAutoplay();
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
const partnersAreas = document.querySelectorAll('.partners-area');
partnersAreas.forEach((partnersArea) => {
    observer.observe(partnersArea);
});


//----------------------------------------------
// 3. слайдер для відгуків
//----------------------------------------------
let slideIndex = 1;

window.currentSlide = function(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    let slides = document.getElementsByClassName("reviews-field");
    let dots = document.getElementsByClassName("dot");

    if (slides.length === 0) {
        return;
    }

    if (n > slides.length) slideIndex = 1;
    if (n < 1) slideIndex = slides.length;

    for (let i = 0; i < slides.length; i++) {
        slides[i].classList.remove("active");
        slides[i].style.display = "none";
    }
    for (let i = 0; i < dots.length; i++) {
        dots[i].classList.remove("active");
    }

    slides[slideIndex - 1].classList.add("active");
    slides[slideIndex - 1].style.display = "flex";
    if (dots[slideIndex - 1]) {
        dots[slideIndex - 1].classList.add("active");
    }
}

document.addEventListener("DOMContentLoaded", function() {
    const slides = document.getElementsByClassName("reviews-field");
    if (slides.length === 0) {
        return;
    }

    showSlides(slideIndex);

    setInterval(() => {
        slideIndex++;
        showSlides(slideIndex);
    }, 5000);
});