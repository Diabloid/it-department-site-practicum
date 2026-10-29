document.addEventListener('DOMContentLoaded', () => {
    const tabButtons = document.querySelectorAll('.tab-btn');
    const tabPanels = document.querySelectorAll('.tab-panel');

    tabButtons.forEach(button => {
        button.addEventListener('click', () => {
            // 1. Remove active state from all buttons and hide all panels
            tabButtons.forEach(btn => {
                btn.classList.remove('active');
                btn.setAttribute('aria-selected', 'false');
            });
            tabPanels.forEach(panel => panel.classList.remove('active'));

            // 2. Add active state to clicked button
            button.classList.add('active');
            button.setAttribute('aria-selected', 'true');

            // 3. Reveal the corresponding target panel
            const targetId = button.getAttribute('data-tab');
            document.getElementById(targetId).classList.add('active');
        });
    });
});