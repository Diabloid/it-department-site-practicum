(function () {
    const PAGE_SIZE = 5;
    const MOBILE_MAX_WIDTH = 768;

    const grid = document.querySelector(".disciplines-grid");
    const pager = document.querySelector(".disciplines-pager");
    if (!grid || !pager) return;

    const cards = Array.from(grid.querySelectorAll(".discipline-card"));
    let currentPage = 1;
    let resizeTimer = 0;

    function isMobile() {
        return window.innerWidth <= MOBILE_MAX_WIDTH;
    }

    function pageCount() {
        return Math.ceil(cards.length / PAGE_SIZE);
    }

    function showAll() {
        cards.forEach((card) => card.classList.remove("is-pager-hidden"));
        pager.hidden = true;
        pager.replaceChildren();
    }

    function renderPager(totalPages) {
        pager.replaceChildren();
        for (let i = 1; i <= totalPages; i++) {
            const button = document.createElement("button");
            button.type = "button";
            button.className = "disciplines-pager-btn" + (i === currentPage ? " is-active" : "");
            button.textContent = String(i);
            button.setAttribute("aria-label", "Сторінка " + i);
            if (i === currentPage) button.setAttribute("aria-current", "page");
            button.addEventListener("click", () => goToPage(i));
            pager.appendChild(button);
        }
        pager.hidden = false;
    }

    function applyPage() {
        const start = (currentPage - 1) * PAGE_SIZE;
        const end = start + PAGE_SIZE;
        cards.forEach((card, index) => {
            card.classList.toggle("is-pager-hidden", index < start || index >= end);
        });
        renderPager(pageCount());
    }

    function goToPage(page) {
        currentPage = page;
        applyPage();
        grid.scrollIntoView({ behavior: "smooth", block: "start" });
    }

    function sync() {
        if (!isMobile() || cards.length <= PAGE_SIZE) {
            currentPage = 1;
            showAll();
            return;
        }

        const total = pageCount();
        if (currentPage > total) currentPage = total;
        applyPage();
    }

    window.addEventListener("resize", () => {
        clearTimeout(resizeTimer);
        resizeTimer = window.setTimeout(sync, 150);
    });

    sync();
})();
