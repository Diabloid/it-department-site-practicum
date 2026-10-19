(function () {
    const form = document.querySelector(".disciplines-search");
    const input = document.getElementById("disciplines-search-input");
    const container = document.getElementById("disciplines-results");
    if (!form || !input || !container) return;

    const DEBOUNCE_MS = 500;
    const MAX_NAME_QUERY_LENGTH = 50;
    let debounceTimer = null;
    let activeRequest = null;
    let lastQuery = currentQuery();

    function currentQuery() {
        const value = input.value.slice(0, MAX_NAME_QUERY_LENGTH);
        if (value !== input.value) {
            input.value = value;
        }
        return value.trim();
    }

    function buildParams(pageNumber) {
        const params = new URLSearchParams();
        params.set("required", form.elements["required"].value);
        const query = currentQuery();
        if (query) {
            params.set("name", query);
        }
        if (pageNumber > 1) {
            params.set("pageNumber", String(pageNumber));
        }
        return params;
    }

    function loadPage(pageNumber) {
        const params = buildParams(pageNumber);

        // Cancel the previous request so a slow response can't overwrite a newer one.
        activeRequest?.abort();
        activeRequest = new AbortController();

        fetch(`/disciplines/paged?${params}`, { signal: activeRequest.signal })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Помилка мережі або сервера");
                }
                return response.text();
            })
            .then(html => {
                container.innerHTML = html;
                // Keep the address bar in sync so reload / sharing the link shows the same page.
                history.replaceState(null, "", `/disciplines?${params}`);
            })
            .catch(error => {
                if (error.name === "AbortError") return;
                console.error("Помилка:", error);
                container.innerHTML = '<div class="alert alert-danger">Не вдалося завантажити дані.</div>';
            });
    }

    function applySearch() {
        const query = currentQuery();
        if (query === lastQuery) return;
        lastQuery = query;
        loadPage(1);
    }

    function scheduleSearch() {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(applySearch, DEBOUNCE_MS);
    }

    input.addEventListener("input", scheduleSearch);

    form.addEventListener("submit", (event) => {
        event.preventDefault();
        clearTimeout(debounceTimer);
        applySearch();
    });

    // The pager is re-rendered with every response, so listen on the container.
    container.addEventListener("click", (event) => {
        const link = event.target.closest(".disciplines-pager a[data-page]");
        if (!link) return;
        event.preventDefault();
        loadPage(Number(link.dataset.page));
        container.scrollIntoView({ behavior: "smooth", block: "start" });
    });
})();
