$(document).ready(function () {
    $('form').on('submit', function (event) {
        event.preventDefault(); 

        var form = $(this);
        var actionUrl = form.attr('action'); 

        $.ajax({
            type: 'POST',
            url: actionUrl, 
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    
                    let followButton = form.find('.btn-follow-unfollow-custom');
                    let hiddenInput = form.find('input[type="hidden"]'); 

                    if (response.isFollowing) {
                        
                        followButton.text('Unfollow');
                        form.attr('action', '/Users/Unfollow'); 
                        hiddenInput.attr('name', 'userIdToUnfollow'); 

                        
                        let followersCount = parseInt($('.followers-count').text());
                        $('.followers-count').text(followersCount + 1 + " Followers");
                    } else {
                        
                        followButton.text('Follow');
                        form.attr('action', '/Users/Follow'); 
                        hiddenInput.attr('name', 'userIdToFollow'); 

                        
                        let followersCount = parseInt($('.followers-count').text());
                        $('.followers-count').text(followersCount - 1 + " Followers");
                    }
                } else {
                    
                    alert(response.message);
                }
            },
            error: function () {
                alert('An error occurred. Please try again.');
            }
        });
    });
});
