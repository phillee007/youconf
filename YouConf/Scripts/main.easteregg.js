$(function () {
    //And for the second spot prize - make the Code Project man go wild!
    if (window.location.search.indexOf("crazymaneasteregg") > 0) {
        $("img")
        .attr("id", "crazyman")
        .attr("src", "http://www.codeproject.com/images/bob.png")
        .css("width", "278px")
        .css("height", "384px")
        .css("position", "absolute")
        .css("top", "0")
        .css("left", $(window).width() / 2 - 278 / 2)
        .css("display", "none")
        .appendTo("body");

        $("#crazyman")
        .animate({
            width: 'toggle',
            height: 'toggle'
        }, {
            duration: 5000,
            specialEasing: {
                width: 'linear',
                height: 'easeOutBounce'
            },
            complete: function () {
                $("#crazyman").hide("explode", 3000, function () {
                    $("<div style='position: absolute; top: 0; left: 0; width: 100%; height: 100%; text-align: center;background-color:#fff;font-size:50px;'>Easter egg time!</div>")
                    .appendTo("body")
                    .slideDown(1500)
                    .slideUp(1500, function () {
                        $("#crazyman").remove();
                        $(this).remove();
                    });
                });
            }
        });
    }
});