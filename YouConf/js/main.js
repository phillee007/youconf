$(window).load(function () {
    $('.flexslider').flexslider();
});

$(function () {
    $('#work').carouFredSel({
        width: '100%',
        scroll: 1,
        auto: false,
        pagination: false,
        prev: '.prev_item',
        next: '.next_item'
    });

    $("#work").touchwipe({
        wipeLeft: function () { $('.next_item').trigger('click'); },
        wipeRight: function () { $('.prev_item').trigger('click'); }
    });
});