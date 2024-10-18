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
                        $(that).closest('div').find('.like-count').addClass('text-danger').removeClass('text-secondary');
                    } else {
                        icon.removeClass('text-danger').addClass('text-secondary');
                        $(that).closest('div').find('.like-count').removeClass('text-danger').addClass('text-secondary');
                    }
                }         

                var likeCountElement = $(that).closest('div').find('.like-count');
                
                if (likeCountElement.length > 0 && response.likeCount > 0) {
                    likeCountElement.addClass("visible");
                    likeCountElement.text(response.likeCount);
                }
                else
                {
                    likeCountElement.removeClass("visible");
                    likeCountElement.text("");
                }
            },
            error: function() {
                alert('Error. like');
            }
        });
    });
});
$(document).ready(function() {
    $('.retweet').on('click', function(e) {
        e.preventDefault();
        var tweetId = $(this).data('tweet-id');
        var isRetweeted = $(this).find('i').hasClass('text-success');
        var retweetUrl = $(this).data('retweet-url');
        var unretweetUrl = $(this).data('unretweet-url');
        var url = isRetweeted ? unretweetUrl : retweetUrl;
        var that = this;

        $.ajax({
            url: url,
            type: 'POST',
            data: { tweetId: tweetId },
            success: function(response) {
                if (response.success) {
                    var icon = $(that).find('i');

                    if (icon.hasClass('text-success')) {
                        icon.removeClass('text-success').addClass('text-secondary');
                        $(that).closest('div').find('.retweet-count').removeClass('text-success').addClass('text-secondary');
                    } else {
                        icon.removeClass('text-secondary').addClass('text-success');
                        $(that).closest('div').find('.retweet-count').removeClass('text-secondary').addClass('text-success');
                    }
                }

                var retweetCountElement = $(that).closest('div').find('.retweet-count');

                if (retweetCountElement.length > 0 && response.retweetCount > 0) {
                    retweetCountElement.addClass("visible");
                    retweetCountElement.text(response.retweetCount);
                } else {
                    retweetCountElement.removeClass("visible");
                    retweetCountElement.text("");
                }
            },
            error: function() {
                alert('Error. retweet');
            }
        });
    });
});
