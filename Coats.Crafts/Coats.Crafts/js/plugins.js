/*

Console Fixer
- Overview: Prevent console errors on unsupported browsers

*/

(function () {
	var method;
	var noop = function () { };
	var methods = [
		'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
		'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
		'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
		'timeStamp', 'trace', 'warn'
	];
	var length = methods.length;
	var console = (window.console = window.console || {});

	while (length--) {
		method = methods[length];

		// Only stub undefined methods.
		if (!console[method]) {
			console[method] = noop;
		}
	}
} ());



/*

Coats gallery modal plugin
- Overview: Constructs lightboxes for a given object.
- Author: JC
- Date: 27/06/13
- Edited: 27/06/13

*/

(function ($) {

	$.fn.coatsGalleryModal = function (opts) {

		// Declare variables
		var object = $(this),
			settings = {},
			imagesArr = new Array(),
			open = false;
			modal = null;

		var methods = {

			init: function () {

				// Maintain reference to methods object
				var that = this;

				that.bindEvents();

				// Build images array - used as a lookup when we click on an image, matching indexs
				object.each(function() {
				
					var link = $(this);

					imagesArr.push(link.attr('href'));
				});
			},

			bindEvents: function () {

				// Maintain reference to methods object
				var that = this;

				object.on('click', function (e) {

					if($(window).width() > 768) {

						e.preventDefault();

						// Trigger modal to show and pass through index of image to show
						that._showModal(object.index(this));
					}
				});

				$(window).on('resize', function (e) {

					if(($(window).width() < 768) && open) {
						that._hideModal();
					}
				});
			},

			_showModal: function (index) {

				// Maintain reference to methods object
				var that = this;

				if(!open) {

					modal = parent.$('<div id="new-modal" class="modal container fade hide gallery" />');

					// Display header
					modal.prepend('<div class="modal-header"><a href="" class="close-filter" data-dismiss="modal" aria-hidden="true">&times;</a></div>');
					modal.append('<div class="controls"><a class="icon left">Prev</a><a class="icon right">Next</a></div>');

					var images = $('<ul />');

					for (var i = 0; i < imagesArr.length; i++) {

						var listItem = $('<li class="' + ((i == index) ? 'current' : '') + '"><img src="' + imagesArr[i] + '" /></li>');

						images.append(listItem);
					}

					// Display image
					modal.append(images);
					modal.on('hidden', function (e) {

						// Destroy modal dom object
						modal.remove();

						open = false;
					});

					modal.modal();

					that._setUpCarouselFunctionality();

					open = true;
				}
			},

			_hideModal: function () {
				modal.modal('hide');
			},

			_setUpCarouselFunctionality: function () {

				modal.find('.icon').on('click', function (e) {

					e.preventDefault();

					var object = $(this),
						current = modal.find('.current');

					if(object.hasClass('left')) {

						if(current.prev().is('li')) {
							current.removeClass('current').prev('li').addClass('current');
						}
						else
						{
							modal.find('li').removeClass('current').last().addClass('current');
						}
					}

					if(object.hasClass('right')) {
						if(current.next().is('li')) {
							current.removeClass('current').next('li').addClass('current');
						}
						else 
						{
							modal.find('li').removeClass('current').first().addClass('current');
						}
					}
				});
			}
		}


		// Merge options with default settings
		settings = $.extend(settings, opts);

		// Initialise plugin
		methods.init();
	};
})(jQuery);



/*

Coats Docked Side Bars
- Overview: Positions side bar elements
- Author: JC
- Date: 15/07/13
- Edited: 15/07/13

*/

(function( $ ){

	$.fn.coatsDockSideBar = function(opts) {

		return this.each(function() {

			// Declare variables
			var object = $(this),
				settings = {
					offset : 40, // Offset from top
                    fixedOffset: 40
				};

			var methods = {
				init : function () {

					var that = this;

                    
					if ($('.crumbs').length > 0) {
						settings.offset += $('.crumbs').outerHeight();
					}
					if ($('.product-nav').length > 0) {
						settings.offset += $('.product-nav').outerHeight();
					}
					if ($('.author-rating').length > 0) {
						settings.offset += $('.author-rating').outerHeight();
					}
					if ($('.row-fluid.headers:eq(0)').length > 0) {
						settings.offset += $('.row-fluid.headers:eq(0)').outerHeight(false);
					}
					if ($('.store-locator').length > 0) {
						settings.offset += $('.store-locator').outerHeight(false);
					}
                    
                    if ($(window).width() > 768 && !$('html').hasClass('touch')) {
					    object.css({ top: settings.offset });
                    }
                    

					that.bindEventHandlers();
				},
				bindEventHandlers : function () {

					// On scroll set position of share bar
					$(window).on('scroll resize', function () {

						    if ($(window).width() > 768 && !$('html').hasClass('touch')) {

                                // Needs to be in place as default height is set on dropdown div which is picked up
                                var dropDownHeight = ($('#dropdown').is(':visible') ? $('#dropdown').outerHeight() : 0);

							    // Start dock position
							    var positionDock = settings.offset + $('#head').outerHeight() + dropDownHeight,
								    scrollPosition = $(document).scrollTop();

							    // Start fix position
							    var positionFix = $('#base').position().top - object.outerHeight(true) - 40;

							    var pos,
								    topPos,
								    bottomPos;

							    // Set CSS positioning based on scroll position
							    if (scrollPosition < positionDock) {

								    pos = 'absolute';
								    topPos = settings.offset + 'px';
								    bottomPos = 'auto';
							    }

							    if (scrollPosition >= positionDock && scrollPosition < positionFix) {

								    pos = 'fixed';
								    topPos = settings.fixedOffset + 'px';
								    bottomPos = 'auto';
							    }
							    if (scrollPosition >= positionFix) {

								    pos = 'absolute';
								    topPos = 'auto';
								    bottomPos = '0px';
							    }

							    object.css({ position: pos, top: topPos, bottom: bottomPos });
						    }
					});
				}
			}


			// Merge options with default settings
			settings = $.extend(settings, opts);

			// Initialise plugin
			methods.init();
		});
	};
})(jQuery);


/*

Coats stylised select element plugin
- Overview: Constructs unordered lists from select options that when clicked will set the corresponding option as selected. Allows for improved styling of form elements.
- Author: JC
- Date: 27/06/13
- Edited: 27/06/13

*/

(function( $ ){

	$.fn.coatsDropdown = function(opts) {

		return this.each(function() {

			// Declare variables
			var object = $(this),
				settings = {};

			var methods = {

				init : function () { 

					// Maintain reference to methods object
					var that = this;

					// Define variables
					var options = object.find('option'),
						list = $('<ul class="dropdown" />');

					// Loop over all options in select and build list
					for (var i = 0; i < options.length; i++) {
						var option = options[i];

						list.append('<li data-value="' + option.attr('value') + '">' + option.text() + '</li>')
					}

					// Hide dropdown and insert stylised dropdown after it
					object
						.hide()
						.after(list);

					that.bindEvents();
				},

				bindEvents : function () {
					
					// Bind click event handlers to ilst items
					$(document).on('click', 'ul.dropdown li', function (e) {

						// Catch the click
						e.preventDefault();

						var item = $(this),
							index = item.index();

						// Find the corresponding item in the nearest select element and set as selected
						item
							.parents('ul')
							.prev('select')
							.find('option')
							.eq(index)
							.attr('selected', 'selected');
					});
				}
			}


			// Merge options with default settings
			settings = $.extend(settings, opts);

			// Initialise plugin
			methods.init();
		});
	};
})(jQuery);


/*

Coats slider plugin
- Overview: Construct an animated slider
- Author: JC
- Date: 21/03/13
- Edited: 18/06/13

- TODO - On resize, check screen size and set arrow pos to base and arrow size to small when relevent

*/

(function( $ ){

	$.fn.coatsSlider = function(opts) {

		return this.each(function() {
			
			// Declare variables
			var object = $(this),
				settings = {
					arrowSize: 'normal',
					controlPos: 'inside',
					showArrows: true,
					showPips: true,
					arrowPos: 'base',
					easing: 'easeInOutQuart',
					speed: 1000
				},
				width       = object.width(),
				outer       = object.find('.outer'),
				inner       = object.find('.inner'),
				panels      = object.find('.new-panel'),
				controls,
				pips,
				pipListItems,
				pipListItemsLinks,
				prevArr,
				nextArr,
				sliderInt,
				current;


			var methods = {
				init : function () { 

					// Maintain reference to methods object
					var that = this;

					// Check we've got more than one
					if(panels.length > 1) {

						// Control position
						var dataControlPos = object.data('slider-control-position');

						if(typeof dataControlPos != 'undefined') {

							settings.controlPos = dataControlPos;
							object.addClass(settings.controlPos);
						}

						// Set arrow position class
						var dataArrowPos = object.data('slider-arrow-position');

						if(typeof dataArrowPos != 'undefined') {

							settings.arrowPos = dataArrowPos;
							object.addClass(settings.arrowPos);
						}

						// Set arrow sizes class
						var dataArrowSize = object.data('slider-arrow-size');

						if(typeof dataArrowSize != 'undefined') {

							settings.arrowSize = dataArrowSize;
							object.addClass(settings.arrowSize);
						}

						// Check if we need to build controls div
						if(settings.showArrows || settings.showPips) {

							// Dynamically build controls div
							controls = $('<div class="new-controls" />');

							// Hacking in home page slider functionality
							if(outer.length > 0) {

								// Append to container outer div
								controls.appendTo(outer);
							} else {

								// Append to container inner div
								controls.appendTo(inner);
							}
						}


						// Check if we're supposed to show pips
						if(settings.showPips) {

							// Build pip controls
							pips = $('<ul class="pips" />');

							// Generate pip links for every panel
							for (var i = 0; i < panels.length; i++) {

								pips.append('<li><a class="pip">To Panel ' + (i + 1) + '</a></li>');
							}

							// Append to controls div
							pips.appendTo(controls);

							var pipElements = $('.pip', pips);

							// Position pips
							pips.css({
								'position' : 'absolute',
								'left' : '50%',
								'margin-left' : - ((pipElements.first().parent().width() * pipElements.length) / 2)
							});

							pipListItems = pips.find('li');
							pipListItemsLinks = pips.find('li a');
						}


						// Check if we're supposed to show arrows
						if(settings.showArrows) {

							// Build arrow controls
							prevNext = $('<ul class="prevnext"><li><a class="prev" rel="prev"></a></li><li><a title="" class="next" rel="next"></a></li></ul>');
							prevArr = prevNext.find('.prev');
							nextArr = prevNext.find('.next');

							// Append to controls div
							prevNext.appendTo(controls);
							that.setPositions();
						}

						// Set panels width
						panels.show().width(width);

						// Set inner div width
						inner.width(panels.outerWidth(true) * panels.length);

						// Set up event handlers
						that.bindEvents();

						// Set first panel as current
						pips.find('li:eq(0) a').trigger('click');

						// Set up timed slider
						sliderInt = setInterval(function () {

							// Trigger click on to next panel link
							nextArr.trigger('click');
						}, 5000);
					}
				},
				setPositions : function () {

                    var windowWidth = $(window).width();

					// Check if we're supposed to show arrows
					if(settings.showArrows) {
						
						// Position arrows with computed CSS
						var objectCss   = {},
							prevArrCss  = {},
							nextArrCss  = {},
							pipsCss     = {};

						if(settings.controlPos == 'inside') {
						
							objectCss = {
								'padding' : '0 0 0 0'
							}
						}

						if(settings.controlPos == 'outside') {

							 objectCss = {
								'padding' : '0 0 56px 0'
							}
						}
						
						if(settings.controlPos == 'outside-close') {

							 objectCss = {
								'padding' : '0 0 18px 0'
							}
						}
						if(settings.arrowPos == 'base') {

							prevArrCss = {
								'bottom' : '0px',
								'position' : 'absolute',
								'right' : '50%',
								'margin-right' : (pips.width() / 2) + 20
							}

							nextArrCss = {
								'bottom' : '0px',
								'position' : 'absolute',
								'left' : '50%',
								'margin-left' : (pips.width() / 2) + 20
							}

							if(settings.controlPos == 'inside') {
						
								pipsCss = {
									'margin-bottom' : '17px'
								}
							}
						}

						if(settings.arrowPos == 'side') {

							prevArrCss = {
								'position' : 'absolute',
								'left' : '20px',
								'margin-right' : '0',
								'margin-top' : - (prevArr.height() / 2) + 'px',
								'top' : '35%',
								'right' : 'auto',
								'bottom' : 'auto'
							}
								
							nextArrCss = {
								'position' : 'absolute',
								'right' : '20px',
								'margin-left' : '0',
								'margin-top' : - (nextArr.height() / 2) + 'px',
								'top' : '35%',
								'left' : 'auto',
								'bottom' : 'auto'
							}

							if(settings.controlPos == 'inside') {
						
								pipsCss = {
									'margin-bottom' : '17px'
								}
							}
                        }

                        if(settings.arrowPos == 'special-brands') {

                            var calcedMargin = '-' + (prevArr.height() / 2) + 'px';

							prevArrCss = {
	                            'margin-top' : calcedMargin
                            }
								
                            nextArrCss = {
	                            'margin-top' : calcedMargin
                            }


                            if(windowWidth < 960) {

                                var calcedMargin = ((pips.width() / 2) + 20) + 'px';

								prevArrCss = {
		                            'margin-right' : calcedMargin
	                            }

	                            nextArrCss = {
		                            'margin-left' : calcedMargin
	                            }
							}

							if(settings.controlPos == 'inside') {
						
								pipsCss = {
									'margin-bottom' : '17px'
								}
							}
						}

						if(settings.arrowPos == 'special-home') {

							if(windowWidth < 960) {

								var extraMargin = windowWidth < 360 ? 10 : 20;

								prevArrCss = {
									'margin-right' : (pips.width() / 2) + extraMargin + 'px'
								}

								nextArrCss = {
									'margin-left' : (pips.width() / 2) + extraMargin + 'px'
								}
							}
						}

						object.css(objectCss);
						prevArr.css(prevArrCss);
						nextArr.css(nextArrCss);
						pips.css(pipsCss);
					}
				},
				bindEvents : function () {

					// Maintain reference to methods object
					var that = this;

					// Catch pip clicks
					pips.find('li a').on('click', function (e) {

						// Check if an actual click not triggered
						if (typeof e.originalEvent !== 'undefined') {

							// If so, clear the interval
							clearInterval(sliderInt);
						}

						e.preventDefault();

						// Make sure we're not already sliding
						if(!inner.is(':animated')) {

							var target = $(e.target);
							var index = target.parent('li').index();

							if (!target.hasClass('current')) {
								that.toPanel(index);
							}
						}
					});

					// Catch clicks on the arrows
					controls.find('.prevnext li a').on('click', function (e) {

						e.preventDefault();

						// Check if an actual click not triggered
						if (typeof e.originalEvent !== 'undefined') {

							// If so, clear the interval
							clearInterval(sliderInt);
						}

						// Make sure we're not already sliding
						if(!inner.is(':animated')) {

							var target = $(e.target);

							var intendedIndex = false,
								allowTransition = false;

							// Get the index of the panel we're moving to
							if(target.hasClass('prev')) {
								intendedIndex = current - 1;

								if(intendedIndex < 0) {
									intendedIndex = (panels.length -1);
								}
							}

							if(target.hasClass('next')) {
								intendedIndex = current + 1;

								if (current === (panels.length - 1)) {
									intendedIndex = 0;
								} else {
									intendedIndex = current + 1;
								}
							}

							// Check index is a valid panel
							if($(panels[intendedIndex]).is('.new-panel')) {
								allowTransition = true;
							}

							// Move to panel
							if(allowTransition) {
								that.toPanel(intendedIndex);
							}
						}
					});

					// Reposition panels on windows resize
					$(window).on('resize', function (e) {

						width = object.width();
						that.reset();
					});
				},
				reset : function () {

					// Maintain reference to methods object
					var that = this;

					panels.width(width);
					inner.css({
						'margin-left' : -current * panels.outerWidth(true),
						'width' : panels.length * panels.outerWidth(true)}
					);
					that.setPositions();
				},
				toPanel : function (index) {

					// Reset Remove current classes
					pipListItems.find('a').removeClass('current');
					pips.find('li a').removeClass('current');


					current = index;

					panels.removeClass('current').eq(index).addClass('current');
					pipListItems.eq(index).find('a').addClass('current');

					// Animate position and set current classes when complete
					inner.animate({'margin-left': -index * panels.outerWidth(true)}, settings.speed, settings.easing);
				}
			}

			// Merge options with default settings
			settings = $.extend(settings, opts);

			// Initialise plugin
			methods.init();
		});
	};
})(jQuery);


/*

Coats Google Maps/Geocoder jQuery Plugin
- Overview: Initialise Google Maps Geocoding functionality
- Author: JC/JK
- Created: 06/07/13
- Edited: 19/07/13

*/
var RegionMappings = {
	"ar-sa": "sa",
	"bg-bg": "bg",
	"ca-es": "es",
	"zh-tw": "tw",
	"cs-cz": "cz",
	"da-dk": "dk",
	"de-de": "de",
	"el-gr": "gr",
	"en-us": "us",
	"fi-fi": "fi",
	"fr-fr": "fr",
	"he-il": "il",
	"hu-hu": "hu",
	"is-is": "is",
	"it-it": "it",
	"ja-jp": "jp",
	"ko-kr": "kr",
	"nl-nl": "nl",
	"nb-no": "no",
	"pl-pl": "pl",
	"pt-br": "br",
	"rm-ch": "ch",
	"ro-ro": "ro",
	"ru-ru": "ru",
	"hr-hr": "hr",
	"sk-sk": "sk",
	"sq-al": "al",
	"sv-se": "se",
	"th-th": "th",
	"tr-tr": "tr",
	"ur-pk": "pk",
	"id-id": "id",
	"uk-ua": "ua",
	"be-by": "by",
	"sl-si": "si",
	"et-ee": "ee",
	"lv-lv": "lv",
	"lt-lt": "lt",
	"tg-cyrl-tj": "tj",
	"fa-ir": "ir",
	"vi-vn": "vn",
	"hy-am": "am",
	"az-latn-az": "az",
	"eu-es": "es",
	"hsb-de": "de",
	"mk-mk": "mk",
	"tn-za": "za",
	"xh-za": "za",
	"zu-za": "za",
	"af-za": "za",
	"ka-ge": "ge",
	"fo-fo": "fo",
	"hi-in": "in",
	"mt-mt": "mt",
	"se-no": "no",
	"ms-my": "my",
	"kk-kz": "kz",
	"ky-kg": "kg",
	"sw-ke": "ke",
	"tk-tm": "tm",
	"uz-latn-uz": "uz",
	"tt-ru": "ru",
	"bn-in": "in",
	"pa-in": "in",
	"gu-in": "in",
	"or-in": "in",
	"ta-in": "in",
	"te-in": "in",
	"kn-in": "in",
	"ml-in": "in",
	"as-in": "in",
	"mr-in": "in",
	"sa-in": "in",
	"mn-mn": "mn",
	"bo-cn": "cn",
	"cy-gb": "gb",
	"km-kh": "kh",
	"lo-la": "la",
	"gl-es": "es",
	"kok-in": "in",
	"syr-sy": "sy",
	"si-lk": "lk",
	"iu-cans-ca": "ca",
	"am-et": "et",
	"ne-np": "np",
	"fy-nl": "nl",
	"ps-af": "af",
	"fil-ph": "ph",
	"dv-mv": "mv",
	"ha-latn-ng": "ng",
	"yo-ng": "ng",
	"quz-bo": "bo",
	"nso-za": "za",
	"ba-ru": "ru",
	"lb-lu": "lu",
	"kl-gl": "gl",
	"ig-ng": "ng",
	"ii-cn": "cn",
	"arn-cl": "cl",
	"moh-ca": "ca",
	"br-fr": "fr",
	"ug-cn": "cn",
	"mi-nz": "nz",
	"oc-fr": "fr",
	"co-fr": "fr",
	"gsw-fr": "fr",
	"sah-ru": "ru",
	"qut-gt": "gt",
	"rw-rw": "rw",
	"wo-sn": "sn",
	"prs-af": "af",
	"gd-gb": "gb",
	"ar-iq": "iq",
	"zh-cn": "cn",
	"de-ch": "ch",
	"en-gb": "gb",
	"es-mx": "mx",
	"fr-be": "be",
	"it-ch": "ch",
	"nl-be": "be",
	"nn-no": "no",
	"pt-pt": "pt",
	"sr-latn-cs": "cs",
	"sv-fi": "fi",
	"az-cyrl-az": "az",
	"dsb-de": "de",
	"se-se": "se",
	"ga-ie": "ie",
	"ms-bn": "bn",
	"uz-cyrl-uz": "uz",
	"bn-bd": "bd",
	"mn-mong-cn": "cn",
	"iu-latn-ca": "ca",
	"tzm-latn-dz": "dz",
	"quz-ec": "ec",
	"ar-eg": "eg",
	"zh-hk": "hk",
	"de-at": "at",
	"en-au": "au",
	"es-es": "es",
	"fr-ca": "ca",
	"sr-cyrl-cs": "cs",
	"se-fi": "fi",
	"quz-pe": "pe",
	"ar-ly": "ly",
	"zh-sg": "sg",
	"de-lu": "lu",
	"en-ca": "ca",
	"es-gt": "gt",
	"fr-ch": "ch",
	"hr-ba": "ba",
	"smj-no": "no",
	"ar-dz": "dz",
	"zh-mo": "mo",
	"de-li": "li",
	"en-nz": "nz",
	"es-cr": "cr",
	"fr-lu": "lu",
	"bs-latn-ba": "ba",
	"smj-se": "se",
	"ar-ma": "ma",
	"en-ie": "ie",
	"es-pa": "pa",
	"fr-mc": "mc",
	"sr-latn-ba": "ba",
	"sma-no": "no",
	"ar-tn": "tn",
	"en-za": "za",
	"es-do": "do",
	"sr-cyrl-ba": "ba",
	"sma-se": "se",
	"ar-om": "om",
	"en-jm": "jm",
	"es-ve": "ve",
	"bs-cyrl-ba": "ba",
	"sms-fi": "fi",
	"ar-ye": "ye",
	"es-co": "co",
	"sr-latn-rs": "rs",
	"smn-fi": "fi",
	"ar-sy": "sy",
	"en-bz": "bz",
	"es-pe": "pe",
	"sr-cyrl-rs": "rs",
	"ar-jo": "jo",
	"en-tt": "tt",
	"es-ar": "ar",
	"sr-latn-me": "me",
	"ar-lb": "lb",
	"en-zw": "zw",
	"es-ec": "ec",
	"sr-cyrl-me": "me",
	"ar-kw": "kw",
	"en-ph": "ph",
	"es-cl": "cl",
	"ar-ae": "ae",
	"es-uy": "uy",
	"ar-bh": "bh",
	"es-py": "py",
	"ar-qa": "qa",
	"en-in": "in",
	"es-bo": "bo",
	"en-my": "my",
	"es-sv": "sv",
	"en-sg": "sg",
	"es-hn": "hn",
	"es-ni": "ni",
	"es-pr": "pr",
	"es-us": "us"
};

function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    
    return (myNav.indexOf('msie') != -1) ? true : false;
}
(function ($) {

	$.fn.coatsGoogleMapper = function (opts) {

		return this.each(function () {

			// Declare variables
			var object = $(this),
				settings = {
					mapOptions: {
						backgroundColor: '#E4E1E0',
						center: new google.maps.LatLng(51.51121389999999, -0.11982439999997041),
						disableDefaultUI: true,
						mapTypeId: google.maps.MapTypeId.ROADMAP,
						scrollwheel: false,
						draggable: false,
                        mapTypeControl: true,
                        mapTypeControlOptions: {
                            position: google.maps.ControlPosition.TOP_LEFT
                        },
						zoom: 2
					},
					
					markerOptions: {
						icon: window.appPath + '/img/map-icon-fg-current.png',
						shadow: {
							url: window.appPath + '/img/map-icon-bg.png',
							anchor: new google.maps.Point(10, 27)
						}
					},

					markers: [], // Keep track of the markers on the map

					mapSelector: '#canvas',     // Map canvas
					btnSelector: '#find',       // Find me button
					fbkSelector: '#feeder',     // Feedback message element
					latSelector: "#lat",        // Latitude input selector
					lngSelector: "#long",       // Longitude input selector
					frmSelector: null,
					zoomSelector: '#Distance',
					zoomSelectorOptions: '#Distance option',
					locationSelector: '#Location',
                    WithinValSelector: '#WithinVal',
					optionList: '#optionList li',

					scrollable: false,
					draggable: false,
					showControls: false,

					mapObj: null,
					btnObj: null,
					optObj: null,
					fbkObj: null,
					latObj: null,
					lngObj: null,
					frmObj: null,

					regionBias: null,           // Region bias for location lookups.
					lastBlurRequest: null,

					mapHandler: null,
					encoderHandler: null,

					markersJSON: null,
                    centerOnBlur: true,
                    useGeoLocator: false,
					allowRefinement: false,
					debug: true
				},

			    // Plugin methods
				methods = {
					init: function () {



                        $('.store-locator #NoJS').attr('value', 'false');
						var that = this;
						var setZoom = true;

                        if(settings.useGeoLocator)
                        {
                            if (navigator.geolocation) {
                                navigator.geolocation.getCurrentPosition(function(location){that.mapUserLocation(location, that)});
                            }
                        }

                        object.on("keypress", function(e) {
                        	if ( e.keyCode === 13 ) {
                                if(settings.centerOnBlur)
                                {
								    that.findLocation(function() {								    
                                        object.parents("#store-locator-form").submit();
								    });
                                }
                                else
                                {
								    that.findLocation(function() {								    
                                        object.parents("#store-locator-form").submit();
								    }, true);
                                }
                        		
                        	}
                        })

						// Check and set map object
						if ($(settings.mapSelector).length == 1) {
							settings.mapObj = $(settings.mapSelector);
						} else {
							that.log('Map container not present on page.');
						}

						// Check and set button object
						if ($(settings.btnSelector).length == 1) {
							settings.btnObj = $(settings.btnSelector);
						} else {
							that.log('Find button not present on page.');
						}

						// Check and set search results object
						if ($(settings.optSelector).length == 1) {
							settings.optObj = $(settings.optSelector).hide();
						} else {
							that.log('Location option container not present on page.');
						}

						// Check and set feedback object
						if ($(settings.fbkSelector).length == 1) {
							settings.fbkObj = $(settings.fbkSelector).hide();
						} else {
							that.log('Feedback element not present on page.');
						}
					    
						// Check and set latitude object
						if ($(settings.latSelector).length == 1) {
							settings.latObj = $(settings.latSelector);
						} else {
							that.log('Latitude input not present on page.');
						}

						// Check and set longitude object
						if ($(settings.lngSelector).length == 1) {
							settings.lngObj = $(settings.lngSelector);
						} else {
							that.log('Longitude input not present on page.');
						}
					    
						if ($(settings.frmSelector).length == 1) {
							settings.frmObj = $(settings.frmSelector);
						}
						
						if (!settings.regionBias) {
							var culture = window.appPath.replace('/', '');
							var bias = RegionMappings[culture];
							settings.regionBias = bias ? bias : "us";
						}


                        if ($(settings.WithinValSelector).length > 0){
                            var withinVal = $(settings.WithinValSelector).val()
                            settings.regionBias = withinVal;
                            $(settings.WithinValSelector).on('change', function() {
                                settings.regionBias = $(this).val();
                                if($(settings.locationSelector).val().length > 0) {
                                    that.findLocation();
                                }
                            });
                        }


						if(typeof $(settings.locationSelector).val() != 'undefined')
						{
							if($(settings.locationSelector).val().length == 0) {
								setZoom = false;
							}
						}

						if(typeof $(settings.optionList).size() != 'undefined')
						{
							if($(settings.optionList).size() > 0) {
								setZoom = false;
							}
						}

                        if($('#store-locator-error').length > 0)
                        {
                            setZoom = false;
                        }
                        
                        alert("setZoom:"+setZoom);
						if ($(settings.zoomSelector).val() > 0 && setZoom == true) {
							    //CD - logic is to take the values
							    var noOfOptions = $(settings.zoomSelectorOptions).size();
							    var selectedOptionIdx = $(settings.zoomSelector).prop("selectedIndex");

							    var zoomValue = noOfOptions * 2.1 - selectedOptionIdx;
							    settings.zoom = zoomValue;
							
						} 
                        alert("zoomValue:"+zoomValue);
					    settings.mapOptions.disableDefaultUI = settings.showControls ? false : true;
						settings.mapOptions.scrollwheel = settings.scrollable;
						settings.mapOptions.draggable = settings.draggable;
	
						if(setZoom){
							if (!isNaN(parseInt(settings.zoom, 10))){
								settings.mapOptions.zoom = parseInt(settings.zoom, 10);
							}
						}

                        if (isIE())
                         {
                          settings.mapOptions.zoom = settings.mapOptions.zoom -4;
                        }


						// Bind event handlers
						that.bindEventHandlers();

						// If markers are present, add to map
						if (settings.markers.length > 0) {
							that.setMapMarkers();
						}

						// Initialise map handlers
						settings.mapHandler = new google.maps.Map(settings.mapObj[0], settings.mapOptions);

						settings.encoderHandler = new google.maps.Geocoder();
						settings.markerOptions.map = settings.mapHandler;

						// If we have a latitude and a longitude already, set the position
                        /*
                         if (settings.latObj.val() != '' && settings.latObj.val() != 0 && settings.lngObj.val() != '' && settings.lngObj.val() != 0) {

							var location = new google.maps.LatLng(settings.latObj.val(), settings.lngObj.val());

							that.setMapCenter(location);
							that.setMapMarker(location);
						}
                        */

                        var location;

                        if (typeof currentLocationJSON != "undefined")
                        {
                            if (currentLocationJSON.latitude != 0 & currentLocationJSON.longitude != 0) {
							    location = new google.maps.LatLng(currentLocationJSON.latitude, currentLocationJSON.longitude);
						    }
                            else if (settings.latObj.val() != '' && settings.latObj.val() != 0 && settings.lngObj.val() != '' && settings.lngObj.val() != 0) {
							    location = new google.maps.LatLng(settings.latObj.val(), settings.lngObj.val());
                            }
                            else{
                                //we shouldn't ever get here but this means no lat/lng and no default found
                                location = new google.maps.LatLng(0,0);
                            }
                        }
                        else{
                            if (settings.latObj.val() != '' && settings.latObj.val() != 0 && settings.lngObj.val() != '' && settings.lngObj.val() != 0) {

							    location = new google.maps.LatLng(settings.latObj.val(), settings.lngObj.val());
						    }
                        }

                        
                        if (typeof location != "undefined")
                        {
                            that.setMapCenter(location);
                            google.maps.event.addListenerOnce(settings.mapHandler, 'idle', function() {
                                that.setMapMarker(location);
                                that.setMapCenter(location); //do this again because sometimes the resulting centre is incorrect if still loading
                            });
                        }


                        // bork race condition between default marker and real markers when using JSON search.
		                if (settings.markersJSON) {
		                	setTimeout(function() {that.setMarkers(settings.markersJSON) }, 100)
		                }

				    },
				    setMarkers: function(markers) {

				    	// Instantiate new infobox object
		                var gaInfoBox = new InfoBox({
			                alignBottom: true,
			                boxClass: 'infobox pod',
			                closeBoxMargin: '1.5em',
			                disableAutoPan: false,
			                infoBoxClearance: new google.maps.Size(20, 20),
			                isHidden: false,
			                pixelOffset: new google.maps.Size(-108, -40),
			                pane: 'floatPane'
		                });

		                for (var i = 0; i < markers.length; i++) {
			                var current = markers[i];
		                    var count = i + 1;

			                var marker = new google.maps.Marker({
					                map: settings.mapHandler,
					                icon: window.appPath + '/img/markers/marker_' + String.fromCharCode(65 + i) + ".png",
					                position: new google.maps.LatLng(current.RetailerFromWCF.Latitude, current.RetailerFromWCF.Longitude)
				                });

                            marker.setZIndex(999); //makes markers appear on top of center marker

			                // Set marker objects content property
			                marker.infoBox = '<div class="content"><div class="head"><dl><dt>Distance</dt><dd>' + Math.ceil(current.RetailerFromWCF.DistanceFromPoint) + ' mile(s)</dd></dl></div><div class="body"><ul><li><strong>' + current.RetailerFromWCF.RetailerName + '</strong></li><li>' + current.RetailerFromWCF.RoadName + '</li><li>' + current.RetailerFromWCF.Town + '</li><li>' + current.RetailerFromWCF.PostCode + '</li><li>' + current.RetailerFromWCF.ContactTelephone + '</li><li><a href="' + current.RetailerFromWCF.SiteUrl + '" title="" target="_blank">' + current.RetailerFromWCF.SiteUrl.replace("http://", "").replace("https://", "") + '</a></li></ul></div><div class="base"><a href="#store-' + count + '" title="" class="btn pink">View store profile</a></div></div>';
            
			                // Create event listener		                
			                google.maps.event.addListener(marker, 'click', function (obj) {

				                // Reset infobox content based on clicked marker
				                gaInfoBox.setContent(this.infoBox);

				                // Show infobox, positioned relative to this marker
				                gaInfoBox.open(settings.mapHandler, this);
			                });
		                }
				    },
                    mapUserLocation: function(location, that){
                        //var that = this;
                        if (location.coords) {
                            $('#Latitude').val(location.coords.latitude);
                            $('#Longitude').val(location.coords.longitude);
							    var location = new google.maps.LatLng(settings.latObj.val(), settings.lngObj.val());
							    that.setMapCenter(location);
                                that.setMapMarker(location);
                        }
                    },
					findLocation: function (successCallback, doNotCenter) {
						var that = this;

						var location = object.val();
						var geocodeOptions = { address: location };

						if (settings.regionBias)
                        {
							geocodeOptions.region = settings.regionBias;
                            geocodeOptions.componentRestrictions = {country: settings.regionBias};
                        }

                        
						if (that.validateInput(location)) {

							that.hideFeedback();
							that.hideOptions();

							settings.encoderHandler.geocode(geocodeOptions, function (results, status) {
							    
								if (status == google.maps.GeocoderStatus.OK) {
								    
									switch (results.length) {
								        
										case 0:
                                            $('#store-locator-error').hide();
											that.showFeedback(window.Resources.StoreLocatorLocationNotRecognised);
											break;

										case 1:
                                            if (!(results[0].partial_match == true && results[0].types[0] == "country")){

											    var geometry = results[0].geometry.location;
										    
                                                if (successCallback) {
										            // do the succes callback only when the map is done updating - otherwise we
										            // get a map full of "image not available".

                                                    var lat = settings.latObj.val();
                                                    var lng = settings.lngObj.val();

                                                    if (lat == geometry.lat() && lng == geometry.lng())
                                                        successCallback();
                                                    else {
                                                        google.maps.event.addListenerOnce(settings.mapHandler, 'idle', function() {
                                                            successCallback();
                                                        });
                                                    }
                                                }

											    that.setInputs(geometry);

                                                if(!doNotCenter){
											        that.setMapCenter(geometry);
											        that.setMapMarker(geometry);
                                                }
                                            }
                                            else{
                                                $('#store-locator-error').hide();
										        that.showFeedback(window.Resources.StoreLocatorLocationNotRecognised);
                                                that.resetInputs();
                                            }

										    break;

										default:
											if (settings.allowRefinement) {
											    //reset long and lat if we have refinements
											    that.resetInputs();
												//that.showFeedback('Did you mean...');
												//that.showOptions(results);
											} else {
                                                $('#store-locator-error').hide();
												that.showFeedback('Please enter a more specific postcode.');
											}

										    if (successCallback)
										        successCallback();
									}
								} else {
									//that.showFeedback('Location not recognised');
								}
							});
						} else {
							//that.showFeedback('Please correct your input.');
						}
					},

					bindEventHandlers: function () {

						var that = this;

						// Bind event handler for keypress on input field
						object.on('keypress', function (e) {

							var key = e.charCode || e.keyCode;

							if (key == 13) {
								e.preventDefault();

                                if(settings.centerOnBlur)
                                {
								    that.findLocation(function() {								    
                                        if (settings.frmObj) settings.frmObj.submit();
								    });
                                }
                                else
                                {
								    that.findLocation(function() {								    
                                        if (settings.frmObj) settings.frmObj.submit();
								    }, true);
                                }
							}
						});

                        
						object.on('blur', function (e) {
							var value = object.val();

							if (that.lastBlurRequest !== value) {
                                if(settings.centerOnBlur)
                                {
                                    that.findLocation();
                                }
                                else
                                {
                                    that.findLocation(null, true);
                                }
								
								that.lastBlurRequest = value;
							}
						});

						if (settings.optObj) {
							// Bind event handler to options links (use selector to simulate live binder deferred to document)
							$(document).on('click', settings.optObj.selector + ' a', function (e) {

								e.preventDefault();

								object.val($(this).data('full-address'));

								settings.btnObj.trigger('click');
							});
						}

						if (!settings.btnObj)
							return;

						// Bind event handler to find button
						settings.btnObj.on('click', function (e) {
							e.preventDefault();
                                if(settings.centerOnBlur)
                                {
								    that.findLocation(function() {								    
                                        if (settings.frmObj) settings.frmObj.submit();
								    });
                                }
                                else
                                {
								    that.findLocation(function() {								    
                                        if (settings.frmObj) settings.frmObj.submit();
								    }, true);
                                }
						});
					},

					validateInput: function (input) {

						return (input.length > 0);
					},

					showFeedback: function (message) {
						if (settings.fbkObj) {
							settings.fbkObj.text(message).show();
						}
					},

					hideFeedback: function () {
						if (settings.fbkObj) {
							settings.fbkObj.empty().hide();
						}
					},

					showOptions: function (results) {

						for (var i = 0; i < results.length; i++) {

							var option = results[i];
							var addressComponents = option.address_components;
							var address = '';

							for (var ii = 0; ii < addressComponents.length; ii++) {

								address += addressComponents[ii].long_name;

								if (ii != addressComponents.length - 1) {
									address += ", ";
								}
							}

							settings.optObj.append('<li><a data-full-address="' + address + '" data-formatted-address="' + option.formatted_address + '">' + option.formatted_address + '</a></li>');
						}

						settings.optObj.show();
					},

					hideOptions: function () {
						if (settings.optObj) {
							settings.optObj.empty().hide();
						}
					},

					setInputs: function (geometry) {
					    //console.log('DEBUG SET GEOMETRY' + geometry.lat() + " - " + geometry.lng());
						settings.latObj.val(geometry.lat());
						settings.lngObj.val(geometry.lng());
					},

                    resetInputs: function () {
						settings.latObj.val('0');
						settings.lngObj.val('0');
					},
					
					setMapCenter: function (geometry) {
						settings.mapHandler.setCenter(geometry);
					},

					setMapMarker: function (geometry) {

						var that = this;
                        alert("Marker");
						that.clearMapMarkers();

						settings.markerOptions.position = geometry;

						var marker = new google.maps.Marker(settings.markerOptions);

						settings.markers.push(marker);
					},

					clearMapMarkers: function () {

						var that = this;

						for (var i = 0; i < settings.markers.length; i++) {

							// Remove from map
							settings.markers[i].setMap(null);

							// Remove marker from array
							settings.markers.splice(i, 1);
						}
					},
					log: function (message) {
						if (settings.debug) {
							//console.log(message);
						}
					}
				};

			// Merge options with default settings
            //true denotes recursion when mixing, need this for overriding map center
			settings = $.extend(true, settings, opts);

			// Initialise the plugin
			methods.init();
		});
	};
})(jQuery);


/*

Coats Pod Stacker Plugin
- Overview: Constructs columnnal layouts without isotope - much more lightweight so can be used when filtering and ordering isn't required.
- Expects: Markup to be .pods > .pod. Container .pods should have data attributes defining desktop, tablet and mobile column counts. Relvent CSS needs to be present as well which should include some decent no-js fallback styling.
- Author: JC
- Date: 06/08/13
- Edited: 06/08/13

- NOTE: Not currently in use but may be useful if multiple instances of isotope is slowing the page down.

*/

(function ($) {

    $.fn.coatsPodStacker = function (opts) {

        return this.each(function () {

            // Declare variables
            var object = $(this),
				settings = {},
				storedPods;

            var methods = {

                init: function () {

                    // Maintain reference to methods object
                    var that = this;

                    settings.desktopColCount = object.data('desktop-columns'),
					settings.tabletColCount = object.data('tablet-columns'),
					settings.mobileColCount = object.data('mobile-columns');

                    // Grab all pods in this object, store them and count them
                    var pods = object.find('.pod');
                    settings.storedPods = pods.clone();
                    settings.storedPodsCount = settings.storedPods.length;

                    // Remove them from DOM, now stored in memory
                    pods.remove();

                    // Bindy the events
                    that.bindEvents();
                },

                calculate: function () {


                    var windowWidth = $(window).width();
                    var columnCount;

                    if (windowWidth > 768) {
                        columnCount = settings.desktopColCount;
                    }

                    if (windowWidth < 768 && windowWidth > 480) {
                        columnCount = settings.tabletColCount;
                    }

                    if (windowWidth < 480) {
                        columnCount = settings.mobileColCount;
                    }

                    var podsPerColumn = Math.ceil(settings.storedPodsCount / columnCount);

                    var runningStart = 0;

                    // Build new columns
                    for (var i = 0; i < columnCount; i++) {

                        var column = $('<div class="column">');

                        var slicedPods = settings.storedPods.slice(runningStart, runningStart + podsPerColumn);

                        column.append(slicedPods).appendTo(object);

                        runningStart = runningStart + podsPerColumn;
                    }
                },

                bindEvents: function () {

                    // Maintain reference to methods object
                    var that = this;

                    $(window).on('resize load', function (e) {
                        that.calculate();
                    })
                }
            }


            // Merge options with default settings
            settings = $.extend(settings, opts);

            // Initialise plugin
            methods.init();
        });
    };
})(jQuery);


(function ($) {

    //re-set all client validation given a jQuery selected form or child
    $.fn.resetValidation = function () {

        var $form = this;

        //reset jQuery Validate's internals
        $form.validate().resetForm();

        //reset unobtrusive validation summary, if it exists
        $form.find("[data-valmsg-summary=true]")
            .removeClass("validation-summary-errors")
            .addClass("validation-summary-valid")
            .find("ul").empty();

        //reset unobtrusive field level, if it exists
        $form.find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();

        return $form;
    };
})(jQuery);