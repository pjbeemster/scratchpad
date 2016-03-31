/*
*
* Tracking
*
* Helper functions and event handlers namespaced within '_tracking'.
* Each event is pushed into the '_tracking.handlers' array to maintain a reference to them.
* Likely not needed but could prove useful.
*
* '_tracking.debug' flag can be set to true or false to log or actually fire to GA respectively.
*
* Requires: jQuery.
*
* Note for Youtube: http://www.analyticsresults.com/2013/03/youtube-video-tracking-into-google.html
*
*/


var _tracking = {
    debug : false,
    handlers : []
};


// Wrapper Functions
_tracking.trackpageview = function (url) {
    
    //add appPath to virtual Urls
    if (url.indexOf('virtual') == 0) {
        url = appPath + "/" + url;
    }
    else if (url.indexOf('/virtual') == 0) {
        url = appPath + url;
    }

    if (!this.debug) {
        _gaq.push(['_trackPageview', url]);
        return;
    } else {
        // console.debug('_trackPageview', url);
        return;
    }
};

_tracking.setcustomvar = function (index, name, value, scope) {

    if (!this.debug) {
        _gaq.push(['_setCustomVar', index, name, value, scope]);
        return;
    }
    else {
        // console.debug('_setCustomVar', index, name, value, scope);
        return;
    }
};

_tracking.tracksocial = function (trackingObj, eventObj) {

    if (!this.debug) {
        _gaq.push(['_trackSocial', trackingObj.network, trackingObj.action, trackingObj.target, trackingObj.page]);
        return;
    } else {
        // console.debug('_trackSocial', trackingObj, eventObj);
        return;
    }
};

_tracking.trackevent = function (trackingObj, eventObj) {

    if (!this.debug) {
        _gaq.push(['_trackEvent', trackingObj.category, trackingObj.action, trackingObj.label, trackingObj.value, trackingObj.nonint]);
        return;
    } else {
        // console.debug('_trackEvent', trackingObj, eventObj);
        return;
    }
};


// Begin defining tracking events

// Custom vars for user state
_tracking.handlers.push(function () {

    $(document).on('ready', function (event) {

        var i = 1,
            n = 'User Type',
            v = '',
            s = 2;

        if ($('body').hasClass('visitor')) {
            v = 'Visitor';
        }

        if ($('body').hasClass('member')) {
            v = 'Member';
        }

        _tracking.setcustomvar(i, n, v, s);
    });
});


// Login/Registration Form Tracking
// - Track login form submits
_tracking.handlers.push(function () {

    $(document).on('submit', '#login', function (e) {

        _tracking.trackpageview('virtual/login');
    });
});


// - Track registration form submits
_tracking.handlers.push(function () {

    $(document).on('submit', '#registration', function (e) {

        if ($(this).find('input[name="Stage"]').val() == 'register') {
            _tracking.trackpageview('virtual/registration');
        }
    });
});


// - Track number of newsletter signups
_tracking.handlers.push(function () {

    $('body').on('click', '#email-newsletter-Yes', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Email-Signup',
            action: "click",
            label: '',
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
    
    //Removed for CCPLCR-170
//    $('form.regsign').on('submit', function (event) {

//        // Set up tracking object
//        var trackingObject = {
//            category: 'Email-Signup',
//            action: "click",
//            label: '',
//            value: null,
//            nonint: false
//        };

//        // Set up event object
//        var eventObj = {
//            type: event.type,
//            href: event.target.href,
//            allowRedirect: true
//        };

//        _tracking.trackevent(trackingObject, eventObj);
//    });
});


// Navigation tracking
// - Header
_tracking.handlers.push(function () {

    $('#head a, #navbar a').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Navigation',
            action: 'Top',
            label: $(event.target).text(),
            value: null,
            nonint: true
        }

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        }

        if ($(event.target).hasClass('ajax')) {
            eventObj.allowRedirect = false;
        }

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// - Main body
_tracking.handlers.push(function () {

    $('#main a').on('click', function (event) {

        var $target = $(event.target);

        if ($target.hasClass('js-no-nav-tracking') || $target.parents('div[class=addthis_toolbox]').length > 0)
            return;

        var label = $(event.target).text();

        if (label == "") {
            label = $(this).attr('title');
        }

        // Set up tracking object
        var trackingObject = {
            category: 'Navigation',
            action: 'Middle',
            label: label,
            value: null,
            nonint: true
        }

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        }

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// - Footer
_tracking.handlers.push(function () {

    $('#base a').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Navigation',
            action: 'Footer',
            label: $(event.target).text(),
            value: null,
            nonint: true
        }

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        }

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// Comments section
// - Add rating tracking
_tracking.handlers.push(function () {

    $('form[action*="AddRating"]').on('submit', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Add Rating',
            label: $(this).find('#hdnItemId').val(),
            value: $(this).find('#ratingOption').val(),
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.tracksocial(trackingObject, eventObj);
    });
});


// Download instructions
_tracking.handlers.push(function () {

    $('.download').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Sidebar Interaction',
            action: 'Download',
            label: $(this).parents('section').find('h1').text(),
            value: $(this).text(),
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// Track ajax load mores
_tracking.handlers.push(function () {

    $('.ruled a').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'View All',
            label: window.location.href,
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// Track ajax load mores
_tracking.handlers.push(function () {

    $('.show-more .load-more').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Teaser',
            label: 'Load More Products',
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// Read more links
_tracking.handlers.push(function () {

    $('.read-full').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Read Article',
            label: $(event.target).parents('section').find('h1').text(),
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// Carousel interactions
_tracking.handlers.push(function () {

    $(document).on('click', '#home-carousel .carousel-control', function (event) {
        if (event.originalEvent !== undefined) {

            // Set up tracking object
            var trackingObject = {
                category: 'Additional Content',
                action: 'Carousel',
                label: '',
                value: null,
                nonint: false
            };

            if ($(this).hasClass('right')) {
                trackingObject.label = 'Next';
            }
            else {
                trackingObject.label = 'Previous';
            }

            // Set up event object
            var eventObj = {
                type: event.type,
                href: event.target.href,
                allowRedirect: true
            };

            _tracking.trackevent(trackingObject, eventObj);
        }
    });
});


// Track filter clicks
_tracking.handlers.push(function () {

    $(document).on('click', '.full-width-carousel .carousel-control', function (event) {

        if (event.originalEvent !== undefined) {

            // Set up tracking object
            var trackingObject = {
                category: 'Additional Content',
                action: 'Carousel',
                label: '',
                value: null,
                nonint: false
            };

            if ($(this).hasClass('right')) {
                trackingObject.label = 'Next';
            }
            else {
                trackingObject.label = 'Previous';
            }

            // Set up event object
            var eventObj = {
                type: event.type,
                href: event.target.href,
                allowRedirect: true
            };

            _tracking.trackevent(trackingObject, eventObj);
        }
    });
});


// Track content clicks
_tracking.handlers.push(function () {

    $('[class*="full-width-carousel"] .new-panel a').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Carousel',
            label: $(this).text(),
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// External link tracking
_tracking.handlers.push(function () {

    $('a[href^="http://"]').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'External',
            action: $(this).attr('href'),
            label: '',
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// Country selector
_tracking.handlers.push(function () {

    $('.country a').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Change Country',
            action: 'Select',
            label: $(event.target).text(),
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// Action/Icon tracking
// - Check if user is starting an action
_tracking.handlers.push(function () {

    $(document).on('ready', function (event) {

        var windowHref = window.location.href.toLowerCase();

        // Check we're on the registration page first of all
        if (windowHref.indexOf('registration') > 0) {

            if (windowHref.indexOf('action') > 0) {

                var intendedAction = windowHref.match(/action=(.*?)&/)[1], category;

                if (windowHref.indexOf('action') > 0) {

                    switch (intendedAction) {

                        case 'addtoscrapbook':
                            category = 'Add To Scrapbook';
                            label = windowHref.match(/type=(.*?)&/)[1].replace(/\+/g, ' ');
                            break;

                        case 'addtoshoppinglist':
                            category = 'Add To Shopping List';
                            label = windowHref.match(/name=(.*?)&/)[1].replace(/\+/g, ' ');
                            break;

                        case 'download':
                            category = 'Download';
                            label = windowHref.match(/project=(.*?)&/)[1].replace(/\+/g, ' '); ;
                            break;

                        default:
                            category = '';
                            label = '';
                    }

                    if (category != '' && label != '') {

                        // Set up tracking object
                        var trackingObject = {
                            category: category,
                            action: 'Pre Login',
                            label: label,
                            value: null,
                            nonint: true
                        };

                        // Set up event object
                        var eventObj = {};

                        _tracking.trackevent(trackingObject, eventObj);
                    }
                }
            }
        }
    });
});

// - Track shopping list adds by submission (Logged in)
_tracking.handlers.push(function () {

    $('form[action*="shoppinglist/additem"]').on('submit', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Form Interaction',
            action: 'Add To Shopping List',
            label: $(this).find('input[name="id"]').val(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);

        // Reset tracking object
        trackingObject = {
            category: 'Add To Shopping List',
            action: 'Pre Login',
            label: $(this).find('input[name="name"]'),
            value: null,
            nonint: true
        };

        _tracking.trackevent(trackingObject, eventObj);

        // Reset tracking object
        trackingObject = {
            category: 'Add To Shopping List',
            action: 'Post Login',
            label: $(this).find('input[name="name"]'),
            value: null,
            nonint: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Track shopping list adds by link click (Not logged in)
///// NOTE - IS THIS NEEDED ANY MORE AS WE TRACK ON REG PAGE LOAD
_tracking.handlers.push(function () {

    $('a.addtoshoppinglistbtn').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Link Interaction',
            action: 'Add To Shopping List (Intent)',
            label: $(this).closest('article').find('h1').text(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Track scrapbook add by submission (Logged in)
_tracking.handlers.push(function () {

    $('form[action*="scrapbook/additem"]').on('submit', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Form Interaction',
            action: 'Add To Scrapbook',
            label: $(this).find('input[name="description"]').val(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);

        // Reset tracking object
        trackingObject = {
            category: 'Add To Scrapbook',
            action: 'Pre Login',
            label: $(this).find('input[name="description"]').val(),
            value: null,
            nonint: true
        };

        _tracking.trackevent(trackingObject, eventObj);

        // Reset tracking object
        trackingObject = {
            category: 'Add To Scrapbook',
            action: 'Post Login',
            label: $(this).find('input[name="description"]').val(),
            value: null,
            nonint: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Track add to scrapbook clicks
_tracking.handlers.push(function () {

    $('.actions a.icon.scrapbook').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Icon Interaction',
            action: 'Add To Scrapbook (Intent)',
            label: $(this).closest('article').find('h1').text(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Track comment clicks
_tracking.handlers.push(function () {

    $('.actions a.icon.comments').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Icon Interaction',
            action: 'Add comment',
            label: $(this).closest('article').find('h1').text(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Track pinterest pins
_tracking.handlers.push(function () {

    $('.actions a.icon.pinterest').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Icon Interaction',
            action: 'Pin on Pinterest',
            label: $(this).closest('article').find('h1').text(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


/* Store locator Page -------------------------*/

// Track submissions of search form
_tracking.handlers.push(function () {

    $('.store-finder #store-locator-form').on('submit', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Store Locator',
            action: 'Click',
            label: '',
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// Add store locator outflow tracking event
_tracking.handlers.push(function () {

    $('.search-results table a.external').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Store Locator',
            action: $('#store-locator-form #Location').attr('value'),
            label: $(this).text(),
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// User panel interactions
// - Add External Resource tracking event
_tracking.handlers.push(function () {

    $('.user-panel form[action*="scrapbook/additem"]').on('submit', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Add External Source',
            label: $(this).find('input[name="source"]').val(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Print Shopping List tracking event
_tracking.handlers.push(function () {

    $('a[href*="shoppinglist/print.html"]').on('click', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Print Shopping List',
            label: '',
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Email Shopping List tracking event
_tracking.handlers.push(function () {

    $('#emailshoppinglist').on('submit', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Email Shopping List',
            label: '',
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});


// Faceted search filter tracking
// - Internal site search
_tracking.handlers.push(function () {

    $(document).on('submit', '.fh-filter', function (event) {

        var val,
            disc = $(this).find('#ComponentSection_SearchTerm').attr('value'), // Discover/Learn/Share Box
            prod = $(this).find('#ExtComponentSection_SearchTerm').attr('value');  // Products Box

        if (typeof disc != 'undefined') {
            val = disc;
        }

        if (typeof prod != 'undefined') {
            val = prod;
        }

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Internal Site Search',
            label: val,
            value: null,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Filter panel
_tracking.handlers.push(function() {

    $(document).on('click', '.filter-panel a', function (event) {

        var $label = $(this).text();
        var $action = $(this).parents('.facet').find('h2');

        if ($action.length == 0)
            $action = $(this).parents('.filter-panel').find('h1');

        $action = $action.first().text();

        // Set up tracking object
        var trackingObject = {
            category: 'Filter',
            action: $action,
            label: $label, 
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });    
});

// - Sort by filters
_tracking.handlers.push(function () {

    $(document).on('click', '.filters .sort a', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Filter',
            label: $(this).text(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Radio link filters
_tracking.handlers.push(function () {

    $(document).on('click', '.product-nav .control-group a', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Radio Button Filter',
            label: $(this).text(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// - Category filters
_tracking.handlers.push(function () {

    $(document).on('click', '.product-nav .category a', function (event) {

        // Set up tracking object
        var trackingObject = {
            category: 'Additional Content',
            action: 'Choose a Category Filter',
            label: $(this).text(),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

///////////////////////////
// ADD THIS INTERGRATION
///////////////////////////

if (typeof addthis !== "undefined") {
    addthis.addEventListener('addthis.menu.share', function (event) {

        if (!event.data)
            return;
        
        var trackingObject = {
            network: event.data.service,
            action: 'Share',
            target: '',
            page: window.location.href
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.tracksocial(trackingObject, eventObj);
    });
}

///////////////////////////
// DOWNLOAD PROJECT
///////////////////////////

_tracking.handlers.push(function () {

    $(document).on('click', '.download-project', function (event) {

        var $this = $(this);
        var $container = $this.parents('aside');

        var $brands = $('.js-facetitem-brands', $container);
        var $techniques = $('.js-facetitem-technique', $container);

//        if ($brands.length != 0 && $techniques.length != 0) {

//            var brands = $brands.text();
//            var techniques = $techniques.text();

//            if (brands.indexOf(':') >= 0) {
//                brands = $.trim(brands.split(':')[1]);
//            }

//            if (techniques.indexOf(':') >= 0) {
//                techniques = $.trim(techniques.split(':')[1]);
//            }

//            // Set up tracking object
//            var trackingObject = {
//                category: 'Download Project',
//                action: brands,
//                label: techniques,
//                value: null,
//                nonint: true
//            };

//            // Set up event object
//            var eventObj = {
//                type: event.type,
//                href: event.target.href,
//                allowRedirect: true
//            };

//            _tracking.trackevent(trackingObject, eventObj);
//        } else {

//            // Fire empty if brands and technique not set - HACKY!

//            // Set up tracking object
//            var trackingObject = {
//                category: 'Download Project',
//                action: '',
//                label: '',
//                value: null,
//                nonint: true
//            };

//            // Set up event object
//            var eventObj = {
//                type: event.type,
//                href: event.target.href,
//                allowRedirect: true
//            };

//            _tracking.trackevent(trackingObject, eventObj);
//        }

        // Rating stuff.

        var $title = $('.js-header-title');
        var $ratingOption = $('#ratingOption');

        if ($title.length == 0 || $ratingOption.length == 0 || isNaN(parseInt($ratingOption.val())) || !isFinite($ratingOption.val()))
            return;

        var rating = parseInt($ratingOption.val());
        // Set up tracking object
        var trackingObject = {
            category: 'Download Project',
            action: 'Click',
            label: $.trim($title.text()),
            value: rating,
            nonint: false
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

///////////////////////////
// DOWNLOAD BOOKING FORM
///////////////////////////

_tracking.handlers.push(function () {

    $('.booking-form').on('click', function(event) {
        
        var $this = $(this);
        var $container = $this.parents('aside');

        var $subject = $('.js-event-subject', $container);
        var $categories = $('.js-event-categories', $container);

        // Set up tracking object
        var trackingObject = {
            category: 'Download Booking Form',
            action: $.trim($categories.text()),
            label: $.trim($subject.text()),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

///////////////////////////
// DOWNLOAD VOUCHER
///////////////////////////

_tracking.handlers.push(function () {

    $('.js-promo-pod').each(function() {
        var $pod = $(this);
        var $link = $('.js-promo-link', $pod);
        var $title = $('.js-promo-title', $pod);

        $link.on('click', function(event) {
            // Set up tracking object
            var trackingObject = {
                category: 'Download Voucher',
                action: 'Click',
                label:  $.trim($title.text()),
                value: null,
                nonint: true
            };

            // Set up event object
            var eventObj = {
                type: event.type,
                href: event.target.href,
                allowRedirect: true
            };

            _tracking.trackevent(trackingObject, eventObj);
        });
    });   
});

///////////////////////////
// NEWSLETTER
///////////////////////////

_tracking.handlers.push(function () {

    $('.js-newsletter-form').on('submit', function (event) {

        var $form = $(this);
        var $checked = $('input:checkbox', $form);

        var signedupFor = "";

        $checked.each(function() {
            var $checkbox = $(this);
            signedupFor += ">" + $checkbox.siblings('label').text();
        });


        // Set up tracking object
        var trackingObject = {
            category: 'Email-Signup',
            action: signedupFor,
            label: '',
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});
///////////////////////////
// YOUTUBE
///////////////////////////

_tracking.handlers.push(function () {

    $(document).on('mousedown', '#player-overlay #play-pause', function (event) {

        var $title = $('.js-header-title');

        // Set up tracking object
        var trackingObject = {
            category: 'Video',
            action: 'Play',
            label: $.trim($title.text()),
            value: null,
            nonint: true
        };

        // Set up event object
        var eventObj = {
            type: event.type,
            href: event.target.href,
            allowRedirect: true
        };

        _tracking.trackevent(trackingObject, eventObj);
    });
});

// Loop over all handlers in handlers array, executing the functions to bind them
for (var i = 0; i < _tracking.handlers.length; i++) {
    _tracking.handlers[i]();
}

// Track refferers
if (document.referrer.match(/google\./gi) && document.referrer.match(/cd/gi)) {
    var myString = document.referrer;
    var r = myString.match(/cd=(.*?)&/);
    var rank = parseInt(r[1]);
    var kw = myString.match(/q=(.*?)&/);

    if (kw[1].length > 0) {
        var keyWord = decodeURI(kw[1]);
    } else {
        keyWord = "(not provided)";
    }

    var p = document.location.pathname;
    _gaq.push(['_trackEvent', 'RankTracker', keyWord, p, rank, true]);
}