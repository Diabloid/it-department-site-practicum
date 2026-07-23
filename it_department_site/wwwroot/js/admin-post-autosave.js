(function () {
    const DEBOUNCE_MS = 28000;
    let autosaveTimer = null;
    let autosaveInFlight = false;

    function getPostForm() {
        return document.querySelector('#contentArea [data-post-form]');
    }

    function setStatus(text) {
        const el = document.getElementById('postAutosaveStatus');
        if (el) el.textContent = text || '';
    }

    function formatSavedTime(date) {
        const d = date instanceof Date ? date : new Date(date);
        return d.toLocaleTimeString('uk-UA', { hour: '2-digit', minute: '2-digit' });
    }

    function buildDraftFormData(form) {
        if (typeof syncRichTextEditors === 'function') {
            syncRichTextEditors(form);
        }
        const fd = new FormData(form);
        fd.set('Status', 'Draft');
        const fileInput = form.querySelector('input[name="thumbnailFile"]');
        if (fileInput) fd.delete('thumbnailFile');
        return fd;
    }

    function applyDraftResponse(data) {
        const idInput = document.getElementById('PostId');
        const statusInput = document.getElementById('PostStatus');
        if (idInput && data.id) idInput.value = data.id;
        if (statusInput) statusInput.value = 'Draft';
        if (data.savedAt) {
            setStatus('Чернетка збережена о ' + formatSavedTime(data.savedAt));
        }
    }

    async function runAutosave(force) {
        const form = getPostForm();
        if (!form) return;

        const title = (form.querySelector('[name="Title"]')?.value || '').trim();
        const slug = (form.querySelector('[name="Slug"]')?.value || '').trim();
        const contentField = form.querySelector('[name="Content"]');
        let content = contentField?.value || '';
        if (contentField?.ckeditorInstance) {
            content = contentField.ckeditorInstance.getData();
        }
        const excerptField = form.querySelector('[name="Excerpt"]');
        let excerpt = excerptField?.value || '';
        if (excerptField?.ckeditorInstance) {
            excerpt = excerptField.ckeditorInstance.getData();
        }

        const hasText = title.length > 0 || slug.length > 0 ||
            (content && content.replace(/<[^>]*>/g, '').trim().length > 0) ||
            (excerpt && excerpt.replace(/<[^>]*>/g, '').trim().length > 0);
        const postId = parseInt(form.querySelector('[name="Id"]')?.value || '0', 10);

        if (!force && !hasText && postId === 0) return;
        if (autosaveInFlight) return;

        autosaveInFlight = true;
        if (force) setStatus('Зберігаємо…');

        try {
            const fd = buildDraftFormData(form);
            const res = await fetch('/Admin/SavePostDraft', {
                method: 'POST',
                body: fd,
                credentials: 'same-origin',
                keepalive: true
            });
            if (!res.ok) throw new Error('autosave failed');
            const data = await res.json();
            applyDraftResponse(data);
        } catch (e) {
            console.error('Post autosave error:', e);
            if (force) setStatus('Не вдалося зберегти чернетку');
        } finally {
            autosaveInFlight = false;
        }
    }

    function scheduleAutosave() {
        clearTimeout(autosaveTimer);
        autosaveTimer = setTimeout(() => runAutosave(false), DEBOUNCE_MS);
    }

    function beaconAutosave() {
        const form = getPostForm();
        if (!form) return;

        if (typeof syncRichTextEditors === 'function') {
            syncRichTextEditors(form);
        }

        const title = (form.querySelector('[name="Title"]')?.value || '').trim();
        const postId = parseInt(form.querySelector('[name="Id"]')?.value || '0', 10);
        if (!title && postId === 0) return;

        const fd = buildDraftFormData(form);
        if (navigator.sendBeacon) {
            navigator.sendBeacon('/Admin/SavePostDraft', fd);
        } else {
            fetch('/Admin/SavePostDraft', {
                method: 'POST',
                body: fd,
                credentials: 'same-origin',
                keepalive: true
            }).catch(() => {});
        }
    }

    window.savePostDraft = async function () {
        await runAutosave(true);
    };

    window.publishPost = function () {
        const form = getPostForm();
        if (!form) return;

        if (typeof syncRichTextEditors === 'function') {
            syncRichTextEditors(form);
        }

        const statusInput = document.getElementById('PostStatus');
        if (statusInput) statusInput.value = 'Published';

        const fd = new FormData(form);
        fd.set('Status', 'Published');

        setStatus('Публікуємо…');

        fetch('/Admin/PublishPost', {
            method: 'POST',
            body: fd,
            credentials: 'same-origin'
        })
            .then(r => r.text())
            .then(html => {
                document.getElementById('contentArea').innerHTML = html;
                if (typeof initRichTextEditors === 'function') initRichTextEditors();
                if (typeof initPostAutosave === 'function') initPostAutosave();
            })
            .catch(err => {
                console.error('Publish error:', err);
                setStatus('Помилка публікації');
            });
    };

    window.initPostAutosave = function () {
        const form = getPostForm();
        if (!form || form.dataset.autosaveReady === '1') return;
        form.dataset.autosaveReady = '1';

        form.querySelectorAll('input, textarea, select').forEach(el => {
            el.addEventListener('input', scheduleAutosave);
            el.addEventListener('change', scheduleAutosave);
        });

        document.addEventListener('visibilitychange', () => {
            if (document.visibilityState === 'hidden') beaconAutosave();
        });
        window.addEventListener('pagehide', beaconAutosave);

        const hookEditors = () => {
            form.querySelectorAll('textarea.editor').forEach(textarea => {
                if (textarea.ckeditorInstance && textarea.dataset.autosaveCke !== '1') {
                    textarea.dataset.autosaveCke = '1';
                    textarea.ckeditorInstance.model.document.on('change:data', scheduleAutosave);
                }
            });
        };
        hookEditors();
        let attempts = 0;
        const editorPoll = setInterval(() => {
            hookEditors();
            if (++attempts > 20) clearInterval(editorPoll);
        }, 500);
    };
})();
