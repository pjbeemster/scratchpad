/* switch off cookies for add this to stop case where we have different counts on different devices CC-1482*/
var addthis_config = {
    data_use_cookies: false,
    data_track_clickback: false
};



// Function to encaspulate window.resource functionality
function getResource(key) {

    var label = 'Unable to locate label: ' + key;

    if (typeof window.Resources != 'undefined') {

        label = window.Resources['key'];
    }

    return label;
}

if (!Modernizr.input.placeholder) {
    $('[placeholder][type!="password"]').focus(function () {
        var input = $(this);
        if (input.val() == input.attr('placeholder')) {
            input.val('');
            input.removeClass('placeholder');
        }
    }).blur(function () {
        var input = $(this);
        if (input.val() == '' || input.val() == input.attr('placeholder')) {
            input.addClass('placeholder'); 
            input.val(input.attr('placeholder'));
        }
    }).blur();
    $('[placeholder][type!="password"]').parents('form').submit(function () {
        $(this).find('[placeholder][type!="password"]').each(function () {
            var input = $(this);
            if (input.val() == input.attr('placeholder')) {
                input.val('');
            }
        })
    });
}


function initRegPostcodeGeocoder() {
        var cgmSettingsObj = {
            mapSelector: '#geolocation-canvas',
            btnSelector: '#btnLocate',
            latSelector: '#lat',
            lngSelector: '#long',
            zoom: 11
        };

        var centerLat = $('.register #centerOnLat').val();
        var centerLng = $('.register #centerOnLng').val();
        if (centerLat && centerLng) {
            cgmSettingsObj.mapOptions = { center: new google.maps.LatLng(centerLat, centerLng) };
        }

        // Create Coats Google Geocoder
        $('#RegistrationForm_AddressDetails_Postcode').coatsGoogleMapper(cgmSettingsObj);
}

// Resets iframes height based on its content
function resetHeight(iframeHeight) {

    $('#panels').height(iframeHeight);
    $('#dropdown').height(iframeHeight);
}

function resetModalHeight(iframeHeight) {

    if ($('.modal').length > 0) {
        $('.modal iframe').height(iframeHeight)
    }
}

function resetMainMinHeight() {

    $('#main .faceted').css('min-height', ($('.filter-panel').outerHeight() + 60));
}

// Youtube on ready handler
function onYouTubeIframeAPIReady() {

    // ASH: Not sure this is being hit from anywhere???
    //      Main youtube rendering is done by HtmlHelpers/VideoHelper.cs.

	var video = $('#ytplayer');

	if (video.length > 0) {

	    var playerUrl = video.find('iframe').attr('src'), videoId = '';

        // Potential fix for pulling out video ID - needs testing properly
	    if (playerUrl.indexOf('http://www.youtube.com/embed/') != -1) {
	        videoId = playerUrl.replace('http://www.youtube.com/embed/', '');
	    }

	    if (playerUrl.indexOf('http://youtu.be/') != -1) {
	        videoId = playerUrl.replace('http://youtu.be/', '');
	    }

	    if (videoId != '') {

	        // Before swapping out stock iframe, check postmessage is supported
	        if ($('html').hasClass('postmessage')) {

            	function onPlayerStateChange(e) {
	                if (e.data === 2) {
	                    player.pauseVideo();
	                }
	            }

	            // Initialise player on page
	            var player = new YT.Player('ytplayer', {
	                height: '309',
	                width: '550',
	                videoId: videoId,
	                playerVars: {
	                    'controls': 1,
	                    'modestbranding': 1,
	                    'showinfo': 0,
	                    'wmode': 'transparent',
                        'rel' : 0
	                },
	                events: {
	                    'onStateChange': onPlayerStateChange
	                }
	            });

	            var overlay = $('#player-overlay');

	            if (overlay.length > 0) {

	                overlay.find('#play-pause').on('mousedown', function (e) {

	                    // Catch default action
	                    e.preventDefault();

	                    // Hide the stock iframe
	                    $('#original').hide();

	                    // Play the video
	                    player.playVideo();

	                    overlay.find('#play-pause').fadeOut(250);

	                    // Hide the overlay after 250 milliseconds to hide youtube play button
	                    setTimeout(function () {
	                        overlay.hide();
	                    }, 250);
	                });
	            }
	        }
	    }
	}
}

function equaliseHeights(els) {
    var maxHeight = 0;

    $(els).each(function () {
        if ($(this).height() > maxHeight) { maxHeight = $(this).height(); }
    });

    $(els).height(maxHeight);
}


// Ajax failed response handler
function failHandler(jqXHR, status, error) {

    // console.log('Ajax page request failed: ' + status + ' - ' + error, jqXHR);

    top.toast(window.Resources.AjaxError);
}


// Adds leading zeroes to number
function zeroPad(num, places) {
	var zero = places - num.toString().length + 1;
	return Array(+(zero > 0 && zero)).join("0") + num;
}

// Method to scroll to a specified selector
function scrollTo(selector, speed, easing, offset) {
	if ($(selector)) {
		offset = (offset) ? offset : 0;
		speed = (speed) ? speed : 400;
		easing = (easing) ? "swing" : easing;
		var ypos = $(selector).offset().top - offset;

		$('html, body').stop().animate({ scrollTop: ypos }, speed, easing);
	}
}

/*Detect android (currently for checkbox fallback styling)*/
var nua = navigator.userAgent;
var is_android = nua.indexOf('Android') > -1 && nua.indexOf('Chrome') < 0;
if (is_android) {
    $('html').addClass('android-browser');
}

/* Initialise custom scroller - called on resize (triggered on load) and on ajax update if filter panel is present */
function initCustomScroller () { 

        $('.no-touch .filter-panel .custom-scroller').mCustomScrollbar('destroy');

        if ($(window).width() >= 768) {
            $('.no-touch .filter-panel .custom-scroller').css('max-height', $(window).height() - 150);

            $('.no-touch .filter-panel .custom-scroller').mCustomScrollbar({
                scrollInertia: 0,
                mouseWheel: false,
                theme: 'dark-thin',
                autoDraggerLength: true,
                advanced: {
                    updateOnContentResize: true
                },
                scrollButtons: {
                    enable: true
                }
            });

            resetMainMinHeight();
        }
    }

    //CCPLCR-12 - Chrome sometimes doesn't center webfont correctly
    function chromeCenteringFix() {
        var affectedEls = "#navbar #main-nav li a, .pod .content .head h1, .ruled h1, .ruled h2, .ruled h1:before, .ruled h1:after, .ruled h2:before, .ruled h2:after";
        $(affectedEls).addClass('chromeCenteringFix');
        setTimeout(function () {
            $(affectedEls).removeClass('chromeCenteringFix');
            $('.isotope').isotope('reLayout');
        }, 200);
    }

    $(window).on('load', function () {

        if (nua.indexOf('Chrome') > 0) {
            chromeCenteringFix();
        }
        /* Set up new home page carousel ----------*/

        // NOTE - Currently this carousel uses bootstraps stock one which allows infinite rotation

        var nuCarousel = $('#home-carousel'),
        panels,
        indicators;

        if (nuCarousel.length > 0) {

            panels = nuCarousel.find('.carousel-inner .item');
            indicators = $('<ol class="carousel-indicators" />');

            // Build carousel pips
            panels.each(function (i) {

                indicators.append('<li data-target="#home-carousel" data-slide-to="' + i + '" />');
            });

            nuCarousel.prepend(indicators);

            var leftArr = $('<a class="carousel-control icon left" data-target="#home-carousel" data-slide="prev" rel="prev">&lsaquo;</a>');
            var rightArr = $('<a class="carousel-control icon right" data-target="#home-carousel" data-slide="next" rel="next">&rsaquo;</a>');

            // Build carousel controls
            nuCarousel.append(leftArr, rightArr);

            panels.first().addClass('active');

            indicators.find('li').first().addClass('active');

            $('#home-carousel').carousel({
                interval: 10000,
                pause: "false"
            });

        }


        //setup article carousels

        initBootStrapCarousel('.full-width-carousel');
        initBootStrapCarousel('.inline-carousel');
        initBootStrapCarousel('.new-slider.brands');

        function initBootStrapCarousel(selector) {

            var myCarousel = $(selector),
        myPanels,
        myIndicators;

            if (myCarousel.length > 0) {

                myPanels = myCarousel.find('.carousel-inner .new-panel');
                myIndicators = $('<ol class="carousel-indicators" />');

                // Build carousel pips
                myPanels.each(function (i) {

                    myIndicators.append('<li data-target="' + selector + '" data-slide-to="' + i + '" />');
                });

                myCarousel.prepend(myIndicators);

                var myLeftArr = $('<a class="carousel-control icon left" data-target="' + selector + '" data-slide="prev">&lsaquo;</a>');
                var myRightArr = $('<a class="carousel-control icon right" data-target="' + selector + '" data-slide="next">&rsaquo;</a>');

                // Build carousel controls
                myCarousel.append(myLeftArr, myRightArr);

                myPanels.first().addClass('active');

                myIndicators.find('li').first().addClass('active');

                $(selector).carousel({
                    interval: 10000,
                    pause: "false"
                });


                var offset = (((myIndicators.find('li').length * myIndicators.find('li').first().outerWidth(true)) / 2) + 18);

                myLeftArr.css({
                    left: 'auto',
                    marginRight: offset + 'px',
                    right: '50%'
                });

                myRightArr.css({
                    left: '50%',
                    marginLeft: offset + 'px',
                    right: 'auto'
                });


                // Bind handlers to reposition controls on resize
                $(window).on('resize', function () {

                    var offset = (((myIndicators.find('li').length * myIndicators.find('li').first().outerWidth(true)) / 2) + 18);

                    myLeftArr.css({
                        left: 'auto',
                        marginRight: offset + 'px',
                        right: '50%'
                    });

                    myRightArr.css({
                        left: '50%',
                        marginLeft: offset + 'px',
                        right: 'auto'
                    });
                });
            }

        }




        /* Trigger resize event to force isotope to reflow on page load */

        $(window).trigger('resize');

        /* Drop down panels ----------------------------*/

        // Fire iframe height reset if we're open in an iframe
        if (self != top) {

            // HACK - Set timeout to zero to force execution to base of call stack
            setTimeout(function () {
                top.resetHeight($('body').outerHeight());
                top.resetModalHeight($('body').outerHeight());
            }, 0);
        }


        /* Slide in sidebar ----------------------------*/

        if ($(window).width() > 768 && !$('html').hasClass('touch')) {
            setTimeout(function () {
                $('.show-filter').trigger('click');
            }, 800);
        }

        // facetedIsotopes.isotope('reLayout');


        /* YT Video Handler - Player -------------------*/

        var video = $('#ytplayer');

        if (video.length > 0) {
            // Load in YT scripts
            var ytScript = document.createElement('script'),
					firstTag = document.getElementsByTagName('script')[0];

            ytScript.src = 'https://www.youtube.com/iframe_api';
            firstTag.parentNode.insertBefore(ytScript, firstTag);
        }


        /* Configure sliders ---------------------------*/

        var sliders = $('.new-slider');

        if (sliders.length > 0) {

            // Initialise Brands Slider
            //sliders.coatsSlider();
        }


        /* Sort table ----------------------------------*/

        $('table.sort').tablesorter();
        $('table.sort-with-exceptions').tablesorter({ selectorHeaders: 'thead th:not(.no-sort)', sortList: [[0, 0]] });


        /* Moodboard scripts ---------------------------*/

        var moodboardIsotope = $('.moody .pods');

        if (moodboardIsotope.length > 0) {

            // Initialise isotope
            moodboardIsotope.isotope({ itemSelector: '.pod', masonry: { columnWidth: moodboardIsotope.width() / 4 }, animationEngine: 'css' });

            // Horrible fix for width issues
            setTimeout(function () {
                moodboardIsotope.isotope('reLayout');
            }, 1);

            // Reinitalise isotope on window resize
            $(window).on('resize', function () {

                // Only apply to modern browsers
                // if (!$('html').hasClass('lt-ie9')) {

                // NOTE: May be worth adding extra values in here for each breakpoint
                //moodboardIsotope.isotope({ itemSelector: '.pod', masonry: { columnWidth: moodboardIsotope.width() / 4 }, animationEngine: 'css' });
                //moodboardIsotope.isotope('reLayout');
                moodboardIsotope.isotope('destroy');
                moodboardIsotope.isotope({ itemSelector: '.pod', masonry: { columnWidth: moodboardIsotope.width() / 4 }, animationEngine: 'css' });
                //}
            });

            $('.moody .pod > a').on('click', function (e) {

                e.preventDefault();
                var itemLink = $(this),
				itemImage = itemLink.find('img'),
				item = itemLink.parent(),
				magIcon = item.find('[class*="zoom"]'),
				newSrc;

                if (!item.hasClass('open')) {
                    item.addClass('open');
                    newSrc = itemLink.attr('href');
                    magIcon.removeClass('zoom-in').addClass('zoom-out');
                } else {
                    item.removeClass('open');
                    newSrc = itemImage.data('img');
                    magIcon.removeClass('zoom-out').addClass('zoom-in');
                }

                if (newSrc !== itemImage.attr("src")) {

                    // Dependency on load of new image - if a new image isn't supplied the relayout won't be triggered
                    itemImage.attr('src', newSrc).on('load', function () {

                        moodboardIsotope.isotope('reLayout', function () {

                            scrollTo(item, null, 'easeInOutQuart', 10);

                            setInterval(function () {
                                moodboardIsotope.isotope('reLayout');
                            }, 100);
                        });
                    });
                } else {
                    moodboardIsotope.isotope('reLayout');
                }
            });

            $('a.zoom-in').on('click', function (e) {

                e.preventDefault();

                $(e.target).parents('.pod').find(' > a').trigger('click');
            });
        }




        /* Configure show/hide on store table ----------*/

        var moreLess = $('.search-results .moreless ')

        if (moreLess.length > 0) {

            moreLess.on('click', function (e) {

                e.preventDefault();

                var target = $(e.target);

                target.parents('li').toggleClass('open');
            });
        }


        /* Configure sliding tables --------------------*/
        // @NOTE - Would be nice to merge this with the slider crumbs logic below in a plugin if possible to normalise markup enough

        /** REMOVED ON 17/10/2013 - replaced with the following CSS: .overflower {overflow: scroll; } */

        //  var overflower = $('.overflower');

        //  // Check if crumbs bar is on the page
        //  if (overflower.length > 0) {
        //      // Declare variables
        //      var lastTouchPosX = false,
        // innerTable = overflower.find('table'),
        // allowMovement = false;

        //      // Bind event to touchstart and mousedown
        //      overflower.on('touchstart', function (e) {

        //          // Catch event
        //          e.preventDefault();

        //          // Enable movement
        //          allowMovement = true;
        //      });

        //      // Bind event to touchend and mouseup
        //      overflower.on('touchend', function (e) {

        //          e.preventDefault();
        //          // Check if movement is allowed
        //          if (allowMovement) {

        //              // Check if width of elements is less than window width
        //              if (innerTable.width() < $(window).width()) {

        //                  // If so, set margin left position back to zero
        //                  innerTable.animate({ 'margin-left': '0px' }, 400);
        //              }
        //          }

        //          // Disable movement
        //          lastTouchPosX = false;
        //          allowMovement = false;
        //      });

        //      // Bind event to touch and mouse move
        //      overflower.on('touchmove', function (e) {

        //          // Catch event
        //          e.preventDefault();

        //          // Check if movement is allowed
        //          if (allowMovement) {

        //              var currentPosX;

        //              // Set current positions from touch events
        //              if (e.type == 'touchmove') {
        //                  currentPosX = e.originalEvent.touches[0].screenX;
        //              }

        //              // Set current positions from click events
        //              //                if (e.type == 'mousemove') {
        //              //                    currentPosX = e.pageX;
        //              //                }

        //              // If we have a last position...
        //              if (lastTouchPosX) {

        //                  var diff = currentPosX - lastTouchPosX;

        //                  // Set margin left position
        //                  innerTable.css('margin-left', '+=' + diff + 'px');
        //              }

        //              // Reset last position
        //              lastTouchPosX = currentPosX;
        //          }
        //      });
        //  }


        /* Configure sliding crumbs bar ----------------*/

        var crumbsBar = $('.crumbs');

        // Check if crumbs bar is on the page
        if (crumbsBar.length > 0) {

            // Declare variables
            var lastTouchPosX = false,
			crumbsRow = crumbsBar.find('.row-fluid'),
		allowMovement = false;

            crumbsRow.css('overflow', 'hidden');

            // Bind event to touchstart and mousedown
            crumbsRow.find('.span12').on('touchstart', function (e) {

                // Enable movement
                allowMovement = true;
            });

            // Bind event to touchend and mouseup
            crumbsRow.find('.span12').on('touchend', function (e) {

                // Check if movement is allowed
                if (allowMovement) {

                    var crumbElementsWidth = crumbsRow.find('.back-to-search').width() + crumbsRow.find('ul').width();

                    // Check if width of elements is less than window width
                    if (crumbElementsWidth < $(window).width()) {

                        // If so, set margin left position back to zero
                        crumbsRow.find('.span12').animate({ 'margin-left': '0px' }, 400);
                    }
                }

                // Disable movement
                lastTouchPosX = false;
                allowMovement = false;
            });

            // Bind event to touch and mouse move
            crumbsRow.find('.span12').on('touchmove', function (e) {

                // Catch event
                e.preventDefault();

                // Check if movement is allowed
                if (allowMovement) {

                    var currentPosX;

                    // Set current positions from touch events
                    if (e.type == 'touchmove') {
                        currentPosX = e.originalEvent.touches[0].screenX;
                    }

                    // Set current positions from click events
                    //                if (e.type == 'mousemove') {
                    //                    currentPosX = e.pageX;
                    //                }

                    // If we have a last position...
                    if (lastTouchPosX) {

                        var diff = currentPosX - lastTouchPosX;

                        // Set margin left position
                        crumbsRow.find('.span12').css('margin-left', '+=' + diff + 'px');
                    }

                    // Reset last position
                    lastTouchPosX = currentPosX;
                }
            });
        }


        /* Initialise general isotopes ---------------------*/

        var generalIsotopes = $('.izotaup .pods');

        // Check if the filtered isotope component is present on page
        if (generalIsotopes.length > 0) {

            generalIsotopes.each(function () {

                var currentIzzle = $(this);

                // Initialise isotope
                currentIzzle.isotope({ itemSelector: '.pod', animationEngine: 'css' });

                // Horrible fix for width issues
                setTimeout(function () {
                    currentIzzle.isotope('reLayout');
                }, 1);

                // Reinitalise isotope on window resize
                $(window).on('resize', function () {

                    currentIzzle.isotope('reLayout');
                });
            });
        }


        /* Filtered isotope -----------------------*/

        var filteredIsotope = $('.filtered-isotope .pods');

        // Check if the filtered isotope component is present on page
        if (filteredIsotope.length > 0) {

            // Set up isotope filtering
            var filters = $('.filtered-isotope .filters');

            // Bind event hanlder to filters
            filters.on('click', 'a', function (e) {

                e.preventDefault();

                var link = $(this);

                link
				.parents('ul')
				.find('li')
				.removeClass('selected');

                link
				.parents('li')
				.addClass('selected');

                filteredIsotope.isotope({ filter: link.attr('data-filter'), sortBy: 'random', animationEngine: 'css' });
            });

            var options = {
                animationEngine: 'css',
                itemSelector: '.pod',
                filter: filters.find('li.selected a').attr('data-filter'),
                sortBy: 'random'
            };


            // Initialise isotope
            filteredIsotope.isotope(options);
            $('.filtered-isotope').show();

            // Horrible fix for width issues
            setTimeout(function () {
                filteredIsotope.isotope('reLayout');
            }, 1);

            // Reinitalise isotope on window resize
            $(window).on('resize', function () {

                // Only apply to modern browsers
                //if (!$('html').hasClass('lt-ie9')) {

                filteredIsotope.isotope('reLayout');
                //}
            });
        }


        /* Rating tooltips ------------------------*/

        var tooltipvalues = [
		window.Resources.RatingPoor,
		window.Resources.RatingNotSoGood,
		window.Resources.RatingOk,
		window.Resources.RatingGood,
		window.Resources.RatingExcellent
	];

        $("#ratingform").on('over', function (event, value) {
            $(this).attr('title', tooltipvalues[value - 1]);
        });

        $("#ratingform").on('click touchend', function (event, value) {
            $(this).parents('form').trigger('submit');
        });
    });

// Reset custom scroller
$(window).on('resize', initCustomScroller);


var facetedIsotopes;

$(document).on('ready', function () {

    // COATSCRAFTSWARRANTY-47 - Windows Mobile reports no touch so the menu was cut short!
    //$('.no-touch .filter-panel .custom-scroller').css('max-height', $(window).height() - 150);
    if (!Modernizr.touch && $(window).width() > 799) {
        $('.no-touch .filter-panel .custom-scroller').css('max-height', $(window).height() - 150);
    }

    /* Set up scroll event tracking ----------------*/
    $.scrollDepth();


    /* Show search box in product nav --------------*/

    var showSearchBtn = $('.show-search');

    if (showSearchBtn.length > 0) {

        showSearchBtn.on('click', function (e) {

            var searchBlock = showSearchBtn.parents('.searchblock');

            searchBlock.addClass('visi');
        });
    }


    /* Initialise dropdown click functionality -------*/

    var droppers = $('.dropdown');

    $(document).on('click', function (e) {

        droppers.find('ul').hide();
    });

    droppers.on('mouseleave', 'ul', function (e) {

        $(this).hide();
    });

    droppers.live('click', function (e) {

        e.stopPropagation();

        droppers.not($(this)).find('ul').hide(); // Stops multiple dropdowns being open

        $(this).find('ul').toggle({
            complete: function () {

                // COATSCRAFTSWARRANTY-53 - change the z-index such that the product nav is above the left-hand panel if
                // if the left-hand pnael is open
                var filteropen = parseInt($(".filter-panel").css("left")) == 0 || false;
                if (filteropen) {
                    $(this).parents(".product-nav").css("z-index", "999");
                }
                else {
                    $(this).parents(".product-nav").css("z-index", "250");
                }
            }
        });
    });


    /* Position docked elements --------------------*/

    var dockered = $('#sideshare, .show-filter, .filter-panel');

    if (dockered.length > 0) {
        dockered.coatsDockSideBar();
    }


    /* Initialise faceted isotopes -----------------*/

    facetedIsotopes = $('.faceted .pods');

    // Check if the filtered isotope component is present on page
    if (facetedIsotopes.length > 0) {

        // Initialise isotope
        facetedIsotopes.isotope({ itemSelector: '.pod', animationEngine: 'css' });

        // Reinitalise isotope on window resize
        $(window).on('resize', function () {

            facetedIsotopes.isotope('reLayout');
        });
    }

    // Bind click event handlers to filter panel links
    var filterPane = $('.filter-panel');

    // Check we have a panel on page
    //windows mobile seems to have a problem with click handlers on none <a> elements
    //changed this to target the a rather than the h2 COATSCRAFTSWARRANTY-47
    $(document).on('click', '.filter-panel h2 a, .filter-panel h2 a span', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        $(this).parents('h2').toggleClass('closed');

        resetMainMinHeight();

        /*
        *
        * Scroll down to bring panel into view more
        * on interaction if scroll position is less 
        * than offset top of title area.
        *
        */

        var productHeader = $('.product-header');

        if ($(window).width() > 768) {

            if (window.scrollY < productHeader.offset().top) {

                scrollTo(productHeader);
            }
        }

        if (!$(this).parents('h2').hasClass('closed')) {
            if ($('.mCSB_container').length > 0) {
                var scrollVal = $(this).parents('h2').offset().top - $('.mCSB_container').offset().top;
                $('.no-touch .filter-panel .custom-scroller.mCustomScrollbar').mCustomScrollbar("scrollTo", scrollVal);
            }
        }
    });

    // Bind submit event handler to forms
    var form = $('.fh-filter');

    if (form.length > 0) {

        $(document).on('submit', '.fh-filter', function (e) {

            // Catch default submit
            e.preventDefault();

            // history.js and back button fix
            // set the stateChangeIsLocal to true
            window.stateChangeIsLocal = true;

            // Fire up ajax request
            var ajaxed = isotopeHelper.ajaxer({
                data: getFormData($(this).find(':input')),
                dataType: 'html',
                type: 'POST'
            });

            // Able to hook into deffered object here
            // ajaxed.done(function () { });
        });

        $(document).on('change', '.fh-filter input[type="checkbox"]', function (e) {

            // Fire off form submission
            $(this).parents('form').trigger('submit');
        });
    }


    // Bind event handlers to product nav links
    var prodNav = $('.product-nav');

    if (prodNav.length > 0) {

        $(document).on('click', '.product-nav a.radioesque', function (e) {

            // history.js and backbutton fix
            // set the stateChangeIsLocal to false so we're not calling ajaxer twice
            window.stateChangeIsLocal = true;

            // Catch the click
            e.preventDefault();

            // Fire up ajax request
            var ajaxed = isotopeHelper.ajaxer({
                dataType: 'html',
                type: 'GET',
                url: $(this).attr('href')
            });

            // Able to hook into deffered object here
            // ajaxed.always(function () { });
        });
    }

    // -----------------------------
    // Bind event handlers to active discussion nav links
    var prodHeader = $('.product-header');

    if (prodHeader.length > 0) {

        $(document).on('click', '.product-header .active-discussion a.radioesque', function (e) {

            // history.js and backbutton fix
            // set the stateChangeIsLocal to false so we're not calling ajaxer twice
            window.stateChangeIsLocal = true;

            // Catch the click
            e.preventDefault();

            // Fire up ajax request
            var ajaxed = isotopeHelper.ajaxer({
                dataType: 'html',
                type: 'GET',
                url: $(this).attr('href')
            });

            // Able to hook into deffered object here
            // ajaxed.always(function () { });
        });
    }
    // -----------------------------


    // Bind click event handlers to show more
    var showMore = $('.show-more');

    if (showMore.length > 0) {

        $(document).on('click', '.show-more', function (e) {

            // Catch the click
            e.preventDefault();

            // history.js and backbutton fix
            // set the stateChangeIsLocal to false so we're not calling ajaxer twice    
            window.stateChangeIsLocal = true;

            var currentPageIndex = showMore.find('ul li.current').index();
            var intendedPage = showMore.find('ul li').eq(parseInt(currentPageIndex) + 1);

            // Fire up ajax request
            var ajaxed = isotopeHelper.ajaxer({
                dataType: 'html',
                type: 'GET',
                url: intendedPage.find('a').attr('href')
            }, true);

            // Able to hook into deffered object here
            // ajaxed.always(function () { });
        });
    }

    var filterLinks = $('.dropdown.sort, .dropdown.category');

    if (filterLinks.length > 0) {

        $(document).on('click', '.dropdown.sort a, .dropdown.category a', function (e) {
            // Catch the click
            e.preventDefault();

            window.stateChangeIsLocal = true;

            // Fire up ajax request
            var ajaxed = isotopeHelper.ajaxer({
                dataType: 'html',
                type: 'GET',
                url: $(this).attr('href')
            });

            // Allow filter panel to be docked  after a selection is made in product category
            ajaxed.done(function () {
                var dockered = $('.filter-panel');
                if (dockered.length > 0) {
                    dockered.coatsDockSideBar();
                }
            });


            // Able to hook into deffered object here
            // ajaxed.always(function () { });
        })
    }


    // Bind click event handlers to brand filter links
    var brandFilter = $('.brand-filter');

    if (brandFilter.length > 0) {

        $(document).on('click', '.brand-filter .close', function (e) {

            if (!$(this).hasClass('no-ajax')) {
                // Catch the click
                e.preventDefault();

                // Chaining the anonymous ajax method to the slideUp was losing the scope 
                // of the href attribute, so store it here first.
                var urlClose = $(this).attr('href');

                brandFilter.slideUp(function () {

                    // Fire ajax call to reset pods
                    // var ajaxed = isotopeHelper.ajaxer({
                    //     dataType: 'html',
                    //     type: 'GET',
                    //     url: $(this).attr('href')
                    var ajaxed = isotopeHelper.ajaxer({
                        dataType: 'html',
                        type: 'GET',
                        url: urlClose
                    });

                    // Able to hook into deffered object here
                    // ajaxed.always(function () { });
                });
            }
            else {
                //let it do a postback
            }
        });
    }

    /*
    *
    * Shows filter panel, handles all screen sizes
    *
    */

    var facetedClickHandler = function (e) {
        e.preventDefault();
        e.stopPropagation();
    };

    function showFilterPanel(e) {

        if (typeof e != 'undefined') {
            e.preventDefault();
        }

        if ($(window).width() > 768) {

            $('.show-filter').animate({ 'left': -$('.show-filter').outerWidth() }, 200, function () {
                $('.filter-panel').animate({ 'left': '0px' }, 600, 'easeInOutQuart');
            });

        } else {
            $('.filter-panel').show();

            // COATSCRAFTSWARRANTY-47 - stops links under the panel from being clickable in Windows Mobile
            $('body').on("click", '.faceted article a', facetedClickHandler);
            $('.faceted input').attr('disabled', 'disabled');
        }
    }

    /*
    *
    * Hides filter panel, handles all screen sizes
    *
    */
    function hideFilterPanel(e) {

        if (typeof e != 'undefined') {
            e.preventDefault();
        }

        if ($(window).width() > 768) {

            $('.filter-panel').animate({ 'left': -$('.filter-panel').outerWidth() }, 600, 'easeInOutQuart', function () {
                $('.show-filter').animate({ 'left': 0 }, 200);
            });
        } else {
            $('.filter-panel').hide();
            // COATSCRAFTSWARRANTY-47 - stops links under the panel from being clickable in Windows Mobile
            $('body').off("click", '.faceted article a', facetedClickHandler);
            $('.faceted input').removeAttr('disabled');
        }
    }

    /*
    *
    * Normalises any additional styling and classes
    * on resize allow base CSS to take over
    * for positioning
    *
    */
    var resizeWidth = $(window).width();

    function resetFilterPanel(e) {

        /*
        *
        * Check if resizing horizontally.
        * Fixes craziness in mobile webkit
        * et al which fire resize events on
        * scroll
        *
        */
        if ($(window).width() != resizeWidth) {

            if ($(window).width() > 768) {

                $('.filter-panel').css({ 'left': -$('.filter-panel').outerWidth() }).show();
                $('.show-filter').css({ 'left': 0 }, 200);
                if ($('.product-nav .control-group a').length == 0) {
                    putContentTypesBackIntoProductNav();
                }
            } else {

                $('.filter-panel').removeAttr('style').hide();
                $('.show-filter').removeAttr('style');
                if ($('.filter-panel .facet.types').length == 0) {
                    pullContentTypesIntoFilterPanel();
                }
            }

            resizeWidth = $(window).width();
        }
    }


    /**
    * DOM mutation for portrait-landscape products link + category dropdown.
    */
    ; (function () {

        // only applies to the first control group item.
        if ($('.product-nav .control-group.arrow-base').index() === 0) {

            function relayout(refetch) {
                $newRow.css("display", Modernizr.mq("screen and (max-width: 768px)") ? "block" : "none")
                if (typeof refetch != 'undefined') {
                    if (refetch) {
                        $container.empty()
                        $container.append($('.product-nav .control-group.arrow-base').clone())
                    }
                }
            }

            var $newRow = $('<div class="row-fluid catter"><div class="span12"></div></div>')

            relayout()

            $newRow
                .find('.span12')
                .append($('.product-nav .control-group.arrow-base').clone())
                .end()
                .insertAfter($('.product-header .row-fluid.headers'));

            var $container = $newRow.find('.span12')

            $(window).bind("resize", relayout)

            $(window).on("relayout", relayout)


        }

    })();

    /*
    *
    * Pull product categories
    * into filter panel
    *
    */

    //    if ($(window).width() <= 768) {

    //        if ($('.product-nav .category').length > 0) {

    //            if ($('.filter-panel').length > 0) {

    //                var categoryLinks = $('.product-nav .category a'),
    //                    facetBlock = $('<div class="facet categories"><h2>Categories<span></span></h2><ul></ul></div>');

    //                // Build category list
    //                categoryLinks.each(function () {
    //                    $('<li />').html($(this).prepend('<span />')).appendTo(facetBlock.find('ul'));
    //                });


    //                facetBlock.insertBefore($('.filter-panel .reset'))

    //            } else {

    //                var mainArticle = $('#main > article');

    //                var categoryLinks = $('.product-nav .category a'),
    //                    facetBlock = $('<div class="facet categories"><h2>Categories<span></span></h2><ul></ul></div>');

    //                // Build category list
    //                categoryLinks.each(function () {
    //                    $('<li />').html($(this).prepend('<span />')).appendTo(facetBlock.find('ul'));
    //                })

    //                // Create panel and then pull category list in
    //                $('<div class="show-filter"><a title="Show filter panel" rel="nofollow"></a></div>').prependTo(mainArticle);
    //                $('<div class="filter-panel"><div><a href="#" class="icon close small" title="Hide filter panel" rel="nofollow">Close</a><h1>Filter By:</h1></div></div>').append(facetBlock).prependTo(mainArticle);
    //            }
    //        }
    //    }


    /*
    *
    * Pull content type filters
    * into filter panel
    *
    */
    if ($(window).width() <= 768) {
        pullContentTypesIntoFilterPanel();
    }

    function pullContentTypesIntoFilterPanel() {
        var sideTypeFilter = $('<div class="facet types" />');

        sideTypeFilter.append('<h2>Content Types</h2>').append('<ul />');

        if ($('.product-nav .control-group .radioesque').length > 0) {

            $('.product-nav .control-group .radioesque').each(function () {

                sideTypeFilter.find('ul').append($('<li />').append($(this).clone().removeClass('radioesque')));
            });

            $('.filter-panel').find('.reset').before(sideTypeFilter);
        }
    }

    function putContentTypesBackIntoProductNav() {

        var contentTypes = $('.filter-panel .facet.types li');

        if ($(contentTypes).length > 0) {
            var prodNavCGs = $('.product-nav .control-group');
            $(contentTypes).each(function (i) {
                $(prodNavCGs).eq(i).append($(this).html());
            });

            $('a', prodNavCGs).addClass('radioesque');
        }
    }


    $(window).on('resize', resetFilterPanel);
    $(document).on('click', '.show-filter', showFilterPanel);
    $(document).on('click', '.filter-panel .close', hideFilterPanel);
    $(document).on('click', '.filter-panel a:not(.close, .mCSB_buttonUp, .mCSB_buttonDown, .filter-title)', function (e) {

        e.preventDefault();

        // history.js and backbutton fix
        // set the stateChangeIsLocal to false so we're not calling ajaxer twice
        window.stateChangeIsLocal = true;

        // Fire up ajax request
        var ajaxed = isotopeHelper.ajaxer({
            dataType: 'html',
            type: 'GET',
            url: $(this).attr('href')
        });

        // Able to hook into deffered object here
        ajaxed.done(function () {

            scrollTo($('#main'), 400);
        });
    });


    var feedback = $('#feedback');

    if (feedback.length > 0) {

        // Set timeout to remove feedback anyway
        var feedbackTimeout = setTimeout(function () {
            feedback.remove();
        }, 4000)

        // Handle close link click
        $(document).on('click', '#feedback > a', function (e) {

            e.preventDefault();

            clearTimeout(feedbackTimeout);

            feedback.remove();
        });
    }

    // Initialize bootstrap tooltips - Only apply to no touch devices
    if ($(window).width() > 768) {
        $('*[data-toggle="tooltip"]').tooltip({ placement: 'bottom' });
    }

    // Requires Toast
    $(document).on('logged-in', function (e) {
        toast(window.Resources.LoginSuccess);
    });

    // Requires Toast
    $(document).on('comment-added', function (e) {

        toast(window.Resources.Feedback_AddedComment);
    });


    /* Inserted shopping list event ----------------*/

    var scrapbooktimer = false;

    // Requires Toast
    $(document).on('inserted-shoppinglist', function (e, locationHref) {

        var icon = $('.list.ajax');

        if (!icon.hasClass('show')) {

            // Set new tooltip
            icon
            //.attr('title', 'Item added!')
            .attr('title', window.Resources.Itemadded)
            
				.tooltip('fixTitle')
				.tooltip('show');

            // Reload iframes if open
            if ($('iframe#panels').is(':visible')) {

                $('iframe#panels').attr('src', function (i, val) { return val; });
            }

            // Set timer to reset tooltip
            shoppingListTimer = setTimeout(function () {

                icon
					.attr('title', 'Shopping List')
					.tooltip('fixTitle')
					.tooltip('hide');
            }, 5000);

            // Fire toast message

            toast(window.Resources.Feedback_AddedToShoppingList);

        }


    });


    $(".scrapbook form").submit(function (e) {
        if ($(this).valid() == false) {
            //validation causes form to get bigger so we need to reset it
            top.resetHeight($('body').outerHeight());
        }
    });


    /* Inserted scrapbook event -------------------*/

    var shoppingListTimer = false;

    // Requires Toast
    $(document).on('inserted-scrapbook', function (e, locationHref) {

        var icon = $('.scrapbook.ajax');

        if (!icon.hasClass('show')) {

            // Set new tooltip
            icon
            //.attr('title', 'Item added!')
                .attr('title', window.Resources.Itemadded)
				.tooltip('fixTitle')
				.tooltip('show');

            // Reload iframes if open
            if ($('iframe#panels').is(':visible')) {

                $('iframe#panels').attr('src', function (i, val) { return val; });
            }

            // Set timer to reset tooltip
            shoppingListTimer = setTimeout(function () {

                icon
					.attr('title', 'Scrapbook')
					.tooltip('fixTitle')
					.tooltip('hide');
            }, 5000);

            // Fire toast message
            toast(window.Resources.Feedback_AddedToScrapbook);
        }

    });


    /* Comment form ---------------------------*/

    var commentForm = $('form#comment');

    if (commentForm.length > 0) {

        var textarea = commentForm.find('textarea'),
			commentCountDisplay = commentForm.find('.comment-count'),
        //remainingcharacters = commentForm.find('Remainingcharacters'),
        //  remainingcharacters = document.getElementById("Remainingcharacters").value,
            remainingcharacters = window.Resources.Remainingcharacters,
			maxLength = textarea.attr('maxlength');
        
        
        textarea.on('change focus keyup keypress', function (e) {

            // Disable enters in the comments to maintain correct character count
            if (e.keyCode == 13) {
                e.preventDefault();
            }
            else {
                //commentCountDisplay.text((maxLength - textarea.attr('value').length) + ' characters remaining.');
                commentCountDisplay.text((maxLength - textarea.attr('value').length) + ' ' + remainingcharacters);
                textarea.attr('value', (textarea.attr('value')).replace(/(\r\n|\n|\r)/gm, " "));
            }
        });
    }


    /* Password reset form handler -----------------*/

    var passwordReminderForm = $('#passwordreminderlb');

    if (passwordReminderForm.length > 0) {

        passwordReminderForm.on('submit', function (e) {

            // Catch default action
            e.preventDefault();

            // Remove any feedback messages that may be present
            passwordReminderForm.find('.feedback').remove();

            // Disable form inputs
            passwordReminderForm.find(':input').attr('disabled', 'disabled');

            // Grab form data
            var data = getFormData($('#passwordreminderlb :input'));

            // Post data to server
            var nuAjax = $.ajax({
                type: "POST",
                url: appPath + "/ajax/passwordreminder",
                data: data
            });

            // Handle response
            nuAjax.done(function (data, status, jqXHR) {

                // If request successful...
                if (status == 'success') {

                    // Re-enable all inputs
                    passwordReminderForm.find(':input').removeAttr('disabled');

                    // Display feedback message
                    passwordReminderForm.append('<p class="feedback">' + data.message + '</p>');
                }
            });

            nuAjax.fail(failHandler);
        });
    }


    /* Get involved ticker --------------------*/

    var getInvolved = $('.get-involved');

    if (getInvolved.length > 0) {

        setInterval(function () {

            getInvolved.find('ul li:first').slideUp(function () {
                $(this).appendTo(getInvolved.find('ul')).fadeIn();
            });
        }, 2000);
    }


    /* Forgotten password show ----------------*/

    $(document).on('click', '.show-forgot-pass', function (e) {

        // Catch default click
        e.preventDefault();

        $('form#login').fadeOut(function () {
            $('form#passwordreminderlb').fadeIn();
        });
    });

    $(document).on('click', '.show-login', function (e) {

        // Catch default click
        e.preventDefault();

        $('form#passwordreminderlb').fadeOut(function () {
            $('form#login').fadeIn();
        });
    });


    /* Registration lightbox handler ---------------*/
    //Commented by Ajaya for PopUp
//    $('a[href*="registration"]').on('click', function (e) {

//        // Only apply when browser window is of a decent size
//        // Do not open in lightbox if IE9 because it crashes the browser...
//        if (!Modernizr.touch && $(window).width() > 799 && $(window).height() > 649 && (isIE() > 9 || !isIE())) {

//            /*
//            *
//            * NOTE: We could drop an ajax request in
//            * here to determine if the user is already
//            * logged in or not to prevent edge cases
//            * arising when the user hits the back
//            * button after performing a login. If
//            * returns true, fire off redirect to return
//            * url.
//            *
//            *
//            */

//            e.preventDefault();

//            // Create new lightbox
//            var modal = $('<div id="new-modal" class="modal container fade hide" />');

//            // Load forms into modal
//            modal.prepend('<div class="modal-header"><a href="" class="close-filter" data-dismiss="modal" aria-hidden="true">&times;</a></div>');

//            modal.on('hidden', function (e) {

//                // Destroy modal dom object
//                modal.remove();
//            });

//            // Modal shown event handler
//            modal.on('shown', function (e) {

//                // Set up unobtrusive validation for registration form
//                $.validator.unobtrusive.parse($(".modal form"));
//            });


//            var appender = ($(this).attr('href').indexOf("?") == -1) ? '?iframe=true' : '&iframe=true';

//            modal.append('<iframe src="' + $(this).attr('href') + appender + '" frameborder="0" scrolling="no"></iframe>').modal();
//        }
//    });


    /* Ajax registration form handlers -------------*/

    $(document).on('submit', '#registration', function (e) {

        // Catch default action
        e.preventDefault();

        var form = $(this);

        if (form.valid()) {

            var nuAjax = $.ajax({
                type: $(this).attr('method'),
                url: appPath + '/ajax/registration/index',
                data: $(this).serialize()
            });

            // Remove any feedback messages that may be present
            form.find('.feedback').remove();

            // Disable all inputs
            form.find(':input').attr('disabled', 'disabled');

            // Handle response
            nuAjax.done(function (data, status, jqXHR) {

                // If request successful...
                if (status == 'success') {

                    var ct = jqXHR.getResponseHeader("content-type") || "";

                    // Handle json style response - only really used when we have a redirect parameter to be fired after the first step of login
                    if (ct.indexOf('json') > -1) {

                        if (data.success) {

                            var vps;

                            // Check what stage of the registration we're onW
                            switch (form.find('input[name="Stage"]').val()) {

                                case 'register':
                                    vps = 'virtual/registration/completed';
                                    break;

                                case 'preferences':
                                    vps = 'virtual/registration/preferences';
                                    break;

                                case 'profile':
                                    vps = 'virtual/registration/profile';
                                    break;

                                default:
                                    vps = '';
                            }

                            if (vps != '') {

                                _tracking.trackpageview(vps);
                            }

                            // Fire action complete tracking
                            var windowHref = window.location.href.toLowerCase();

                            // Check we're on the registration page first of all
                            if (windowHref.indexOf('registration') > 0) {

                                if (windowHref.indexOf('action') > 0) {

                                    var intendedAction = windowHref.match(/action=(.*?)&/)[1].replace(/\+/g, ' '), category;

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
                                                action: 'Post Login',
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
                            //set timeout gives the tracking time to run before redirect
                            setTimeout(function () {
                                // Login successful, redirect if present - reload otherwise
                                if (self != top) {
                                    top.window.location.href = (data.redirect != "") ? data.redirect : top.window.location.href;
                                } else {
                                    window.location.href = (data.redirect != "") ? data.redirect : window.location.href;
                                }
                            }, 500);
                        }
                    } else {

                        var dataObj = $(data).filter('.register');

                        var vps;

                        // Check what stage of the registration we're onW
                        switch (form.find('input[name="Stage"]').val()) {

                            case 'register':
                                vps = 'virtual/registration/completed';
                                break;

                            case 'preferences':
                                vps = 'virtual/registration/preferences';
                                break;

                            case 'profile':
                                vps = 'virtual/registration/profile';
                                break;

                            default:
                                vps = '';
                        }

                        if (vps != '') {

                            _tracking.trackpageview(vps);
                        }

                        // Grab new markup from response and swap out
                        $('#main .register').html(dataObj.html());

                        //cc-1672 - scroll back to top of page after each submit (raised as iPad issue)
                        //COATSCRAFTSWARRANTY-48 - wasn't working on windows mobile, moved and added html
                        if (!$('.register').parents('iframe').length > 0) {
                            $('html,body').scrollTop(0);
                        }

                        // Reinitalise Google maps geocoder
                        initRegPostcodeGeocoder();

                        if (self != top) {
                            top.resetModalHeight($('body #wrap').height());
                        }
                    }
                }
                else {
                    form.append('<p class="feedback">Sadly, we couldnt log you in this time, please try again.</p>');
                }
            });


            //cc-1605 - IE8 fix for map error on ajax
            $('#geolocation-canvas').remove();


            nuAjax.fail(failHandler);
        }
    });

    $(document).on('click', '#registration input[type="submit"]', function (e) {

        // HACK - Using set timeout to make sure we get the height after the validation messages have shown. Set timeout with 0 moves to base of call stack.
        if (self != top) {
            setTimeout(function () {
                top.resetModalHeight($('body #wrap').height())
            }, 0)
        }
    });


    /* Login form event handler -------------------*/

    $(document).on('submit', '#login', function (e) {

        // Catch default action
        e.preventDefault();

        if ($('form#login').valid()) {

            var form = $(this);

            var nuAjax = $.ajax({
                type: $(this).attr('method'),
                url: appPath + '/ajax/registration/index',
                data: $(this).serialize()
            });

            // Remove any feedback messages that may be present
            form.find('.feedback').remove();

            // Disable all inputs
            form.find(':input').attr('disabled', 'disabled');

            // Handle response
            nuAjax.done(function (data, status, jqXHR) {

                // If request successful...
                if (status == 'success') {

                    // Login successful, redirect if present - reload otherwise
                    if (data.success) {

                        _tracking.trackpageview('virtual/login/completed');

                        // Fire action complete tracking
                        var windowHref = window.location.href.toLowerCase();

                        // Check we're on the registration page first of all
                        if (windowHref.indexOf('registration') > 0) {

                            if (windowHref.indexOf('action') > 0) {

                                var intendedAction = windowHref.match(/action=(.*?)&/)[1].replace(/\+/g, ' '), category;

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
                                            action: 'Post Login',
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
                        //set timeout gives the tracking time to run before redirect
                        setTimeout(function () {
                            // Login successful, redirect if present - reload otherwise
                            if (self != top) {
                                top.window.location.href = (data.redirect != "") ? data.redirect : top.window.location.href;
                            } else {
                                window.location.href = (data.redirect != "") ? data.redirect : window.location.href;
                            }
                        }, 500);
                    }

                    // Unsuccessful login, show error message
                    if (!data.success) {

                        form.append('<p class="feedback"><span class="field-validation-error">' + data.message + '</span></p>');

                        // Re-enable all inputs
                        form.find(':input').removeAttr('disabled');
                    }
                }
                else {
                    form.append('<p class="feedback">Sadly, we couldnt log you in this time, please try again.</p>');
                }
            });

            nuAjax.fail(failHandler);
        }
    });


    /* Simple ajax form submission event handlers --*/
    // Fires custom events when successful the name of which is pulled from a data attribute on the form

    var ajaxForms = $('form[rel="ajax"]');

    if (ajaxForms.length > 0) {

        ajaxForms.on('submit', function (e) {

            e.preventDefault();

            var form = $(this),
                button = form.find('button[type="submit"]'),
                doneEvent = form.data('done-event');

            if (form.valid()) {

                var data = form.serialize();

                // Fire ajax request
                var ajax = $.ajax({
                    type: 'POST',
                    url: form.attr('action'),
                    data: data
                });

                // Handle successful requests
                ajax.done(function (data, status, jqXHR) {

                    if (status == "success") {

                        // Reset button status
                        button.removeClass('refresh').removeAttr('disabled');

                        // Fire global done event as defined in data attribute
                        if (typeof doneEvent != 'undefined') {

                            // Trigger done event for toasts etc
                            $(document).trigger(doneEvent);
                        }
                    }
                });

                // Define failure handler
                ajax.fail(failHandler);
            }
        });
    }


    /* Post a rating form event handler -------------------*/

    $(document).on('click', '#rateIt', function (e) {

        // Catch default action
        e.preventDefault();
        if ($('#ratingOption').attr('value') == '0') {
            $('#PostARatingValidationMessage').removeClass('hide');
        } else {
            $('.rating-form').submit();
        }
    });


    /* Store search form -----------------------*/

    var storeSearchForm = $('.store-finder');

    if (storeSearchForm.length > 0) {

        // Create Coats Google Geocoder
        $('#Location', storeSearchForm).coatsGoogleMapper({
            mapSelector: '.googlemap #mapcanvas',
            btnSelector: '#btnSearch',
            optSelector: '#options',
            frmSelector: '#store-locator-form',
            fbkSelector: '',
            latSelector: '#Latitude',
            lngSelector: '#Longitude',
            showControls: true,
            allowRefinement: true,
            markersJSON: retailersJSON,
            centerOnBlur: false,
            mapOptions: { zoom: 5 },
            fbkObj: $('.store-locator-js-error'),
            //zoom: '#Zoom',
            //scrollable: !$('html').hasClass('touch'),
            draggable: !$('html').hasClass('touch')
        });
    }

    /* Footer scripts ------------------------------*/

    $('#base .country span').on('click', function () {
        $('#base .country ul').toggle();
    });


    $('.user-panel .lightbox:has(img)').coatsGalleryModal();

    var newsletterFormReg = $('.newsletter form.regsign');
    //Commented by Ajaya for Popup at footer
//    if (newsletterFormReg.length > 0) {

//        newsletterFormReg.on('submit', function (e) {

//            // IE9 bug means that we can't use the lightbox for registering
//            if ($(window).width() > 799 && $(window).height() > 649 && ie != 9) {

//                e.preventDefault();

//                var datas = newsletterFormReg.serialize();

//                // Create new lightbox
//                var modal = $('<div id="new-modal" class="modal container fade hide" />');

//                // Load forms into modal
//                modal.prepend('<div class="modal-header"><a href="" class="close-filter" data-dismiss="modal" aria-hidden="true">&times;</a></div>');

//                modal.on('hidden', function (e) {

//                    // Destroy modal dom object
//                    modal.remove();
//                });

//                modal.append('<iframe src="' + newsletterFormReg.attr('action') + '?iframe=true&' + datas + '" id="lboxer" frameborder="0" scrolling="no"></iframe>').modal();
//            }
//        });
//    }

    var newsletterFormLog = $('.newsletter form.update');

    if (newsletterFormLog.length > 0) {

        newsletterFormLog.on('submit', function (e) {

            e.preventDefault();

            var data = getFormData(newsletterFormLog.find(':input'));

            var ajaxxy = $.ajax({
                url: appPath + '/ajax/profile/emailnewslettersettings',
                method: "POST",
                data: data
            });

            newsletterFormLog.find(':input').attr('disabled', 'disabled');

            var message = "";

            ajaxxy.done(function (data, status, jqXHR) {

                // If request successful...
                if (status == 'success') {

                    message = data.message;

                    if (data.success) {

                        // Reload iframes if open
                        if ($('iframe#panels').is(':visible')) {

                            $('iframe#panels').attr('src', function (i, val) { return val; });
                        }
                    }
                }
            });
            ajaxxy.fail(failHandler);
            ajaxxy.always(function () {

                if (message != "") {
                    top.toast(message);
                }

                newsletterFormLog.find(':input').removeAttr('disabled');
            });
        });
    }

    // equaliseHeights($('#base .span3 .inner'));


    /* Show/hide comments ---------------------*/

    var userComments = $('.component.comments');

    if (userComments.length > 0) {

        var articles = userComments.find('article'),
			articleCount = articles.length,
			defaultDisplay = 3,
			showMoreText = window.Resources.ViewAllComments + " (" + articles.length + ")",
			showLessText = window.Resources.ViewLessComments,
			moreLessBtn = $('<a title="" class="btn pink nomargin">' + showMoreText + '</a>'),
			toggleableArticles = articles.filter(':gt(' + (defaultDisplay - 1) + ')');

        if (articleCount > defaultDisplay) {
            // Add button after last article
            articles.last().after(moreLessBtn);

            // Hide toggleable articles
            toggleableArticles.hide();

            // Bind event handler to button
            moreLessBtn.on('click', function (e) {

                e.preventDefault();

                // Toggle toggleable articles
                toggleableArticles.toggle();

                // Switch text out dependent upon state
                moreLessBtn.text(moreLessBtn.text() == showMoreText ? showLessText : showMoreText);
            });
        }
    }


    /* Show/hide designer comments ------------*/

    var desComments = $('.recent-activity .comments');

    if (desComments.length > 0) {

        var articles = desComments.find('article'),
			articleCount = articles.length,
			defaultDisplay = 2,
			showMoreText = window.Resources.ViewAllComments + " (" + articles.length + ")",
			showLessText = window.Resources.ViewLessComments,
			moreLessBtn = $('<a title="" class="btn pink" style="">' + showMoreText + '</a>'),
			toggleableArticles = articles.filter(':gt(' + (defaultDisplay - 1) + ')');

        if (articleCount > defaultDisplay) {

            // Add button after last article
            articles.last().after(moreLessBtn);

            // Hide toggleable articles
            toggleableArticles.hide();

            // Bind event handler to button
            moreLessBtn.on('click', function (e) {

                e.preventDefault();

                // Toggle toggleable articles
                toggleableArticles.toggle();

                // Switch text out dependent upon state
                moreLessBtn.text(moreLessBtn.text() == showMoreText ? showLessText : showMoreText);
            });
        }
    }


    /* Initialise Registration Google Map ----------*/

    var registrationSection = $('.register');

    if (registrationSection.length > 0) {
        initRegPostcodeGeocoder();
    }


    /* Set up shopping list table -------------*/

    $(document).on('click', '.increase-quantity', function () {
        $.ajax({
            type: "POST",
            context: $(this).parents('td'),
            url: appPath + "/shoppinglist/changequantity",
            data: { id: $(this).siblings('#id').attr('value'), quantityButton: 'increase' }
        }).done(function (data) {
            if (data.success == true) {
                var newQuantity = zeroPad(data.data.Quantity, 2);

                if (data.data.Quantity == 0) {
                    $(this).parents('tr').find('button.delete').trigger('click');
                } else {
                    $(this).children('.quantity').html(newQuantity);
                }
            }
        }).fail(failHandler);
        return false;
    });

    $(document).on('click', '.decrease-quantity', function () {
        $.ajax({
            type: "POST",
            context: $(this).parents('td'),
            url: appPath + "/shoppinglist/changequantity",
            data: { id: $(this).siblings('#id').attr('value'), quantityButton: 'decrease' }
        }).done(function (data) {
            if (data.success == true) {
                var newQuantity = zeroPad(data.data.Quantity, 2);

                if (data.data.Quantity == 0) {
                    $(this).parents('tr').find('button.delete').trigger('click');
                } else {
                    $(this).children('.quantity').html(newQuantity);
                }

            }
        }).fail(failHandler);
        return false;
    });

    $(document).on('submit', '#emailshoppinglist', function (e) {

        $.ajax({
            type: "POST",
            url: $(this).attr('action'),
            //url: appPath + "/shoppinglist/EmailShoppingList",
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function (data) {
            if (data.success == true) {
                var message = window.Resouces.ShoppingListEmailSent;

                if (top === self) {
                    toast(message);
                } else {
                    top.toast(message);
                }
            }
        }).fail(failHandler);
        return false;
    });


    /* Cookies bar ----------------------------*/

    var cookiesBar = $('#cookies');

    // Check if the bar is present on page
    if (cookiesBar.length > 0) {

        // Set up event handlers
        cookiesBar.find('a.continue').on('click', function (e) {
            e.preventDefault();

            cookiesBar.slideUp(function () {
                cookiesBar.remove();
            });
        });
    }


    /* Geolocation canvas initialiser --------------*/
    var profile = $('#profile');

    if (profile.length > 0) {

        var cgmSettingsObj = {
            mapSelector: '#geolocation-canvas',
            btnSelector: '#find',
            optSelector: '#options',
            fbkSelector: '#feeder',
            latSelector: '#lat',
            lngSelector: '#long',
            zoom: 11
        };

        var centerLat = $('#centerOnLat').val();
        var centerLng = $('#centerOnLng').val();
        if (centerLat && centerLng) {
            cgmSettingsObj.mapOptions = { center: new google.maps.LatLng(centerLat, centerLng) };
        }
        // Create Coats Google Geocoder
        $('#AddressDetails_Postcode').coatsGoogleMapper(cgmSettingsObj);

        var status = $('#profileStatus').val();

        if (status == "saved") {
            toast(window.Resources.ProfileChangesSaved);
        }

        if (status == "error") {
            toast(window.Resources.ProfileError);
        }

        // Reset validation messages on profile form when focus is on the NewPassword field
        $('#profile').find('#NewPassword').focus(function () {
            $('#profile').resetValidation();
        });

    }


    var registrationForm = $('#registration');

    if (registrationForm.length > 0) {
        if ($('#RegistrationForm_CustomerDetails_EmailAddress').val() != '' && $('#RegistrationForm_CustomerDetails_EmailAddress').val() != window.Resources.EmailAddress) {
            //trigger validation on the email address field if it's loaded in from newsletter sign up
            $('#RegistrationForm_CustomerDetails_EmailAddress').valid()
        }

        // Reset validation messages on register form when focus is on login
        $('#login').find('#LoginForm_EmailAddress').focus(function () {
            $('#registration').resetValidation();
        });

        // Reset validation messages on register form when focus is on login
        $('#login').find('#LoginForm_Password').focus(function () {
            $('#registration').resetValidation();
        });

        // Reset validation messages on login form when focus is on registration
        $('#registration').find('#RegistrationForm_CustomerDetails_EmailAddress').focus(function () {
            $('#login').resetValidation();
        });
    }


    /* Dropdown panel -------------------------*/

    var panel = $('#dropdown'),
		iframe = panel.find('iframe'),
		closer = panel.find('.icon.close');

    iframe.height(750);
    panel.height(750);

    // Only apply to no touch devices - iOS iframe height fix
    $('.no-touch #user .ajax.list, .no-touch #user .ajax.scrapbook, .no-touch #user .ajax.profile').on('click', function (e) {

        // Abnother check on screensize so we don't get the dropdown box if the user resizes
        if ($(window).width() > 768) {


            e.preventDefault();

            var target = $(e.target);

            $(window).on('resize', function (e) {

                if ($(window).width() < 768 && panel.is(':visible')) {
                    closer.trigger('click');
                }
            });

            closer.on('click', function (e) {

                panel
				.slideUp(400, 'easeInOutQuart', function () {

				    iframe
						.attr('src', '')
						.hide();

				    closer.hide();

				    panel.removeClass('loading');

				    target.parents('ul').find('li a').removeClass('current');
				    target.parents('ul').find('li').removeClass('current');
				    target.parents('#menu').removeClass('user-menu-open');

				    var lias = target.parents('ul').find('li a');
				    $(lias).each(function () {
				        if ($(window).width() > 768) {
				            $(this).tooltip({ placement: 'bottom' });
				        }
				    })
				});
            });

            if (target.hasClass('current')) {

                panel
				.slideUp(400, 'easeInOutQuart', function () {

				    iframe
						.attr('src', '')
						.hide();

				    closer.hide();

				    panel.removeClass('loading');

				    target.parents('ul').find('li a').removeClass('current');
				    target.parents('ul').find('li').removeClass('current');
				    target.parents('#menu').removeClass('user-menu-open');
				    var lias = target.parents('ul').find('li a');
				    $(lias).each(function () {
				        if ($(window).width() > 768) {
				            $(this).tooltip({ placement: 'bottom' });
				        }
				    })
				});
            } else {

                panel.addClass('loading');

                // Check if not already visible
                if (panel.not(':visible')) {

                    panel
					.stop()
					.slideDown(400, 'easeInOutQuart', function () {
					    iframe
							.attr('src', target.attr('href') + '?iframe=true')
							.on('load', function (e) {
							    closer.show();
							    iframe.fadeIn(400);
							    panel.removeClass('loading');

							    if (ie && ie == 9) {
							        /* Last resort hideous fix for IE9 for leaking media query css */
							        if (!getParameterByName(document.URL, "iframe") == true) {
							            if (!($(window).width() > 768 && $(window).width() < 960)) {
							                $('#menu', window.parent.document).addClass('ie9');
							            }
							        }
							    }
							});
					});
                }
                else {

                    iframe
					.attr('src', target.attr('href') + '?iframe=true')
					.on('load', function (e) {

					    panel.removeClass('loading');
					});
                }
            }

            target.parents('ul').find('li a').removeClass('current');
            target.parents('ul').find('li').removeClass('current');
            target.addClass('current');
            target.parents('#menu').addClass('user-menu-open');
            target.parents('li').addClass('current');
            var lias = target.parents('ul').find('li a');
            $(lias).each(function () {
                $(this).tooltip('destroy');
            })
        }
    });

    /* Update profile link in footer - CCPLCR-124 */
    $('.no-touch .newsletter .ajax.profile').on('click', function (e) {
        e.preventDefault();

        $("html, body").animate({ scrollTop: 0 }, 600);

        if (!$('#menu a.profile').hasClass('current')) {
            $('#menu .profile').trigger('click');
        }

    });

    /* Misc ----------------------------------------*/

    // Hide all no-js elements
    $('.no-js').hide();


    //    Commented out on 25/09/13 - No references to this in the markup, likely not required.
    //    /* Normalise product table heights */
    //    var tables = $('.product-table table');

    //    if (tables.length > 0) {

    //        var height = 0;

    //        for (var i = 0; i < tables.length; i++) {

    //            var tableHeight = $(tables[i]).outerHeight();

    //            if (tableHeight > height) {
    //                height = tableHeight;
    //            }
    //        }

    //        tables.height(height);
    //    }

    var menu = $('#head #menu'),
		showHide = $('#showhide');

    showHide.on('click', showHideMenu);

    function showHideMenu(e) {

        e.preventDefault();

        menu.toggle(0, function () {
            showHide.toggleClass('visible');
        });
    }

    /*
    * 
    * Only apply this logic to desktop and no touch
    * devices. Touch devices seem to fire resize
    * events on scroll which closes the menu if it's
    * open.
    * 
    */
    if ($('html').hasClass('no-touch')) {

        $(window).on('resize', function (e) {
            // Show normal menu as we go back to desktop size
            if ($(window).width() > 768) {
                menu.show();
                showHide.addClass('visible');
            }

            if ($(window).width() <= 768) {
                menu.hide();
                showHide.removeClass('visible');
            }
        });
    }


    /*
    *
    * Bind the History adaptor to the window
    * so the back button is enabled for ajax updates 
    *
    */
    window.stateChangeIsLocal = false;

    History.Adapter.bind(window, 'statechange', function () {

        //is this from a back or forward button?
        if (!window.stateChangeIsLocal) {
            var data = History.getState().data;
            if (data) {
                var url = data.url;
                var ajaxed = isotopeHelper.ajaxer({ dataType: 'html', type: 'GET', url: url });
            }
        } else {
            window.stateChangeIsLocal = false;
        }
    });



});

// Return form data
function getFormData(inputs) {
	//option to use
	// var data = $(this).serialize
	//if you need more control  then
	// get all the inputs into an array.
	// get an associative array of just the values.
	var data = {};

	inputs.each(function () {
		if (this.type == "radio") {

			if ($(this).attr("checked")) {
				data[this.name] = $(this).val()
			}
		} else if (this.type == "checkbox") {

			if ($(this).attr("checked") == "checked") {
				if (data[this.name] == null) {
					data[this.name] = "";
				}

				if (data[this.name] != "") {
					data[this.name] += "," + $(this).val();
				} else {
					data[this.name] = $(this).val();
				}
			}
		}
		else {
			//mvc adds hidden field for checkboxes with same name so dont overwrite
			if (data[this.name] == null)
				data[this.name] = $(this).val();
		}
	});
	return data;
}


function toast(message) {

	var toast,
		toastTimer = false,
		closeLink = '<a href="' + window.location.href + '" title="Close message box">&times;</a>',
		messageText = '<p>' + message + '</p>';

	if ($('#feedback').length > 0) {

		clearTimeout(toastTimer);

		toast = $('#feedback');

	} else {

		toast = $('<div id="feedback" />');
	}

	toast
		.hide()
		.html(closeLink + messageText)
		.prependTo('body')
		.show();

	toastTimer = setTimeout(function () {
		toast.remove();
	}, 5000);

}

function getParameterByName(url, name) {
	name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
		results = regex.exec(url);
	return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

/* Loading pop up functionality ---------------*/

// Create popup
var loadPopup = $('<div id="nu-overlay"><div id="box"><p>' + window.Resources.LoadingYourContent + '</p></div></div>');

// Show it
function showLoadPopup() {
    // Add loading box to dom
    loadPopup.hide().appendTo('body').fadeIn();
}

// Hide it
function hideLoadPopup() {
    loadPopup.fadeOut(function () {
        loadPopup.fadeOut;
    });
}

//IE version detection
var ie = (function () {
    var undef,
    v = 3,
    div = document.createElement('div'),
    all = div.getElementsByTagName('i');
        while (
    div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->',
    all[0]
    );
    return v > 4 ? v : undef;
} ());


function isIE () {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
}

function openPinIterest(url) {
	if ($('html').hasClass('touch')) {
		window.open(url);
	} else {
		window.open(url, '', 'width=600,height=340,toolbar=0,menubar=0,location=0');
	}
}