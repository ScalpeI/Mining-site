/*  Theme Name: Zoomka - Responsive Bootstrap 4 Landing Template
    Author: VBThemes
    Version: 1.0.0
    Created:August 2018
    File Description:Main JS file of the template
*/

(function($) {

    'use strict';

    function initNavbarStickey() {
        $(window).on('scroll', function() {
            var scroll = $(window).scrollTop();

            if (scroll >= 50) {
                $(".sticky").addClass("stickyadd");
            } else {
                $(".sticky").removeClass("stickyadd");
            }
        });
    }

    function initSmoothLink() {
        $('.navbar-nav a').on('click', function(event) {
            var $anchor = $(this);
            $('html, body').stop().animate({
                scrollTop: $($anchor.attr('href')).offset().top - 0
            }, 1500, 'easeInOutExpo');
            event.preventDefault();
        });
    }

    function initScrollspy() {
        $("#navbarCollapse").scrollspy({
            offset: 20
        });
    }

    function initBacktoTop() {
        $(window).on('scroll', function() {
            if ($(this).scrollTop() > 100) {
                $('.back_top_angle_up').fadeIn();
            } else {
                $('.back_top_angle_up').fadeOut();
            }
        });
        $('.back_top_angle_up').on('click', function() {
            $("html, body").animate({
                scrollTop: 0
            }, 1000);
            return false;
        });
    }

    function initMfpvideo() {
        $('.features_video').magnificPopup({
            disableOn: 700,
            type: 'iframe',
            mainClass: 'mfp-fade',
            removalDelay: 160,
            preloader: false,
            fixedContentPos: false
        });
    }

    function initSlider() {
        $("#owl-slider").owlCarousel({
            autoPlay: 7000,
            stopOnHover: true,
            navigation: false,
            paginationSpeed: 1000,
            goToFirstSpeed: 2000,
            singleItem: true,
            autoHeight: true,
        });
    }

    function init() {
        initNavbarStickey();
        initSmoothLink();
        initScrollspy();
        initBacktoTop();
        initMfpvideo();
        initSlider();
    }
    init();

})(jQuery)