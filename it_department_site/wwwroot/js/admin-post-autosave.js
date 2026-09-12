(function () {
    let isSaving = false;

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

    async function saveDraft() {
        const form = getPostForm();
        if (!form) return;

        if (isSaving) return;
        isSaving = true;
        setStatus('Зберігаємо…');

        try {
            const fd = buildDraftFormData(form);
            const res = await fetch('/Admin/SavePostDraft', {
                method: 'POST',
                body: fd,
                credentials: 'same-origin'
            });
            if (!res.ok) throw new Error('save failed');
            const data = await res.json();
            applyDraftResponse(data);
        } catch (e) {
            console.error('Post save error:', e);
            setStatus('Не вдалося зберегти чернетку');
        } finally {
            isSaving = false;
        }
    }

    // Ручне збереження чернетки (по кнопці)
    window.savePostDraft = async function () {
        await saveDraft();
    };

    // Ручна публікація (по кнопці)
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
                if (typeof window.initPostAutosave === 'function') window.initPostAutosave();
            })
            .catch(err => {
                console.error('Publish error:', err);
                setStatus('Помилка публікації');
            });
    };

    // Залишив функцію порожньою, щоб HTML-в'юшки не видавали помилку "is not a function" при завантаженні
    window.initPostAutosave = function () {
    };
})();