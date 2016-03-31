$(function () {
    $('#submitProject').click(function (event) {
        event.preventDefault();
        submitWithFBCheck();
    });

    $('.facebookFeed').each(function (i, obj) {

        //var url = "http://graph.facebook.com/100004888600439_241616282635151/comments?limit=3&callback=?";
        var url = "http://graph.facebook.com/" + obj.id + "/comments?limit=3&callback=?";
        $.getJSON(url, function (json) {
            var html = "<ul>";
            $.each(json.data, function (i, fb) {
                html += "<li>" + $('<div>').html(fb.message).text() + "</li>";
            });
            html += "</ul>";

            $(obj).animate({ opacity: 0 }, 500, function () {
                $(obj).html(html);
            });
            $(obj).animate({ opacity: 1 }, 500);

        });
    });

});

// Additional JS functions here
window.fbAsyncInit = function () {
    FB.init({
        appId: '433429130044894', // App ID
        channelUrl: 'http://localhost:54359/channel.html', // Channel File
        status: true, // check login status
        cookie: true, // enable cookies to allow the server to access the session
        xfbml: true  // parse XFBML
    });

    //        // Additional init code here
    //        FB.getLoginStatus(function (response) {
    //            if (response.status === 'connected') {
    //                // connected...
    //                // Will be true whenever the User viewing the page is both logged into Facebook 
    //                // and has already previously authorized the current app.
    //                alert("already connected");
    //            } else if (response.status === 'not_authorized') {
    //                // not_authorized...
    //                // Will be true whenever the User viewing the page is logged into Facebook, but has not yet authorized the current app.
    //                alert("not authorised");
    //                login();
    //            } else {
    //                // not_logged_in...
    //                // Will be true when the User viewing the page is not logged into Facebook, 
    //                // and therefore the state of their authorization of the app is unknown
    //                alert("not logged in");
    //                login();
    //            }
    //        });
};

// Load the SDK Asynchronously
(function (d) {
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
} (document));

function submitWithFBCheck() {
    // Does the user want us to post to their wall?
    if ($('#Share').attr('checked') == true) {

        try {
            FB.getLoginStatus(function (response) {
                if (response.status === 'connected') {
                    // connected...
                    // Will be true whenever the User viewing the page is both logged into Facebook 
                    // and has already previously authorized the current app.
                    alert("already connected");
                    FB.api('/me', function (response) {
                        // console.log('Good to see you, ' + response.name + '.');
                        // first_name	"Crochet"	                                    String
                        // gender	    "female"	                                    String
                        // id	        "100004888600439"	                            String
                        // last_name	"Pattern"	                                    String
                        // link	        "http://www.facebook.com/crochet.pattern.75"	String
                        // locale	    "en_GB"	                                        String
                        // name	        "Crochet Pattern"	                            String
                        // timezone	    0	                                            Number
                        // updated_time	"2012-12-18T11:56:39+0000"	                    String
                        // username	    "crochet.pattern.75"	                        String
                        $('#FBId').val(response.id);
                        $('form#myCompletedProject').submit();
                    });
                } else {
                    if (response.status == 'not_authorized') {
                        // not_authorized...
                        // Will be true whenever the User viewing the page is logged into Facebook, but has not yet authorized the current app.
                        alert("not authorised");
                    } else {
                        // not_logged_in...
                        // Will be true when the User viewing the page is not logged into Facebook, 
                        // and therefore the state of their authorization of the app is unknown
                        alert("not logged in");
                    }
                    login();
                }
            });
        }
        catch (err) {
            alert('Error: ' + err);
        }

    }
}

function login() {
    FB.login(function (response) {
        if (response.authResponse) {

            //response.authResponse.accessToken	    "AAAGKM5VU5d4BAKcyXkFbpajs5zku6fT1vUheHXFbc4rFnQsazZA6UTnnCSqNENDSqzRuMOn0K4wTa0GZCt2imwykHNh7Fyp4l1tZCEjxHjeHGVvMhwQ"	String
            //response.authResponse.expiresIn		6024	Number
            //response.authResponse.signedRequest	"2HKkEcM2lrm3leBE41X3ftThW9ZA085RJfPlLviUSA0.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImNvZGUiOiJBUUN5VTI3ckhlWi1YVmhXZml1MWlBd2FiQW5fVXJ0X0lOT2YwRU9FdUg3UGwwRWJNazVfcFkwTUpVUGhaN1lsRk9ZLVFJQkM4aF80RlI3YUw5aTdYbVdYS09tU0ZhQjA5Mk05Z1BhczFjdnlQM0NTeWQ0a0U5dXpBMnhDYll0NE9fUlcwRkxTRVYxcC01M2x5SmhYb0Y3dFlPaUZ2dTJzZGUyM3dTS1BPZVpRRjFnb3d2TV9NenAyMXNaMkMzSHRvRDVRVWRRN3BJRG5qY1o1VF80bDd6ZmwiLCJpc3N1ZWRfYXQiOjEzNTczMDU1NzYsInVzZXJfaWQiOiIxMDAwMDQ4ODg2MDA0MzkifQ"	String
            //response.authResponse.userID		    "100004888600439"	String                

            // connected
            $('#FBId').val(response.authResponse.userID);

            // N.B. The response.authResponse.accessToken only lasts for 1-2 hours, or until the user logs out of facebook.
            //      You need to exchange the short lived access token for a long lived token, which will last for about 60 days.
            //      I have moved that particular piece of functionality to the server side controller.
            $('#FBShortLivedToken').val(response.authResponse.accessToken);
            //$('#FBLongLivedToken').val('Hawkman');

            $('form#myCompletedProject').submit();
        } else {
            $('form#myCompletedProject').submit();
        }
    }, { scope: 'publish_stream' });        // Request oAuth to publish.
}

function afterFacebookConnect() {
    FB.getLoginStatus(function (response) {
        if (response.session) {
            window.location = "../Account/FacebookLogin?token=" + response.session.access_token;
        } else {
            // user clicked Cancel
        }
    });
};

function testAPI() {
    // console.log('Welcome!  Fetching your information.... ');
    FB.api('/me', function (response) {
        // console.log('Good to see you, ' + response.name + '.');
        // first_name	"Crochet"	                                    String
        // gender	    "female"	                                    String
        // id	        "100004888600439"	                            String
        // last_name	"Pattern"	                                    String
        // link	        "http://www.facebook.com/crochet.pattern.75"	String
        // locale	    "en_GB"	                                        String
        // name	        "Crochet Pattern"	                            String
        // timezone	    0	                                            Number
        // updated_time	"2012-12-18T11:56:39+0000"	                    String
        // username	    "crochet.pattern.75"	                        String
    });
}





