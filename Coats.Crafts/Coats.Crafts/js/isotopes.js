var isotopes = $('.izotaup');

// Check if we have any isotopes on page
if(isotopes.length > 0) {

	// Loop over each isotope
	for(var i = 0; i < isotopes.length; i++) {

		// Cache reference to this isotope
		var current = $(isotopes[i]);

		// Check we have the pods container
		if(current.find('.pods').length > 0) {

			// Check we have pods within that pods container
			if(current.find('.pods .pod').length > 0) {

				var windowWidth 	= $(window).width(),
					containerWidth 	= current.find('.pods').width(),
					colCount, options;

				// Check if we should set up a four column layout
				if(current.find('.pods .pod:eq(0)').hasClass('span3')) {

					colCount = 4;
				}

				// Check if we should set up a three column layout
				if(current.find('.pods .pod:eq(0)').hasClass('span4')) {

					colCount = 3;
				}

				// Check if we should set up a two column layout
				if(current.find('.pods .pod:eq(0)').hasClass('span6')) {

					colCount = 2;
				}

				options = {
					itemSelector: current.find('.pod'),
					masonry: {columnWidth: columnWidth(windowWidth, containerWidth, colCount)}
				};

				// Initalise isotope
				current.find('.pods').isotope(options);

				// Horrible fix for width issues - trigger a reflow after one millisecond
				setTimeout(function () { current.find('.pods').isotope('reLayout'); }, 1);

				// Assign event handler
				$(window).on('resize', function () {

					// Only apply to modern browsers
					if (!$('html').hasClass('lt-ie9')) {

						// Reposition isotoped items
						options = {
							masonry: {columnWidth: columnWidth($(window).width(), current.find('.pods').width(), colCount)}
						};

						// Initalise isotope
						current.find('.pods').isotope(options);
					}
				});
			}
		}
	}
}









		$(window).on('resize', function () {

			if (!$('html').hasClass('lt-ie9')) {

				var windowWidth = $(window).width(),
							containerWidth = productsIsotope.width(),
							columnWidths;

				if (windowWidth <= 480) {
					columnWidths = Math.floor(containerWidth / 2);
				}

				if (windowWidth > 480 && windowWidth <= 768) {
					columnWidths = Math.floor(containerWidth / 3);
				}

				if (windowWidth > 768) {
					columnWidths = Math.floor(containerWidth / 4);
				}

				productsIsotope.isotope({
					masonry: {
						columnWidth: columnWidths
					}
				});
			}
		});