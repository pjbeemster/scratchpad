/*
 *
 * Isotope static helper methods
 *
 * These methods are used to rebuild the page after an ajax call
 * has been performed to filter the content. These should only 
 * be used for the faceted content isotope layouts from now on.
 *
 */
var isotopeHelper = {

    allowAjax: true,

    _rebuild: function (data, append) {

        // var wrapper = $('<div />').append(data).find('#wrap #main');
        var wrapper = $('<div />').append(data).find('#wrap');

        wrapper.imagesLoaded(function () {

            $('.atclear').remove();

            if (wrapper.find('#main .faceted .pods').length == 0) {
                $('#no-results').show();
            } else {
                $('#no-results').hide();
            };

            // Build returned data object
            var returned = {
                ProductNav: wrapper.find('#main .product-nav'),
                BrandFilter: wrapper.find('#main .brand-filter'),
                Pods: wrapper.find('#main .faceted .pods'),
                FilterPanel: wrapper.find('#main .filter-panel'),
                ShowFilter: wrapper.find('#main .show-filter'),
                ShowMore: wrapper.find('#main .show-more'),
                fhLocation: wrapper.find('#main #FredHopperLocation'),
                header: wrapper.find('#main .headers'),
                newsletter: wrapper.find('#base div.newsletter')
            };

            // Need to re-bind the show-more link after ajax update
            if (returned.ShowMore.length > 0) {

                $(document).on('click', '.show-more', function (e) {

                    // Catch the click
                    e.preventDefault();

                    // history.js and backbutton fix
                    // set the stateChangeIsLocal to false so we're not calling ajaxer twice    
                    window.stateChangeIsLocal = true;

                    var currentPageIndex = returned.ShowMore.find('ul li.current').index();
                    var intendedPage = returned.ShowMore.find('ul li').eq(parseInt(currentPageIndex) + 1);

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

            if (navigator) {
                if (navigator.userAgent) {
                    /*
                    Windows phone sometimes adds ajaxed images to cache, but then won't load them.
                    This adds cache busting to the ajaxed images in the pods on win phones only
                    */
                    if (navigator.userAgent.match(/Windows Phone/i)) {
                        var rand = Math.floor((Math.random() * 1000000) + 1);
                        $('.pod img', returned.Pods).each(function () {
                            $(this).attr('src', $(this).attr('src') + "?" + rand);
                        });
                    }
                }
            }

            // Swap out old content for new
            $('#FredHopperLocation').val(returned.fhLocation.val());

            //
            // Brand filter is a little bit more involved than just replacing the html...
            //
            //$('.brand-filter').html(returned.BrandFilter.html());
            var brandFilter = $('.brand-filter');
            if (brandFilter.length > 0 && returned.BrandFilter.length > 0) {
                // We have both an existing filter AND new brand filter to replace it with
                brandFilter.html(returned.BrandFilter.html()).show();
            }
            else if (returned.BrandFilter.length > 0) {
                // We've got a new brand filter, but no existing container div to put it in,
                // so we need to inject the returned HTML after the product header.
                var productHeader = $('.product-header');
                if (productHeader.length > 0) {
                    // Found the product header container, so inject.
                    productHeader.after(returned.BrandFilter);
                    // Make sure it's visible
                    brandFilter = $('.brand-filter');
                    if (brandFilter.length > 0) {
                        // Be surprised if the brand filter was missing, especially seeing as we have just injected it!!!
                        brandFilter.show();
                    }
                }
            }
            else {
                // No returned content. Probably the filter has been switched off be another means
                // such as de-selecting the relevant brand filter.
                // In which case, hide the mofo.
                brandFilter.hide();
            }


            if (returned.ProductNav.length > 0) {
                $('.product-nav').html(returned.ProductNav.html());
            }

            if (returned.ShowMore.length > 0) {

                if ($('.show-more').length == 0) {
                    // No container div for show more, so let's add one after the isotope faceted pods div
                    var podsIsotope = $('.faceted .pods');
                    if (podsIsotope.length > 0) {
                        podsIsotope.after(returned.ShowMore).show();
                    }
                }
                else {
                    $('.show-more').html(returned.ShowMore.html()).show();
                }
            }
            else {
                $('.show-more').hide();
            }

            if (returned.FilterPanel.length > 0) {

                var filterPanel = $('.filter-panel');

                if (filterPanel.length == 0) {
                    // No content in the filterPanel, so let's add some
                    $('.product-nav').after(returned.FilterPanel);
                    $('.product-nav').after(returned.ShowFilter);
                }

                // OK, now try looking for the filterPanel again
                filterPanel = $('.filter-panel');

                if (filterPanel.length > 0) {

                    if ($(window).width() <= 768) {
                        $('.filter-panel').html(returned.FilterPanel.html()).hide();
                    } else {
                        $('.filter-panel').html(returned.FilterPanel.html()).show();
                    }

                    $('.show-filter').show();
                    // Trigger a click event to force the filterPanel to show
                    $('.show-filter').trigger('click');
                }
                // Moved this up above the "if (filterPanel.length > 0) {"
                //                else {
                //                    $('.product-nav').before(returned.FilterPanel);
                //                    $('.product-nav').before(returned.ShowFilter);
                //                }


                // Reset custom scroller
                initCustomScroller();
            }
            else {
                $('.filter-panel').remove();
                $('.show-filter').remove();
            }

            if (returned.header.length > 0) {
                $('.headers').html(returned.header.html());
            }

            if (append) {
                $('.faceted .pods').isotope('insert', returned.Pods.find('.pod'));
            }

            else {
                $('.faceted .pods').isotope('remove', $('.faceted .pods').find('.pod'));
                $('.faceted .pods').isotope('insert', returned.Pods.find('.pod'));
            }

            if (returned.newsletter.length > 0) {
                $('#base div.newsletter').html(returned.newsletter.html());
            }


            /*
            *
            * Pull product categories
            * into new row below header
            *
            */
            // if ($(window).width() <= 768) {

            //     if ($('.product-nav .dropdown.category').length > 0) {

            //         if ($('.catter').length > 0) {
            //             $('.catter').remove();
            //         }

            //         var newRow = $('<div class="row-fluid catter"><div class="span12"></div></div>');

            //         newRow.find('.span12').append($('.product-nav .control-group.arrow-base'));
            //         newRow.insertAfter($('.product-header .row-fluid.headers'));
            //     }
            // }

            $(window).trigger("relayout", true)


            /*
            *
            * Pull product categories
            * into filter panel
            *
            */

            //            if ($(window).width() <= 768) {

            //                if ($('.product-nav .category').length > 0) {

            //                    if ($('.filter-panel').length > 0) {

            //                        var categoryLinks = $('.product-nav .category a'),
            //                            facetBlock = $('<div class="facet categories"><h2>Categories<span></span></h2><ul></ul></div>');

            //                        // Build category list
            //                        categoryLinks.each(function () {
            //                            $('<li />').html($(this).prepend('<span />')).appendTo(facetBlock.find('ul'));
            //                        });


            //                        facetBlock.insertBefore($('.filter-panel .reset'))

            //                    } else {

            //                        var mainArticle = $('#main > article');

            //                        var categoryLinks = $('.product-nav .category a'),
            //                            facetBlock = $('<div class="facet categories"><h2>Categories<span></span></h2><ul></ul></div>');

            //                        // Build category list
            //                        categoryLinks.each(function () {
            //                            $('<li />').html($(this).prepend('<span />')).appendTo(facetBlock.find('ul'));
            //                        })

            //                        // Create panel and then pull category list in
            //                        $('<div class="show-filter"><a title="Show filter panel" rel="nofollow"></a></div>').prependTo(mainArticle);
            //                        $('<div class="filter-panel"><div><a href="#" class="icon close small" title="Hide filter panel" rel="nofollow">Close</a><h1>Filter By:</h1></div></div>').append(facetBlock).prependTo(mainArticle);
            //                    }
            //                }
            //            }


            /*
            *
            * Pull content type filters
            * into filter panel
            *
            */
            if ($(window).width() <= 768) {

                var sideTypeFilter = $('<div class="facet types" />');

                sideTypeFilter.append('<h2>Content Types</h2>').append('<ul />');

                if ($('.product-nav .control-group .radioesque').length > 0) {

                    $('.product-nav .control-group .radioesque').each(function () {

                        sideTypeFilter.find('ul').append($('<li />').append($(this).removeClass('radioesque')));
                    });

                    sideTypeFilter.insertBefore($('.filter-panel .reset'));
                }
            }

            // Check addthis is loaded
            if (typeof addthis != 'undefined') {

                // Reinitalise addthis for newly added pods
                addthis.toolbox('.pods');
            }

            // Reset min height of main div now everything has been reloaded
            resetMainMinHeight();

            $('div.rateit').rateit();

            // Bit nasty to call from here but reduces code bloat where a number of deffered objects call the same methods
            hideLoadPopup();
        });
    },

    ajaxer: function (options, append) {

        var that = this;

        // console.log("ajaxer");

        if (this.allowAjax) {

            this.allowAjax = false;

            // Show load box
            showLoadPopup();

            // Set up ajax request
            var ajax = $.ajax(options)

            // Define done handler based on passed in arguments
            if (append) {
                ajax.done(isotopeHelper.doneHandlerAppend)
            } else {
                ajax.done(isotopeHelper.doneHandlerReplace)
            }

            // Define failure handler
            ajax.fail(failHandler);

            // Always reset to allow another call
            ajax.always(function () {

                //Add the ajax url to History so we can load it up from the back button
                addAjaxUrlToHistory(options);

                that.allowAjax = true;
            });

            return ajax;
        }
    },

    doneHandlerAppend: function (data) {
        // Rebuild DOM and isotope
        isotopeHelper._rebuild(data, true);
    },

    doneHandlerReplace: function (data) {
        // Rebuild DOM and isotope
        isotopeHelper._rebuild(data, false);
    }
}


//----------------------------------------------
// function addAjaxUrlToHistory
//
// Generates the FredHopper QueryString and pushes
// it to the browser history to enable links to work
// when the browser back and forward buttons are used
//----------------------------------------------

function addAjaxUrlToHistory(options) {

    // AJAX POST update
    if (typeof options.data != 'undefined') {

        //build FH query
        var loc = options.data.FredHopperLocation;
        var searchTerm = options.data["ComponentSection.SearchTerm"];
        var sortBy = options.data["Sort.SortBy"];

        //do we have an existing search term
        var searchLocStart = loc.indexOf("$s=");

        if (searchLocStart > -1) {
            //if we have an existing search term then replace it with the new one
            var searchLocEnd = loc.indexOf("/", (searchLocStart));
            var searchString = loc.substr((searchLocStart + 3), searchLocEnd - (searchLocStart + 3));
            loc = loc.replace(searchString, searchTerm);
        } else {
            //if we don't have a search term then we add the new one to the FH location
            var locSplit = loc.split("/schematitle");
            loc = locSplit[0] + "/$s=" + searchTerm + "/schematitle" + locSplit[1];
        }

        var fhLocation = "fh_location=" + loc + "&fh_view_size=10&fh_view=lister&fh_sort_by=" + sortBy;
        fhLocation = encodeURIComponent(fhLocation);
        var postData = "?fh_params=" + fhLocation;
        $data = { text: 'txt', type: 'global', url: window.location.pathname + postData };
        //Add to history
        History.pushState($data, document.title, window.location.pathname + postData);
    }

    // AJAX GET update 
    if (window.stateChangeIsLocal) {
        if (typeof options.url != 'undefined') {
            //history is a new FH parameter to emulate and restore the previous pagination result
            var optionHistoryUrl = options.url + "&history=true";
            $data = { text: 'txt', type: 'global', url: optionHistoryUrl };
            optionPathUrl = window.location.pathname + optionHistoryUrl;
            //Add to history
            History.pushState($data, document.title, optionPathUrl);
        }
    }

}

