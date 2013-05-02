$(document).ready(function () {

    $('a.img-thumb').hover(function () {
        $(this).parent().stop(false, false).animate({ opacity: 0.5 }, 500);
    }, function () {
        $(this).parent().stop(false, false).animate({ opacity: 1 }, 500);
    });

    //Tour and shortcodes tabs
    $('.tabs').tabs({ fx: { opacity: 'toggle', duration: 'fast'} });

    // Inputs set default value
    $('input[title], textarea').bind({
        focus: function () {
            if ($(this).attr('title') == $(this).attr('value')) {
                $(this).attr('value', '');
            }
        },
        blur: function () {
            if ($(this).attr('value') == '') {
                $(this).attr('value', $(this).attr('title'));
            }
        }
    });

    //Flickr border
    $(".flickr ul li a").hover(function () {
        $(this).stop().animate({ "border-color": "#d93e39" }, 600, 'swing');
    },
	function () {
	    $(this).stop().animate({ "border-color": "#202020" }, 600, 'swing');
	});


    //Video responsive
    $("body").fitVids();   

    /* Homepage companies logo */
    $(".companies li").hover(function () {
        var thumbOver = $(this).find("img").attr("src");
        $(this).find("a.thumb").css({ 'background': 'url(' + thumbOver + ') no-repeat center bottom' });
        $(this).find("span").stop().fadeTo('normal', 0, function () {
            $(this).hide()
        });
    }, function () {
        $(this).find("span").stop().fadeTo('normal', 1).show();
    });


    //Animate featured background on homepage
    $(".featured_bg").find('img').hover(
		function () {
		    $(this).fadeIn(100).animate({ top: "-=10px" }, 100);
		},
		function () {
		    $(this).fadeIn(100).animate({ top: "+=10px" }, 100);
		}
	);



    /* Socials Big */
    $("ul.social_networks li").hover(function () {
        var thumbOver = extractUrl($(this).find("span").css("background-image"));
        $(this).find("span").stop().animate({ backgroundPosition: '32px 0' }, 150, 'easeOutQuint');
    }, function () {
        $(this).find("span").stop().animate({ backgroundPosition: '0 0' }, 150, 'easeOutQuint');
    });

    /* Footer social */
    $("ul.social_footer	li a").hover(function () {
        var thumbOver = extractUrl($(this).find("span").css("background-image"));
        $(this).find("span").stop().animate({ backgroundPosition: '16px 0' }, 250, 'easeOutQuint');
    }, function () {
        $(this).find("span").stop().animate({ backgroundPosition: '0 0' }, 250, 'easeOutQuint');
    });



    /* Menu slide down and hide */
    $('.main-menu li:has(ul)').addClass('submenu');

    $('.main-menu').on('mouseenter', 'li', function () {
        $(this).children('ul').hide().stop(true, true).fadeIn("normal");
    }).on('mouseleave', 'li', function () {
        $(this).children('ul').stop(true, true).fadeOut("normal");
    });

    //Fixing responsive menu
    $(window).resize(function () {
        $('.main-menu').children('ul').children('li').children('ul').hide();
        $('.main-menu').children('ul').children('li').children('ul').children('li').children('ul').hide();
    });

    //Categories Text Indent
    $(".categories ul li").hover(
		function () {
		    if (!($('a', this).hasClass("selected")))
		        $('a', this).stop().animate({ textIndent: 15 }, 800, 'easeOutQuint');
		},
		function () {
		    if (!($('a', this).hasClass("selected")))
		        $('a', this).stop().animate({ textIndent: 0 }, 800, 'easeOutQuint');
		}
	);


    /*Shortcodes*/
    $(".toggle_container").hide();
    $(".toggle").click(function () {
        $(this).toggleClass("toggle_active").next().slideToggle("slow");
    });
    $(".opened_toggle").trigger('click');
    /*End Shortcodes*/

    /*Pretyphoto*/
    $("a[data-rel^='prettyPhoto']").prettyPhoto({ animationSpeed: 'slow', social_tools: false, slideshow: 2000 });    
    /*End Pretyphoto*/


});

//Excract url from element
function extractUrl(input) {
    return input.replace(/"/g, "").replace(/url\(|\)$/ig, "");
}
