document.querySelectorAll('.tweet-button-group .btn').forEach(button => {
    button.addEventListener('click', function() {
        document.querySelector('.tweet-button-group .btn.active')?.classList.remove('active');
        this.classList.add('active');
    });
});