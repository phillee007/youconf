$(document).ready(function () {
    $("#slider").responsiveSlides({
        auto: false,
        pager: false,
        nav: true,
        speed: 500,
        namespace: "portfolio_slider"
    }).bind({ "callbacks-after": function () {
        $("a[rel^='prettyPhoto']").prettyPhoto({ animationSpeed: 'slow', theme: 'facebook', slideshow: 2000 });
    }
    });

    $("#slider").touchwipe({
        wipeLeft: function () { $('.portfolio_slider1_nav.next').trigger('click'); },
        wipeRight: function () { $('.portfolio_slider1_nav.prev').trigger('click'); }
    });
});