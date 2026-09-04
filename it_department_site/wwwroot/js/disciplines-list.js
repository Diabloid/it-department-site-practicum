(function () {
    const PAGE_SIZE = 5;
    const MOBILE_MAX_WIDTH = 768;

    const grid = document.querySelector(".disciplines-grid");
    const pager = document.querySelector(".disciplines-pager");
    const searchForm = document.querySelector(".disciplines-search");
    const searchInput = document.getElementById("disciplines-search-input");
    const emptyState = document.querySelector(".disciplines-search-empty");
    if (!grid || !pager) return;

    const cards = Array.from(grid.querySelectorAll(".discipline-card"));
    let currentPage = 1;
    let query = "";
    let resizeTimer = 0;
    let searchTimer = 0;

    function isMobile() {
        return window.innerWidth <= MOBILE_MAX_WIDTH;
    }

    function cardName(card) {
        return (card.querySelector(".discipline-card-name")?.textContent || "").trim().toLowerCase();
    }

    function matchingCards() {
        if (!query) return cards;
        return cards.filter((card) => cardName(card).includes(query));
    }

    function applySearchVisibility(visible) {
        const visibleSet = new Set(visible);
        cards.forEach((card) => {
            const match = visibleSet.has(card);
            card.classList.toggle("is-search-hidden", !match);
            if (!match) card.classList.remove("is-pager-hidden");
        });
    }

    function hidePager() {
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

    function applyPage(visible) {
        const start = (currentPage - 1) * PAGE_SIZE;
        const end = start + PAGE_SIZE;
        visible.forEach((card, index) => {
            card.classList.toggle("is-pager-hidden", index < start || index >= end);
        });
        renderPager(Math.ceil(visible.length / PAGE_SIZE));
    }

    function showUnpaged(visible) {
        cards.forEach((card) => card.classList.remove("is-pager-hidden"));
        hidePager();
    }

    function goToPage(page) {
        currentPage = page;
        sync();
        grid.scrollIntoView({ behavior: "smooth", block: "start" });
    }

    function sync() {
        const visible = matchingCards();
        applySearchVisibility(visible);

        if (emptyState) {
            emptyState.hidden = visible.length > 0;
        }

        if (visible.length === 0) {
            hidePager();
            return;
        }

        if (!isMobile() || visible.length <= PAGE_SIZE) {
            currentPage = 1;
            showUnpaged(visible);
            return;
        }

        const total = Math.ceil(visible.length / PAGE_SIZE);
        if (currentPage > total) currentPage = total;
        if (currentPage < 1) currentPage = 1;
        applyPage(visible);
    }

    if (searchForm) {
        searchForm.addEventListener("submit", (event) => event.preventDefault());
    }

    if (searchInput) {
        searchInput.addEventListener("input", () => {
            clearTimeout(searchTimer);
            searchTimer = window.setTimeout(() => {
                query = searchInput.value.trim().toLowerCase();
                currentPage = 1;
                sync();
            }, 150);
        });
    }

    window.addEventListener("resize", () => {
        clearTimeout(resizeTimer);
        resizeTimer = window.setTimeout(sync, 150);
    });

    sync();
})();
