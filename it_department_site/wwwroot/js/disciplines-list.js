(function () {
    const form = document.querySelector(".disciplines-search");
    const input = document.getElementById("disciplines-search-input");
    if (!form || !input) return;

    const DEBOUNCE_MS = 300;
    const MAX_QUERY_LENGTH = 50;
    let debounceTimer = null;

    function currentQuery() {
        const value = input.value.slice(0, MAX_QUERY_LENGTH);
        if (value !== input.value) {
            input.value = value;
        }
        return value.trim();
    }

    function applySearch() {
        const url = new URL(window.location.href);
        const query = currentQuery();
        const currentName = (url.searchParams.get("name") ?? "").trim();
        if (query === currentName) return;

        if (query) {
            url.searchParams.set("name", query);
        } else {
            url.searchParams.delete("name");
        }
        url.searchParams.delete("pageNumber");
        window.location.assign(url.toString());
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
})();
