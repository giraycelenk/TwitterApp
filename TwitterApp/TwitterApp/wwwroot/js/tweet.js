function updateCharCount(textarea) {
    const maxLength = 280;
    const currentLength = textarea.value.length;
    const remaining = maxLength - currentLength;
    document.getElementById('charCount').textContent = remaining + ' characters';
}
$(document).ready(function() {
    $('.like').on('click', function(e) {
        e.preventDefault();
        var tweetId = $(this).data('tweet-id');
        var isLiked = $(this).find('i').hasClass('fas');
        var likeUrl = $(this).data('like-url');
        var unlikeUrl = $(this).data('unlike-url');
        var url = isLiked ? unlikeUrl : likeUrl;
        var that = this;

        $.ajax({
            url: url,
            type: 'POST',
            data: { tweetId: tweetId },
            success: function(response) {
                if (response.success) {
                    var icon = $(that).find('i');
                    icon.toggleClass('fas far'); 

                    if (icon.hasClass('fas')) {
                        icon.addClass('text-danger').removeClass('text-secondary');
                        $(that).find('.like-count').addClass('text-danger').removeClass('text-secondary');
                    } else {
                        icon.removeClass('text-danger').addClass('text-secondary');
                        $(that).find('.like-count').removeClass('text-danger').addClass('text-secondary');
                    }
                }
                var likeCountElement = $(that).find('.like-count');
                if (likeCountElement.length > 0 && response.likeCount >= 0) {
                    likeCountElement.text(response.likeCount);
                }
            },
            error: function() {
                alert('Error.');
            }
        });
    });
});
