/* ------------------------------------------------------------------------
Class: prettyPhoto
Use: Lightbox clone for jQuery
Author: Stephane Caron (http://www.no-margin-for-errors.com)
Version: 3.1.4
------------------------------------------------------------------------- */

(function ($) {
    $.prettyPhoto = { version: '3.1.4' }; $.fn.prettyPhoto = function (pp_settings) {
        pp_settings = jQuery.extend({ hook: 'rel', animation_speed: 'fast', ajaxcallback: function () { }, slideshow: 5000, autoplay_slideshow: false, opacity: 0.80, show_title: true, allow_resize: true, allow_expand: true, default_width: 500, default_height: 344, counter_separator_label: '/', theme: 'pp_default', horizontal_padding: 20, hideflash: false, wmode: 'opaque', autoplay: true, modal: false, deeplinking: true, overlay_gallery: true, overlay_gallery_max: 30, keyboard_shortcuts: true, changepicturecallback: function () { }, callback: function () { }, ie6_fallback: true, markup: '<div class="pp_pic_holder"> \
      <div class="ppt">&nbsp;</div> \
      <div class="pp_top"> \
       <div class="pp_left"></div> \
       <div class="pp_middle"></div> \
       <div class="pp_right"></div> \
      </div> \
      <div class="pp_content_container"> \
       <div class="pp_left"> \
       <div class="pp_right"> \
        <div class="pp_content"> \
         <div class="pp_loaderIcon"></div> \
         <div class="pp_fade"> \
          <a href="#" class="pp_expand" title="Expand the image">Expand</a> \
          <div class="pp_hoverContainer"> \
           <a class="pp_next" href="#">next</a> \
           <a class="pp_previous" href="#">previous</a> \
          </div> \
          <div id="pp_full_res"></div> \
          <div class="pp_details"> \
           <div class="pp_nav"> \
            <a href="#" class="pp_arrow_previous">Previous</a> \
            <p class="currentTextHolder">0/0</p> \
            <a href="#" class="pp_arrow_next">Next</a> \
           </div> \
           <p class="pp_description"></p> \
           <div class="pp_social">{pp_social}</div> \
           <a class="pp_close" href="#">Close</a> \
          </div> \
         </div> \
        </div> \
       </div> \
       </div> \
      </div> \
      <div class="pp_bottom"> \
       <div class="pp_left"></div> \
       <div class="pp_middle"></div> \
       <div class="pp_right"></div> \
      </div> \
     </div> \
     <div class="pp_overlay"></div>', gallery_markup: '<div class="pp_gallery"> \
        <a href="#" class="pp_arrow_previous">Previous</a> \
        <div> \
         <ul> \
          {gallery} \
         </ul> \
        </div> \
        <a href="#" class="pp_arrow_next">Next</a> \
       </div>', image_markup: '<img id="fullResImage" src="{path}" />', flash_markup: '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" width="{width}" height="{height}"><param name="wmode" value="{wmode}" /><param name="allowfullscreen" value="true" /><param name="allowscriptaccess" value="always" /><param name="movie" value="{path}" /><embed src="{path}" type="application/x-shockwave-flash" allowfullscreen="true" allowscriptaccess="always" width="{width}" height="{height}" wmode="{wmode}"></embed></object>', quicktime_markup: '<object classid="clsid:02BF25D5-8C17-4B23-BC80-D3488ABDDC6B" codebase="http://www.apple.com/qtactivex/qtplugin.cab" height="{height}" width="{width}"><param name="src" value="{path}"><param name="autoplay" value="{autoplay}"><param name="type" value="video/quicktime"><embed src="{path}" height="{height}" width="{width}" autoplay="{autoplay}" type="video/quicktime" pluginspage="http://www.apple.com/quicktime/download/"></embed></object>', iframe_markup: '<iframe src ="{path}" width="{width}" height="{height}" frameborder="no"></iframe>', inline_markup: '<div class="pp_inline">{content}</div>', custom_markup: '', social_tools: '<div class="twitter"><a href="http://twitter.com/share" class="twitter-share-button" data-count="none">Tweet</a><script type="text/javascript" src="http://platform.twitter.com/widgets.js"></script></div><div class="facebook"><iframe src="//www.facebook.com/plugins/like.php?locale=en_US&href={location_href}&amp;layout=button_count&amp;show_faces=true&amp;width=500&amp;action=like&amp;font&amp;colorscheme=light&amp;height=23" scrolling="no" frameborder="0" style="border:none; overflow:hidden; width:500px; height:23px;" allowTransparency="true"></iframe></div>'
        }, pp_settings); var matchedObjects = this, percentBased = false, pp_dimensions, pp_open, pp_contentHeight, pp_contentWidth, pp_containerHeight, pp_containerWidth, windowHeight = $(window).height(), windowWidth = $(window).width(), pp_slideshow; doresize = true, scroll_pos = _get_scroll(); $(window).unbind('resize.prettyphoto').bind('resize.prettyphoto', function () { _center_overlay(); _resize_overlay(); }); if (pp_settings.keyboard_shortcuts) {
            $(document).unbind('keydown.prettyphoto').bind('keydown.prettyphoto', function (e) {
                if (typeof $pp_pic_holder != 'undefined') {
                    if ($pp_pic_holder.is(':visible')) {
                        switch (e.keyCode) {
                            case 37: $.prettyPhoto.changePage('previous'); e.preventDefault(); break; case 39: $.prettyPhoto.changePage('next'); e.preventDefault(); break; case 27: if (!settings.modal)
                                    $.prettyPhoto.close(); e.preventDefault(); break;
                        };
                    };
                };
            });
        }; $.prettyPhoto.initialize = function () {
            settings = pp_settings; if (settings.theme == 'pp_default') settings.horizontal_padding = 16; if (settings.ie6_fallback && $.browser.msie && parseInt($.browser.version) == 6) settings.theme = "light_square"; theRel = $(this).attr(settings.hook); galleryRegExp = /\[(?:.*)\]/; isSet = (galleryRegExp.exec(theRel)) ? true : false; pp_images = (isSet) ? jQuery.map(matchedObjects, function (n, i) { if ($(n).attr(settings.hook).indexOf(theRel) != -1) return $(n).attr('href'); }) : $.makeArray($(this).attr('href')); pp_titles = (isSet) ? jQuery.map(matchedObjects, function (n, i) { if ($(n).attr(settings.hook).indexOf(theRel) != -1) return ($(n).find('img').attr('alt')) ? $(n).find('img').attr('alt') : ""; }) : $.makeArray($(this).find('img').attr('alt')); pp_descriptions = (isSet) ? jQuery.map(matchedObjects, function (n, i) { if ($(n).attr(settings.hook).indexOf(theRel) != -1) return ($(n).attr('title')) ? $(n).attr('title') : ""; }) : $.makeArray($(this).attr('title')); if (pp_images.length > settings.overlay_gallery_max) settings.overlay_gallery = false; set_position = jQuery.inArray($(this).attr('href'), pp_images); rel_index = (isSet) ? set_position : $("a[" + settings.hook + "^='" + theRel + "']").index($(this)); _build_overlay(this); if (settings.allow_resize)
                $(window).bind('scroll.prettyphoto', function () { _center_overlay(); }); $.prettyPhoto.open(); return false;
        }
        $.prettyPhoto.open = function (event) {
            if (typeof settings == "undefined") { settings = pp_settings; if ($.browser.msie && $.browser.version == 6) settings.theme = "light_square"; pp_images = $.makeArray(arguments[0]); pp_titles = (arguments[1]) ? $.makeArray(arguments[1]) : $.makeArray(""); pp_descriptions = (arguments[2]) ? $.makeArray(arguments[2]) : $.makeArray(""); isSet = (pp_images.length > 1) ? true : false; set_position = (arguments[3]) ? arguments[3] : 0; _build_overlay(event.target); }
            if ($.browser.msie && $.browser.version == 6) $('select').css('visibility', 'hidden'); if (settings.hideflash) $('object,embed,iframe[src*=youtube],iframe[src*=vimeo]').css('visibility', 'hidden'); _checkPosition($(pp_images).size()); $('.pp_loaderIcon').show(); if (settings.deeplinking)
                setHashtag(); if (settings.social_tools) { facebook_like_link = settings.social_tools.replace('{location_href}', encodeURIComponent(location.href)); $pp_pic_holder.find('.pp_social').html(facebook_like_link); }
            if ($ppt.is(':hidden')) $ppt.css('opacity', 0).show(); $pp_overlay.show().fadeTo(settings.animation_speed, settings.opacity); $pp_pic_holder.find('.currentTextHolder').text((set_position + 1) + settings.counter_separator_label + $(pp_images).size()); if (typeof pp_descriptions[set_position] != 'undefined' && pp_descriptions[set_position] != "") { $pp_pic_holder.find('.pp_description').show().html(unescape(pp_descriptions[set_position])); } else { $pp_pic_holder.find('.pp_description').hide(); }
            movie_width = (parseFloat(getParam('width', pp_images[set_position]))) ? getParam('width', pp_images[set_position]) : settings.default_width.toString(); movie_height = (parseFloat(getParam('height', pp_images[set_position]))) ? getParam('height', pp_images[set_position]) : settings.default_height.toString(); percentBased = false; if (movie_height.indexOf('%') != -1) { movie_height = parseFloat(($(window).height() * parseFloat(movie_height) / 100) - 150); percentBased = true; }
            if (movie_width.indexOf('%') != -1) { movie_width = parseFloat(($(window).width() * parseFloat(movie_width) / 100) - 150); percentBased = true; }
            $pp_pic_holder.fadeIn(function () {
                (settings.show_title && pp_titles[set_position] != "" && typeof pp_titles[set_position] != "undefined") ? $ppt.html(unescape(pp_titles[set_position])) : $ppt.html('&nbsp;'); imgPreloader = ""; skipInjection = false; switch (_getFileType(pp_images[set_position])) {
                    case 'image': imgPreloader = new Image(); nextImage = new Image(); if (isSet && set_position < $(pp_images).size() - 1) nextImage.src = pp_images[set_position + 1]; prevImage = new Image(); if (isSet && pp_images[set_position - 1]) prevImage.src = pp_images[set_position - 1]; $pp_pic_holder.find('#pp_full_res')[0].innerHTML = settings.image_markup.replace(/{path}/g, pp_images[set_position]); imgPreloader.onload = function () { pp_dimensions = _fitToViewport(imgPreloader.width, imgPreloader.height); _showContent(); }; imgPreloader.onerror = function () { alert('Image cannot be loaded. Make sure the path is correct and image exist.'); $.prettyPhoto.close(); }; imgPreloader.src = pp_images[set_position]; break; case 'youtube': pp_dimensions = _fitToViewport(movie_width, movie_height); movie_id = getParam('v', pp_images[set_position]); if (movie_id == "") {
                            movie_id = pp_images[set_position].split('youtu.be/'); movie_id = movie_id[1]; if (movie_id.indexOf('?') > 0)
                                movie_id = movie_id.substr(0, movie_id.indexOf('?')); if (movie_id.indexOf('&') > 0)
                                movie_id = movie_id.substr(0, movie_id.indexOf('&'));
                        }
                        movie = 'http://www.youtube.com/embed/' + movie_id; (getParam('rel', pp_images[set_position])) ? movie += "?rel=" + getParam('rel', pp_images[set_position]) : movie += "?rel=1"; if (settings.autoplay) movie += "&autoplay=1"; toInject = settings.iframe_markup.replace(/{width}/g, pp_dimensions['width']).replace(/{height}/g, pp_dimensions['height']).replace(/{wmode}/g, settings.wmode).replace(/{path}/g, movie); break; case 'vimeo': pp_dimensions = _fitToViewport(movie_width, movie_height); movie_id = pp_images[set_position]; var regExp = /http:\/\/(www\.)?vimeo.com\/(\d+)/; var match = movie_id.match(regExp); movie = 'http://player.vimeo.com/video/' + match[2] + '?title=0&amp;byline=0&amp;portrait=0'; if (settings.autoplay) movie += "&autoplay=1;"; vimeo_width = pp_dimensions['width'] + '/embed/?moog_width=' + pp_dimensions['width']; toInject = settings.iframe_markup.replace(/{width}/g, vimeo_width).replace(/{height}/g, pp_dimensions['height']).replace(/{path}/g, movie); break; case 'quicktime': pp_dimensions = _fitToViewport(movie_width, movie_height); pp_dimensions['height'] += 15; pp_dimensions['contentHeight'] += 15; pp_dimensions['containerHeight'] += 15; toInject = settings.quicktime_markup.replace(/{width}/g, pp_dimensions['width']).replace(/{height}/g, pp_dimensions['height']).replace(/{wmode}/g, settings.wmode).replace(/{path}/g, pp_images[set_position]).replace(/{autoplay}/g, settings.autoplay); break; case 'flash': pp_dimensions = _fitToViewport(movie_width, movie_height); flash_vars = pp_images[set_position]; flash_vars = flash_vars.substring(pp_images[set_position].indexOf('flashvars') + 10, pp_images[set_position].length); filename = pp_images[set_position]; filename = filename.substring(0, filename.indexOf('?')); toInject = settings.flash_markup.replace(/{width}/g, pp_dimensions['width']).replace(/{height}/g, pp_dimensions['height']).replace(/{wmode}/g, settings.wmode).replace(/{path}/g, filename + '?' + flash_vars); break; case 'iframe': pp_dimensions = _fitToViewport(movie_width, movie_height); frame_url = pp_images[set_position]; frame_url = frame_url.substr(0, frame_url.indexOf('iframe') - 1); toInject = settings.iframe_markup.replace(/{width}/g, pp_dimensions['width']).replace(/{height}/g, pp_dimensions['height']).replace(/{path}/g, frame_url); break; case 'ajax': doresize = false; pp_dimensions = _fitToViewport(movie_width, movie_height); doresize = true; skipInjection = true; $.get(pp_images[set_position], function (responseHTML) { toInject = settings.inline_markup.replace(/{content}/g, responseHTML); $pp_pic_holder.find('#pp_full_res')[0].innerHTML = toInject; _showContent(); }); break; case 'custom': pp_dimensions = _fitToViewport(movie_width, movie_height); toInject = settings.custom_markup; break; case 'inline': myClone = $(pp_images[set_position]).clone().append('<br clear="all" />').css({ 'width': settings.default_width }).wrapInner('<div id="pp_full_res"><div class="pp_inline"></div></div>').appendTo($('body')).show(); doresize = false; pp_dimensions = _fitToViewport($(myClone).width(), $(myClone).height()); doresize = true; $(myClone).remove(); toInject = settings.inline_markup.replace(/{content}/g, $(pp_images[set_position]).html()); break;
                }; if (!imgPreloader && !skipInjection) { $pp_pic_holder.find('#pp_full_res')[0].innerHTML = toInject; _showContent(); };
            }); return false;
        }; $.prettyPhoto.changePage = function (direction) {
            currentGalleryPage = 0; if (direction == 'previous') { set_position--; if (set_position < 0) set_position = $(pp_images).size() - 1; } else if (direction == 'next') { set_position++; if (set_position > $(pp_images).size() - 1) set_position = 0; } else { set_position = direction; }; rel_index = set_position; if (!doresize) doresize = true; if (settings.allow_expand) { $('.pp_contract').removeClass('pp_contract').addClass('pp_expand'); }
            _hideContent(function () { $.prettyPhoto.open(); });
        }; $.prettyPhoto.changeGalleryPage = function (direction) { if (direction == 'next') { currentGalleryPage++; if (currentGalleryPage > totalPage) currentGalleryPage = 0; } else if (direction == 'previous') { currentGalleryPage--; if (currentGalleryPage < 0) currentGalleryPage = totalPage; } else { currentGalleryPage = direction; }; slide_speed = (direction == 'next' || direction == 'previous') ? settings.animation_speed : 0; slide_to = currentGalleryPage * (itemsPerPage * itemWidth); $pp_gallery.find('ul').animate({ left: -slide_to }, slide_speed); }; $.prettyPhoto.startSlideshow = function () { if (typeof pp_slideshow == 'undefined') { $pp_pic_holder.find('.pp_play').unbind('click').removeClass('pp_play').addClass('pp_pause').click(function () { $.prettyPhoto.stopSlideshow(); return false; }); pp_slideshow = setInterval($.prettyPhoto.startSlideshow, settings.slideshow); } else { $.prettyPhoto.changePage('next'); }; }
        $.prettyPhoto.stopSlideshow = function () { $pp_pic_holder.find('.pp_pause').unbind('click').removeClass('pp_pause').addClass('pp_play').click(function () { $.prettyPhoto.startSlideshow(); return false; }); clearInterval(pp_slideshow); pp_slideshow = undefined; }
        $.prettyPhoto.close = function () { if ($pp_overlay.is(":animated")) return; $.prettyPhoto.stopSlideshow(); $pp_pic_holder.stop().find('object,embed').css('visibility', 'hidden'); $('div.pp_pic_holder,div.ppt,.pp_fade').fadeOut(settings.animation_speed, function () { $(this).remove(); }); $pp_overlay.fadeOut(settings.animation_speed, function () { if ($.browser.msie && $.browser.version == 6) $('select').css('visibility', 'visible'); if (settings.hideflash) $('object,embed,iframe[src*=youtube],iframe[src*=vimeo]').css('visibility', 'visible'); $(this).remove(); $(window).unbind('scroll.prettyphoto'); clearHashtag(); settings.callback(); doresize = true; pp_open = false; delete settings; }); }; function _showContent() {
            $('.pp_loaderIcon').hide(); projectedTop = scroll_pos['scrollTop'] + ((windowHeight / 2) - (pp_dimensions['containerHeight'] / 2)); if (projectedTop < 0) projectedTop = 0; $ppt.fadeTo(settings.animation_speed, 1); $pp_pic_holder.find('.pp_content').animate({ height: pp_dimensions['contentHeight'], width: pp_dimensions['contentWidth'] }, settings.animation_speed); $pp_pic_holder.animate({ 'top': projectedTop, 'left': ((windowWidth / 2) - (pp_dimensions['containerWidth'] / 2) < 0) ? 0 : (windowWidth / 2) - (pp_dimensions['containerWidth'] / 2), width: pp_dimensions['containerWidth'] }, settings.animation_speed, function () {
                $pp_pic_holder.find('.pp_hoverContainer,#fullResImage').height(pp_dimensions['height']).width(pp_dimensions['width']); $pp_pic_holder.find('.pp_fade').fadeIn(settings.animation_speed); if (isSet && _getFileType(pp_images[set_position]) == "image") { $pp_pic_holder.find('.pp_hoverContainer').show(); } else { $pp_pic_holder.find('.pp_hoverContainer').hide(); }
                if (settings.allow_expand) { if (pp_dimensions['resized']) { $('a.pp_expand,a.pp_contract').show(); } else { $('a.pp_expand').hide(); } }
                if (settings.autoplay_slideshow && !pp_slideshow && !pp_open) $.prettyPhoto.startSlideshow(); settings.changepicturecallback(); pp_open = true;
            }); _insert_gallery(); pp_settings.ajaxcallback();
        }; function _hideContent(callback) { $pp_pic_holder.find('#pp_full_res object,#pp_full_res embed').css('visibility', 'hidden'); $pp_pic_holder.find('.pp_fade').fadeOut(settings.animation_speed, function () { $('.pp_loaderIcon').show(); callback(); }); }; function _checkPosition(setCount) { (setCount > 1) ? $('.pp_nav').show() : $('.pp_nav').hide(); }; function _fitToViewport(width, height) { resized = false; _getDimensions(width, height); imageWidth = width, imageHeight = height; if (((pp_containerWidth > windowWidth) || (pp_containerHeight > windowHeight)) && doresize && settings.allow_resize && !percentBased) { resized = true, fitting = false; while (!fitting) { if ((pp_containerWidth > windowWidth)) { imageWidth = (windowWidth - 200); imageHeight = (height / width) * imageWidth; } else if ((pp_containerHeight > windowHeight)) { imageHeight = (windowHeight - 200); imageWidth = (width / height) * imageHeight; } else { fitting = true; }; pp_containerHeight = imageHeight, pp_containerWidth = imageWidth; }; _getDimensions(imageWidth, imageHeight); if ((pp_containerWidth > windowWidth) || (pp_containerHeight > windowHeight)) { _fitToViewport(pp_containerWidth, pp_containerHeight) }; }; return { width: Math.floor(imageWidth), height: Math.floor(imageHeight), containerHeight: Math.floor(pp_containerHeight), containerWidth: Math.floor(pp_containerWidth) + (settings.horizontal_padding * 2), contentHeight: Math.floor(pp_contentHeight), contentWidth: Math.floor(pp_contentWidth), resized: resized }; }; function _getDimensions(width, height) { width = parseFloat(width); height = parseFloat(height); $pp_details = $pp_pic_holder.find('.pp_details'); $pp_details.width(width); detailsHeight = parseFloat($pp_details.css('marginTop')) + parseFloat($pp_details.css('marginBottom')); $pp_details = $pp_details.clone().addClass(settings.theme).width(width).appendTo($('body')).css({ 'position': 'absolute', 'top': -10000 }); detailsHeight += $pp_details.height(); detailsHeight = (detailsHeight <= 34) ? 36 : detailsHeight; if ($.browser.msie && $.browser.version == 7) detailsHeight += 8; $pp_details.remove(); $pp_title = $pp_pic_holder.find('.ppt'); $pp_title.width(width); titleHeight = parseFloat($pp_title.css('marginTop')) + parseFloat($pp_title.css('marginBottom')); $pp_title = $pp_title.clone().appendTo($('body')).css({ 'position': 'absolute', 'top': -10000 }); titleHeight += $pp_title.height(); $pp_title.remove(); pp_contentHeight = height + detailsHeight; pp_contentWidth = width; pp_containerHeight = pp_contentHeight + titleHeight + $pp_pic_holder.find('.pp_top').height() + $pp_pic_holder.find('.pp_bottom').height(); pp_containerWidth = width; }
        function _getFileType(itemSrc) { if (itemSrc.match(/youtube\.com\/watch/i) || itemSrc.match(/youtu\.be/i)) { return 'youtube'; } else if (itemSrc.match(/vimeo\.com/i)) { return 'vimeo'; } else if (itemSrc.match(/\b.mov\b/i)) { return 'quicktime'; } else if (itemSrc.match(/\b.swf\b/i)) { return 'flash'; } else if (itemSrc.match(/\biframe=true\b/i)) { return 'iframe'; } else if (itemSrc.match(/\bajax=true\b/i)) { return 'ajax'; } else if (itemSrc.match(/\bcustom=true\b/i)) { return 'custom'; } else if (itemSrc.substr(0, 1) == '#') { return 'inline'; } else { return 'image'; }; }; function _center_overlay() {
            if (doresize && typeof $pp_pic_holder != 'undefined') {
                scroll_pos = _get_scroll(); contentHeight = $pp_pic_holder.height(), contentwidth = $pp_pic_holder.width(); projectedTop = (windowHeight / 2) + scroll_pos['scrollTop'] - (contentHeight / 2); if (projectedTop < 0) projectedTop = 0; if (contentHeight > windowHeight)
                    return; $pp_pic_holder.css({ 'top': projectedTop, 'left': (windowWidth / 2) + scroll_pos['scrollLeft'] - (contentwidth / 2) });
            };
        }; function _get_scroll() { if (self.pageYOffset) { return { scrollTop: self.pageYOffset, scrollLeft: self.pageXOffset }; } else if (document.documentElement && document.documentElement.scrollTop) { return { scrollTop: document.documentElement.scrollTop, scrollLeft: document.documentElement.scrollLeft }; } else if (document.body) { return { scrollTop: document.body.scrollTop, scrollLeft: document.body.scrollLeft }; }; }; function _resize_overlay() { windowHeight = $(window).height(), windowWidth = $(window).width(); if (typeof $pp_overlay != "undefined") $pp_overlay.height($(document).height()).width(windowWidth); }; function _insert_gallery() { if (isSet && settings.overlay_gallery && _getFileType(pp_images[set_position]) == "image" && (settings.ie6_fallback && !($.browser.msie && parseInt($.browser.version) == 6))) { itemWidth = 52 + 5; navWidth = (settings.theme == "facebook" || settings.theme == "pp_default") ? 50 : 30; itemsPerPage = Math.floor((pp_dimensions['containerWidth'] - 100 - navWidth) / itemWidth); itemsPerPage = (itemsPerPage < pp_images.length) ? itemsPerPage : pp_images.length; totalPage = Math.ceil(pp_images.length / itemsPerPage) - 1; if (totalPage == 0) { navWidth = 0; $pp_gallery.find('.pp_arrow_next,.pp_arrow_previous').hide(); } else { $pp_gallery.find('.pp_arrow_next,.pp_arrow_previous').show(); }; galleryWidth = itemsPerPage * itemWidth; fullGalleryWidth = pp_images.length * itemWidth; $pp_gallery.css('margin-left', -((galleryWidth / 2) + (navWidth / 2))).find('div:first').width(galleryWidth + 5).find('ul').width(fullGalleryWidth).find('li.selected').removeClass('selected'); goToPage = (Math.floor(set_position / itemsPerPage) < totalPage) ? Math.floor(set_position / itemsPerPage) : totalPage; $.prettyPhoto.changeGalleryPage(goToPage); $pp_gallery_li.filter(':eq(' + set_position + ')').addClass('selected'); } else { $pp_pic_holder.find('.pp_content').unbind('mouseenter mouseleave'); } }
        function _build_overlay(caller) {
            if (settings.social_tools)
                facebook_like_link = settings.social_tools.replace('{location_href}', encodeURIComponent(location.href)); settings.markup = settings.markup.replace('{pp_social}', ''); $('body').append(settings.markup); $pp_pic_holder = $('.pp_pic_holder'), $ppt = $('.ppt'), $pp_overlay = $('div.pp_overlay'); if (isSet && settings.overlay_gallery) {
                currentGalleryPage = 0; toInject = ""; for (var i = 0; i < pp_images.length; i++) {
                    if (!pp_images[i].match(/\b(jpg|jpeg|png|gif)\b/gi)) { classname = 'default'; img_src = ''; } else { classname = ''; img_src = pp_images[i]; }
                    toInject += "<li class='" + classname + "'><a href='#'><img src='" + img_src + "' width='50' alt='' /></a></li>";
                }; toInject = settings.gallery_markup.replace(/{gallery}/g, toInject); $pp_pic_holder.find('#pp_full_res').after(toInject); $pp_gallery = $('.pp_pic_holder .pp_gallery'), $pp_gallery_li = $pp_gallery.find('li'); $pp_gallery.find('.pp_arrow_next').click(function () { $.prettyPhoto.changeGalleryPage('next'); $.prettyPhoto.stopSlideshow(); return false; }); $pp_gallery.find('.pp_arrow_previous').click(function () { $.prettyPhoto.changeGalleryPage('previous'); $.prettyPhoto.stopSlideshow(); return false; }); $pp_pic_holder.find('.pp_content').hover(function () { $pp_pic_holder.find('.pp_gallery:not(.disabled)').fadeIn(); }, function () { $pp_pic_holder.find('.pp_gallery:not(.disabled)').fadeOut(); }); itemWidth = 52 + 5; $pp_gallery_li.each(function (i) { $(this).find('a').click(function () { $.prettyPhoto.changePage(i); $.prettyPhoto.stopSlideshow(); return false; }); });
            }; if (settings.slideshow) {
                $pp_pic_holder.find('.pp_nav').prepend('<a href="#" class="pp_play">Play</a>')
                $pp_pic_holder.find('.pp_nav .pp_play').click(function () { $.prettyPhoto.startSlideshow(); return false; });
            }
            $pp_pic_holder.attr('class', 'pp_pic_holder ' + settings.theme); $pp_overlay.css({ 'opacity': 0, 'height': $(document).height(), 'width': $(window).width() }).bind('click', function () { if (!settings.modal) $.prettyPhoto.close(); }); $('a.pp_close').bind('click', function () { $.prettyPhoto.close(); return false; }); if (settings.allow_expand) { $('a.pp_expand').bind('click', function (e) { if ($(this).hasClass('pp_expand')) { $(this).removeClass('pp_expand').addClass('pp_contract'); doresize = false; } else { $(this).removeClass('pp_contract').addClass('pp_expand'); doresize = true; }; _hideContent(function () { $.prettyPhoto.open(); }); return false; }); }
            $pp_pic_holder.find('.pp_previous, .pp_nav .pp_arrow_previous').bind('click', function () { $.prettyPhoto.changePage('previous'); $.prettyPhoto.stopSlideshow(); return false; }); $pp_pic_holder.find('.pp_next, .pp_nav .pp_arrow_next').bind('click', function () { $.prettyPhoto.changePage('next'); $.prettyPhoto.stopSlideshow(); return false; }); _center_overlay();
        }; if (!pp_alreadyInitialized && getHashtag()) { pp_alreadyInitialized = true; hashIndex = getHashtag(); hashRel = hashIndex; hashIndex = hashIndex.substring(hashIndex.indexOf('/') + 1, hashIndex.length - 1); hashRel = hashRel.substring(0, hashRel.indexOf('/')); setTimeout(function () { $("a[" + pp_settings.hook + "^='" + hashRel + "']:eq(" + hashIndex + ")").trigger('click'); }, 50); }
        return this.unbind('click.prettyphoto').bind('click.prettyphoto', $.prettyPhoto.initialize);
    }; function getHashtag() { url = location.href; hashtag = (url.indexOf('#prettyPhoto') !== -1) ? decodeURI(url.substring(url.indexOf('#prettyPhoto') + 1, url.length)) : false; return hashtag; }; function setHashtag() { if (typeof theRel == 'undefined') return; location.hash = theRel + '/' + rel_index + '/'; }; function clearHashtag() { if (location.href.indexOf('#prettyPhoto') !== -1) location.hash = "prettyPhoto"; }
    function getParam(name, url) { name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]"); var regexS = "[\\?&]" + name + "=([^&#]*)"; var regex = new RegExp(regexS); var results = regex.exec(url); return (results == null) ? "" : results[1]; }
})(jQuery); var pp_alreadyInitialized = false;

//PrettyPhoto Ends

//fitvids jquery Plugin
/*global jQuery */
/*! 
* FitVids 1.0
*
* Copyright 2011, Chris Coyier - http://css-tricks.com + Dave Rupert - http://daverupert.com
* Credit to Thierry Koblentz - http://www.alistapart.com/articles/creating-intrinsic-ratios-for-video/
* Released under the WTFPL license - http://sam.zoy.org/wtfpl/
*
* Date: Thu Sept 01 18:00:00 2011 -0500
*/

(function ($) {

    $.fn.fitVids = function (options) {
        var settings = {
            customSelector: null
        }

        var div = document.createElement('div'),
        ref = document.getElementsByTagName('base')[0] || document.getElementsByTagName('script')[0];

        div.className = 'fit-vids-style';
        div.innerHTML = '&shy;<style>         \
      .fluid-width-video-wrapper {        \
         width: 100%;                     \
         position: relative;              \
         padding: 0;                      \
      }                                   \
                                          \
      .fluid-width-video-wrapper iframe,  \
      .fluid-width-video-wrapper object,  \
      .fluid-width-video-wrapper embed {  \
         position: absolute;              \
         top: 0;                          \
         left: 0;                         \
         width: 100%;                     \
         height: 100%;                    \
      }                                   \
    </style>';

        ref.parentNode.insertBefore(div, ref);

        if (options) {
            $.extend(settings, options);
        }

        return this.each(function () {
            var selectors = [
        "iframe[src^='http://player.vimeo.com']",
        "iframe[src^='http://www.youtube.com']",
        "iframe[src^='https://www.youtube.com']",
        "iframe[src^='http://www.kickstarter.com']",
        "object",
        "embed"
      ];

            if (settings.customSelector) {
                selectors.push(settings.customSelector);
            }

            var $allVideos = $(this).find(selectors.join(','));

            $allVideos.each(function () {
                var $this = $(this);
                if (this.tagName.toLowerCase() == 'embed' && $this.parent('object').length || $this.parent('.fluid-width-video-wrapper').length) { return; }
                var height = this.tagName.toLowerCase() == 'object' ? $this.attr('height') : $this.height(),
            aspectRatio = height / $this.width();
                if (!$this.attr('id')) {
                    var videoID = 'fitvid' + Math.floor(Math.random() * 999999);
                    $this.attr('id', videoID);
                }
                $this.wrap('<div class="fluid-width-video-wrapper"></div>').parent('.fluid-width-video-wrapper').css('padding-top', (aspectRatio * 100) + "%");
                $this.removeAttr('height').removeAttr('width');
            });
        });

    }
})(jQuery);
//fitvids jquery plugin ends here
//slides jquery plugin
/*
* Slides, A Slideshow Plugin for jQuery
* Intructions: http://slidesjs.com
* By: Nathan Searles, http://nathansearles.com
* Version: 1.1.9
* Updated: September 5th, 2011
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
(function ($) {
    $.fn.slides = function (option) {
        // override defaults with specified option
        option = $.extend({}, $.fn.slides.option, option);

        return this.each(function () {
            // wrap slides in control container, make sure slides are block level
            $('.' + option.container, $(this)).children().wrapAll('<div class="slides_control"/>');

            var elem = $(this),
				control = $('.slides_control', elem),
				total = control.children().size(),
				width = control.children().outerWidth(),
				height = control.children().outerHeight(),
				start = option.start - 1,
				effect = option.effect.indexOf(',') < 0 ? option.effect : option.effect.replace(' ', '').split(',')[0],
				paginationEffect = option.effect.indexOf(',') < 0 ? effect : option.effect.replace(' ', '').split(',')[1],
				next = 0, prev = 0, number = 0, current = 0, loaded, active, clicked, position, direction, imageParent, pauseTimeout, playInterval;

            // is there only one slide?
            if (total < 2) {
                // Fade in .slides_container
                $('.' + option.container, $(this)).fadeIn(option.fadeSpeed, option.fadeEasing, function () {
                    // let the script know everything is loaded
                    loaded = true;
                    // call the loaded funciton
                    option.slidesLoaded();
                });
                // Hide the next/previous buttons
                $('.' + option.next + ', .' + option.prev).fadeOut(0);
                return false;
            }

            // animate slides
            function animate(direction, effect, clicked) {
                if (!active && loaded) {
                    active = true;
                    // start of animation

                    switch (direction) {
                        case 'next':
                            // change current slide to previous
                            prev = current;
                            // get next from current + 1
                            next = current + 1;
                            // if last slide, set next to first slide
                            next = total === next ? 0 : next;
                            // set position of next slide to right of previous
                            position = width * 2;
                            // distance to slide based on width of slides
                            direction = -width * 2;
                            // store new current slide
                            current = next;
                            break;
                        case 'prev':
                            // change current slide to previous
                            prev = current;
                            // get next from current - 1
                            next = current - 1;
                            // if first slide, set next to last slide
                            next = next === -1 ? total - 1 : next;
                            // set position of next slide to left of previous
                            position = 0;
                            // distance to slide based on width of slides
                            direction = 0;
                            // store new current slide
                            current = next;
                            break;
                        case 'pagination':
                            // get next from pagination item clicked, convert to number
                            next = parseInt(clicked, 10);
                            // get previous from pagination item with class of current
                            prev = $('.' + option.paginationClass + ' li.' + option.currentClass + ' a', elem).attr('href').match('[^#/]+$');
                            // if next is greater then previous set position of next slide to right of previous
                            if (next > prev) {
                                position = width * 2;
                                direction = -width * 2;
                            } else {
                                // if next is less then previous set position of next slide to left of previous
                                position = 0;
                                direction = 0;
                            }
                            // store new current slide
                            current = next;
                            break;
                    }

                    option.animationStart(current);

                    // fade animation
                    if (effect === 'fade') {
                        // fade animation with crossfade
                        if (option.crossfade) {
                            // put hidden next above current
                            control.children(':eq(' + next + ')', elem).css({
                                zIndex: 10
                                // fade in next
                            }).fadeIn(option.fadeSpeed, option.fadeEasing, function () {
                                if (option.autoHeight) {
                                    // animate container to height of next
                                    control.animate({
                                        height: control.children(':eq(' + next + ')', elem).outerHeight()
                                    }, option.autoHeightSpeed, function () {
                                        // hide previous
                                        control.children(':eq(' + prev + ')', elem).css({
                                            display: 'none',
                                            zIndex: 0
                                        });
                                        // reset z index
                                        control.children(':eq(' + next + ')', elem).css({
                                            zIndex: 0
                                        });
                                        // end of animation
                                        option.animationComplete(next + 1);
                                        active = false;
                                    });
                                } else {
                                    // hide previous
                                    control.children(':eq(' + prev + ')', elem).css({
                                        display: 'none',
                                        zIndex: 0
                                    });
                                    // reset zindex
                                    control.children(':eq(' + next + ')', elem).css({
                                        zIndex: 0
                                    });
                                    // end of animation
                                    option.animationComplete(next + 1);
                                    active = false;
                                }
                            });
                        } else {
                            // fade animation with no crossfade
                            control.children(':eq(' + prev + ')', elem).fadeOut(option.fadeSpeed, option.fadeEasing, function () {
                                // animate to new height
                                if (option.autoHeight) {
                                    control.animate({
                                        // animate container to height of next
                                        height: control.children(':eq(' + next + ')', elem).outerHeight()
                                    }, option.autoHeightSpeed,
                                    // fade in next slide
									function () {
									    control.children(':eq(' + next + ')', elem).fadeIn(option.fadeSpeed, option.fadeEasing);
									});
                                } else {
                                    // if fixed height
                                    control.children(':eq(' + next + ')', elem).fadeIn(option.fadeSpeed, option.fadeEasing, function () {
                                        // fix font rendering in ie, lame
                                        if ($.browser.msie) {
                                            $(this).get(0).style.removeAttribute('filter');
                                        }
                                    });
                                }
                                // end of animation
                                option.animationComplete(next + 1);
                                active = false;
                            });
                        }
                        // slide animation
                    } else {
                        // move next slide to right of previous
                        control.children(':eq(' + next + ')').css({
                            left: position,
                            display: 'block'
                        });
                        // animate to new height
                        if (option.autoHeight) {
                            control.animate({
                                left: direction,
                                height: control.children(':eq(' + next + ')').outerHeight()
                            }, option.slideSpeed, option.slideEasing, function () {
                                control.css({
                                    left: -width
                                });
                                control.children(':eq(' + next + ')').css({
                                    left: width,
                                    zIndex: 5
                                });
                                // reset previous slide
                                control.children(':eq(' + prev + ')').css({
                                    left: width,
                                    display: 'none',
                                    zIndex: 0
                                });
                                // end of animation
                                option.animationComplete(next + 1);
                                active = false;
                            });
                            // if fixed height
                        } else {
                            // animate control
                            control.animate({
                                left: direction
                            }, option.slideSpeed, option.slideEasing, function () {
                                // after animation reset control position
                                control.css({
                                    left: -width
                                });
                                // reset and show next
                                control.children(':eq(' + next + ')').css({
                                    left: width,
                                    zIndex: 5
                                });
                                // reset previous slide
                                control.children(':eq(' + prev + ')').css({
                                    left: width,
                                    display: 'none',
                                    zIndex: 0
                                });
                                // end of animation
                                option.animationComplete(next + 1);
                                active = false;
                            });
                        }
                    }
                    // set current state for pagination
                    if (option.pagination) {
                        // remove current class from all
                        $('.' + option.paginationClass + ' li.' + option.currentClass, elem).removeClass(option.currentClass);
                        // add current class to next
                        $('.' + option.paginationClass + ' li:eq(' + next + ')', elem).addClass(option.currentClass);
                    }
                }
            } // end animate function

            function stop() {
                // clear interval from stored id
                clearInterval(elem.data('interval'));
            }

            function pause() {
                if (option.pause) {
                    // clear timeout and interval
                    clearTimeout(elem.data('pause'));
                    clearInterval(elem.data('interval'));
                    // pause slide show for option.pause amount
                    pauseTimeout = setTimeout(function () {
                        // clear pause timeout
                        clearTimeout(elem.data('pause'));
                        // start play interval after pause
                        playInterval = setInterval(function () {
                            animate("next", effect);
                        }, option.play);
                        // store play interval
                        elem.data('interval', playInterval);
                    }, option.pause);
                    // store pause interval
                    elem.data('pause', pauseTimeout);
                } else {
                    // if no pause, just stop
                    stop();
                }
            }

            // 2 or more slides required
            if (total < 2) {
                return;
            }

            // error corection for start slide
            if (start < 0) {
                start = 0;
            }

            if (start > total) {
                start = total - 1;
            }

            // change current based on start option number
            if (option.start) {
                current = start;
            }

            // randomizes slide order
            if (option.randomize) {
                control.randomize();
            }

            // make sure overflow is hidden, width is set
            $('.' + option.container, elem).css({
                overflow: 'hidden',
                // fix for ie
                position: 'relative'
            });

            // set css for slides
            control.children().css({
                position: 'absolute',
                top: 0,
                left: control.children().outerWidth(),
                zIndex: 0,
                display: 'none'
            });

            // set css for control div
            control.css({
                position: 'relative',
                // size of control 3 x slide width
                width: (width * 3),
                // set height to slide height
                height: height,
                // center control to slide
                left: -width
            });

            // show slides
            $('.' + option.container, elem).css({
                display: 'block'
            });

            // if autoHeight true, get and set height of first slide
            if (option.autoHeight) {
                control.children().css({
                    height: 'auto'
                });
                control.animate({
                    height: control.children(':eq(' + start + ')').outerHeight()
                }, option.autoHeightSpeed);
            }

            // checks if image is loaded
            if (option.preload && control.find('img:eq(' + start + ')').length) {
                // adds preload image
                $('.' + option.container, elem).css({
                    background: 'url(' + option.preloadImage + ') no-repeat 50% 50%'
                });

                // gets image src, with cache buster
                var img = control.find('img:eq(' + start + ')').attr('src') + '?' + (new Date()).getTime();

                // check if the image has a parent
                if ($('img', elem).parent().attr('class') != 'slides_control') {
                    // If image has parent, get tag name
                    imageParent = control.children(':eq(0)')[0].tagName.toLowerCase();
                } else {
                    // Image doesn't have parent, use image tag name
                    imageParent = control.find('img:eq(' + start + ')');
                }

                // checks if image is loaded
                control.find('img:eq(' + start + ')').attr('src', img).load(function () {
                    // once image is fully loaded, fade in
                    control.find(imageParent + ':eq(' + start + ')').fadeIn(option.fadeSpeed, option.fadeEasing, function () {
                        $(this).css({
                            zIndex: 5
                        });
                        // removes preload image
                        $('.' + option.container, elem).css({
                            background: ''
                        });
                        // let the script know everything is loaded
                        loaded = true;
                        // call the loaded funciton
                        option.slidesLoaded();
                    });
                });
            } else {
                // if no preloader fade in start slide
                control.children(':eq(' + start + ')').fadeIn(option.fadeSpeed, option.fadeEasing, function () {
                    // let the script know everything is loaded
                    loaded = true;
                    // call the loaded funciton
                    option.slidesLoaded();
                });
            }

            // click slide for next
            if (option.bigTarget) {
                // set cursor to pointer
                control.children().css({
                    cursor: 'pointer'
                });
                // click handler
                control.children().click(function () {
                    // animate to next on slide click
                    animate('next', effect);
                    return false;
                });
            }

            // pause on mouseover
            if (option.hoverPause && option.play) {
                control.bind('mouseover', function () {
                    // on mouse over stop
                    stop();
                });
                control.bind('mouseleave', function () {
                    // on mouse leave start pause timeout
                    pause();
                });
            }

            // generate next/prev buttons
            if (option.generateNextPrev) {
                $('.' + option.container, elem).after('<a href="#" class="' + option.prev + '">Prev</a>');
                $('.' + option.prev, elem).after('<a href="#" class="' + option.next + '">Next</a>');
            }

            // next button
            $('.' + option.next, elem).click(function (e) {
                e.preventDefault();
                if (option.play) {
                    pause();
                }
                animate('next', effect);
            });

            // previous button
            $('.' + option.prev, elem).click(function (e) {
                e.preventDefault();
                if (option.play) {
                    pause();
                }
                animate('prev', effect);
            });

            // generate pagination
            if (option.generatePagination) {
                // create unordered list
                if (option.prependPagination) {
                    elem.prepend('<ul class=' + option.paginationClass + '></ul>');
                } else {
                    elem.append('<ul class=' + option.paginationClass + '></ul>');
                }
                // for each slide create a list item and link
                control.children().each(function () {
                    $('.' + option.paginationClass, elem).append('<li><a href="#' + number + '">' + (number + 1) + '</a></li>');
                    number++;
                });
            } else {
                // if pagination exists, add href w/ value of item number to links
                $('.' + option.paginationClass + ' li a', elem).each(function () {
                    $(this).attr('href', '#' + number);
                    number++;
                });
            }

            // add current class to start slide pagination
            $('.' + option.paginationClass + ' li:eq(' + start + ')', elem).addClass(option.currentClass);

            // click handling 
            $('.' + option.paginationClass + ' li a', elem).click(function () {
                // pause slideshow
                if (option.play) {
                    pause();
                }
                // get clicked, pass to animate function					
                clicked = $(this).attr('href').match('[^#/]+$');
                // if current slide equals clicked, don't do anything
                if (current != clicked) {
                    animate('pagination', paginationEffect, clicked);
                }
                return false;
            });

            // click handling 
            $('a.link', elem).click(function () {
                // pause slideshow
                if (option.play) {
                    pause();
                }
                // get clicked, pass to animate function					
                clicked = $(this).attr('href').match('[^#/]+$') - 1;
                // if current slide equals clicked, don't do anything
                if (current != clicked) {
                    animate('pagination', paginationEffect, clicked);
                }
                return false;
            });

            if (option.play) {
                // set interval
                playInterval = setInterval(function () {
                    animate('next', effect);
                }, option.play);
                // store interval id
                elem.data('interval', playInterval);
            }
        });
    };

    // default options
    $.fn.slides.option = {
        preload: false, // boolean, Set true to preload images in an image based slideshow
        preloadImage: '/img/loading.gif', // string, Name and location of loading image for preloader. Default is "/img/loading.gif"
        container: 'slides_container', // string, Class name for slides container. Default is "slides_container"
        generateNextPrev: false, // boolean, Auto generate next/prev buttons
        next: 'next', // string, Class name for next button
        prev: 'prev', // string, Class name for previous button
        pagination: true, // boolean, If you're not using pagination you can set to false, but don't have to
        generatePagination: true, // boolean, Auto generate pagination
        prependPagination: false, // boolean, prepend pagination
        paginationClass: 'pagination', // string, Class name for pagination
        currentClass: 'current', // string, Class name for current class
        fadeSpeed: 350, // number, Set the speed of the fading animation in milliseconds
        fadeEasing: '', // string, must load jQuery's easing plugin before http://gsgd.co.uk/sandbox/jquery/easing/
        slideSpeed: 350, // number, Set the speed of the sliding animation in milliseconds
        slideEasing: '', // string, must load jQuery's easing plugin before http://gsgd.co.uk/sandbox/jquery/easing/
        start: 1, // number, Set the speed of the sliding animation in milliseconds
        effect: 'slide', // string, '[next/prev], [pagination]', e.g. 'slide, fade' or simply 'fade' for both
        crossfade: false, // boolean, Crossfade images in a image based slideshow
        randomize: false, // boolean, Set to true to randomize slides
        play: 0, // number, Autoplay slideshow, a positive number will set to true and be the time between slide animation in milliseconds
        pause: 0, // number, Pause slideshow on click of next/prev or pagination. A positive number will set to true and be the time of pause in milliseconds
        hoverPause: false, // boolean, Set to true and hovering over slideshow will pause it
        autoHeight: false, // boolean, Set to true to auto adjust height
        autoHeightSpeed: 350, // number, Set auto height animation time in milliseconds
        bigTarget: false, // boolean, Set to true and the whole slide will link to next slide on click
        animationStart: function () { }, // Function called at the start of animation
        animationComplete: function () { }, // Function called at the completion of animation
        slidesLoaded: function () { } // Function is called when slides is fully loaded
    };

    // Randomize slide order on load
    $.fn.randomize = function (callback) {
        function randomizeOrder() { return (Math.round(Math.random()) - 0.5); }
        return ($(this).each(function () {
            var $this = $(this);
            var $children = $this.children();
            var childCount = $children.length;
            if (childCount > 1) {
                $children.hide();
                var indices = [];
                for (i = 0; i < childCount; i++) { indices[indices.length] = i; }
                indices = indices.sort(randomizeOrder);
                $.each(indices, function (j, k) {
                    var $child = $children.eq(k);
                    var $clone = $child.clone(true);
                    $clone.show().appendTo($this);
                    if (callback !== undefined) {
                        callback($child, $clone);
                    }
                    $child.remove();
                });
            }
        }));
    };
})(jQuery);
//slides jquery plugins ends
//jquery colors animate
/*
Color animation jQuery-plugin
http://www.bitstorm.org/jquery/color-animation/
Copyright 2011 Edwin Martin <edwin@bitstorm.org>
Released under the MIT and GPL licenses.
*/
(function (d) {
    function i() { var b = d("script:first"), a = b.css("color"), c = false; if (/^rgba/.test(a)) c = true; else try { c = a != b.css("color", "rgba(0, 0, 0, 0.5)").css("color"); b.css("color", a) } catch (e) { } return c } function g(b, a, c) { var e = "rgb" + (d.support.rgba ? "a" : "") + "(" + parseInt(b[0] + c * (a[0] - b[0]), 10) + "," + parseInt(b[1] + c * (a[1] - b[1]), 10) + "," + parseInt(b[2] + c * (a[2] - b[2]), 10); if (d.support.rgba) e += "," + (b && a ? parseFloat(b[3] + c * (a[3] - b[3])) : 1); e += ")"; return e } function f(b) {
        var a, c; if (a = /#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})/.exec(b)) c =
[parseInt(a[1], 16), parseInt(a[2], 16), parseInt(a[3], 16), 1]; else if (a = /#([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])/.exec(b)) c = [parseInt(a[1], 16) * 17, parseInt(a[2], 16) * 17, parseInt(a[3], 16) * 17, 1]; else if (a = /rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/.exec(b)) c = [parseInt(a[1]), parseInt(a[2]), parseInt(a[3]), 1]; else if (a = /rgba\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9\.]*)\s*\)/.exec(b)) c = [parseInt(a[1], 10), parseInt(a[2], 10), parseInt(a[3], 10), parseFloat(a[4])]; return c
    }
    d.extend(true, d, { support: { rgba: i()} }); var h = ["color", "backgroundColor", "borderBottomColor", "borderLeftColor", "borderRightColor", "borderTopColor", "outlineColor"]; d.each(h, function (b, a) { d.fx.step[a] = function (c) { if (!c.init) { c.a = f(d(c.elem).css(a)); c.end = f(c.end); c.init = true } c.elem.style[a] = g(c.a, c.end, c.pos) } }); d.fx.step.borderColor = function (b) { if (!b.init) b.end = f(b.end); var a = h.slice(2, 6); d.each(a, function (c, e) { b.init || (b[e] = { a: f(d(b.elem).css(e)) }); b.elem.style[e] = g(b[e].a, b.end, b.pos) }); b.init = true }
})(jQuery);
//jquery colors animate ends here
//jquery background position animate plugin here
/**
* @author Alexander Farkas
* v. 1.22
*/


(function ($) {
    if (!document.defaultView || !document.defaultView.getComputedStyle) { // IE6-IE8
        var oldCurCSS = $.curCSS;
        $.curCSS = function (elem, name, force) {
            if (name === 'background-position') {
                name = 'backgroundPosition';
            }
            if (name !== 'backgroundPosition' || !elem.currentStyle || elem.currentStyle[name]) {
                return oldCurCSS.apply(this, arguments);
            }
            var style = elem.style;
            if (!force && style && style[name]) {
                return style[name];
            }
            return oldCurCSS(elem, 'backgroundPositionX', force) + ' ' + oldCurCSS(elem, 'backgroundPositionY', force);
        };
    }

    var oldAnim = $.fn.animate;
    $.fn.animate = function (prop) {
        if ('background-position' in prop) {
            prop.backgroundPosition = prop['background-position'];
            delete prop['background-position'];
        }
        if ('backgroundPosition' in prop) {
            prop.backgroundPosition = '(' + prop.backgroundPosition;
        }
        return oldAnim.apply(this, arguments);
    };

    function toArray(strg) {
        strg = strg.replace(/left|top/g, '0px');
        strg = strg.replace(/right|bottom/g, '100%');
        strg = strg.replace(/([0-9\.]+)(\s|\)|$)/g, "$1px$2");
        var res = strg.match(/(-?[0-9\.]+)(px|\%|em|pt)\s(-?[0-9\.]+)(px|\%|em|pt)/);
        return [parseFloat(res[1], 10), res[2], parseFloat(res[3], 10), res[4]];
    }

    $.fx.step.backgroundPosition = function (fx) {
        if (!fx.bgPosReady) {
            var start = $.curCSS(fx.elem, 'backgroundPosition');
            if (!start) {//FF2 no inline-style fallback
                start = '0px 0px';
            }

            start = toArray(start);
            fx.start = [start[0], start[2]];
            var end = toArray(fx.end);
            fx.end = [end[0], end[2]];

            fx.unit = [end[1], end[3]];
            fx.bgPosReady = true;
        }
        //return;
        var nowPosX = [];
        nowPosX[0] = ((fx.end[0] - fx.start[0]) * fx.pos) + fx.start[0] + fx.unit[0];
        nowPosX[1] = ((fx.end[1] - fx.start[1]) * fx.pos) + fx.start[1] + fx.unit[1];
        fx.elem.style.backgroundPosition = nowPosX[0] + ' ' + nowPosX[1];

    };
})(jQuery);
//jquery background position animate plugin ends here
//respond jquery plugin
/*! matchMedia() polyfill - Test a CSS media type/query in JS. Authors & copyright (c) 2012: Scott Jehl, Paul Irish, Nicholas Zakas. Dual MIT/BSD license */
/*! NOTE: If you're already including a window.matchMedia polyfill via Modernizr or otherwise, you don't need this part */
window.matchMedia = window.matchMedia || (function (e, f) { var c, a = e.documentElement, b = a.firstElementChild || a.firstChild, d = e.createElement("body"), g = e.createElement("div"); g.id = "mq-test-1"; g.style.cssText = "position:absolute;top:-100em"; d.appendChild(g); return function (h) { g.innerHTML = '&shy;<style media="' + h + '"> #mq-test-1 { width: 42px; }</style>'; a.insertBefore(d, b); c = g.offsetWidth == 42; a.removeChild(d); return { matches: c, media: h} } })(document);

/*! Respond.js v1.1.0: min/max-width media query polyfill. (c) Scott Jehl. MIT/GPLv2 Lic. j.mp/respondjs  */
(function (e) { e.respond = {}; respond.update = function () { }; respond.mediaQueriesSupported = e.matchMedia && e.matchMedia("only all").matches; if (respond.mediaQueriesSupported) { return } var w = e.document, s = w.documentElement, i = [], k = [], q = [], o = {}, h = 30, f = w.getElementsByTagName("head")[0] || s, g = w.getElementsByTagName("base")[0], b = f.getElementsByTagName("link"), d = [], a = function () { var D = b, y = D.length, B = 0, A, z, C, x; for (; B < y; B++) { A = D[B], z = A.href, C = A.media, x = A.rel && A.rel.toLowerCase() === "stylesheet"; if (!!z && x && !o[z]) { if (A.styleSheet && A.styleSheet.rawCssText) { m(A.styleSheet.rawCssText, z, C); o[z] = true } else { if ((!g && !/^([a-zA-Z:]*\/\/)/.test(z)) || z.replace(RegExp.$1, "").split("/")[0] === e.location.host) { d.push({ href: z, media: C }) } } } } u() }, u = function () { if (d.length) { var x = d.shift(); n(x.href, function (y) { m(y, x.href, x.media); o[x.href] = true; u() }) } }, m = function (I, x, z) { var G = I.match(/@media[^\{]+\{([^\{\}]*\{[^\}\{]*\})+/gi), J = G && G.length || 0, x = x.substring(0, x.lastIndexOf("/")), y = function (K) { return K.replace(/(url\()['"]?([^\/\)'"][^:\)'"]+)['"]?(\))/g, "$1" + x + "$2$3") }, A = !J && z, D = 0, C, E, F, B, H; if (x.length) { x += "/" } if (A) { J = 1 } for (; D < J; D++) { C = 0; if (A) { E = z; k.push(y(I)) } else { E = G[D].match(/@media *([^\{]+)\{([\S\s]+?)$/) && RegExp.$1; k.push(RegExp.$2 && y(RegExp.$2)) } B = E.split(","); H = B.length; for (; C < H; C++) { F = B[C]; i.push({ media: F.split("(")[0].match(/(only\s+)?([a-zA-Z]+)\s?/) && RegExp.$2 || "all", rules: k.length - 1, hasquery: F.indexOf("(") > -1, minw: F.match(/\(min\-width:[\s]*([\s]*[0-9\.]+)(px|em)[\s]*\)/) && parseFloat(RegExp.$1) + (RegExp.$2 || ""), maxw: F.match(/\(max\-width:[\s]*([\s]*[0-9\.]+)(px|em)[\s]*\)/) && parseFloat(RegExp.$1) + (RegExp.$2 || "") }) } } j() }, l, r, v = function () { var z, A = w.createElement("div"), x = w.body, y = false; A.style.cssText = "position:absolute;font-size:1em;width:1em"; if (!x) { x = y = w.createElement("body") } x.appendChild(A); s.insertBefore(x, s.firstChild); z = A.offsetWidth; if (y) { s.removeChild(x) } else { x.removeChild(A) } z = p = parseFloat(z); return z }, p, j = function (I) { var x = "clientWidth", B = s[x], H = w.compatMode === "CSS1Compat" && B || w.body[x] || B, D = {}, G = b[b.length - 1], z = (new Date()).getTime(); if (I && l && z - l < h) { clearTimeout(r); r = setTimeout(j, h); return } else { l = z } for (var E in i) { var K = i[E], C = K.minw, J = K.maxw, A = C === null, L = J === null, y = "em"; if (!!C) { C = parseFloat(C) * (C.indexOf(y) > -1 ? (p || v()) : 1) } if (!!J) { J = parseFloat(J) * (J.indexOf(y) > -1 ? (p || v()) : 1) } if (!K.hasquery || (!A || !L) && (A || H >= C) && (L || H <= J)) { if (!D[K.media]) { D[K.media] = [] } D[K.media].push(k[K.rules]) } } for (var E in q) { if (q[E] && q[E].parentNode === f) { f.removeChild(q[E]) } } for (var E in D) { var M = w.createElement("style"), F = D[E].join("\n"); M.type = "text/css"; M.media = E; f.insertBefore(M, G.nextSibling); if (M.styleSheet) { M.styleSheet.cssText = F } else { M.appendChild(w.createTextNode(F)) } q.push(M) } }, n = function (x, z) { var y = c(); if (!y) { return } y.open("GET", x, true); y.onreadystatechange = function () { if (y.readyState != 4 || y.status != 200 && y.status != 304) { return } z(y.responseText) }; if (y.readyState == 4) { return } y.send(null) }, c = (function () { var x = false; try { x = new XMLHttpRequest() } catch (y) { x = new ActiveXObject("Microsoft.XMLHTTP") } return function () { return x } })(); a(); respond.update = a; function t() { j(true) } if (e.addEventListener) { e.addEventListener("resize", t, false) } else { if (e.attachEvent) { e.attachEvent("onresize", t) } } })(this);
//respond rquery plugin ends here
//jquery touchwipe plugin
/**
* jQuery Plugin to obtain touch gestures from iPhone, iPod Touch and iPad, should also work with Android mobile phones (not tested yet!)
* Common usage: wipe images (left and right to show the previous or next image)
* 
* @author Andreas Waltl, netCU Internetagentur (http://www.netcu.de)
* @version 1.1.1 (9th December 2010) - fix bug (older IE's had problems)
* @version 1.1 (1st September 2010) - support wipe up and wipe down
* @version 1.0 (15th July 2010)
*/
(function ($) { $.fn.touchwipe = function (settings) { var config = { min_move_x: 20, min_move_y: 20, wipeLeft: function () { }, wipeRight: function () { }, wipeUp: function () { }, wipeDown: function () { }, preventDefaultEvents: true }; if (settings) $.extend(config, settings); this.each(function () { var startX; var startY; var isMoving = false; function cancelTouch() { this.removeEventListener('touchmove', onTouchMove); startX = null; isMoving = false } function onTouchMove(e) { if (config.preventDefaultEvents) { e.preventDefault() } if (isMoving) { var x = e.touches[0].pageX; var y = e.touches[0].pageY; var dx = startX - x; var dy = startY - y; if (Math.abs(dx) >= config.min_move_x) { cancelTouch(); if (dx > 0) { config.wipeLeft() } else { config.wipeRight() } } else if (Math.abs(dy) >= config.min_move_y) { cancelTouch(); if (dy > 0) { config.wipeDown() } else { config.wipeUp() } } } } function onTouchStart(e) { if (e.touches.length == 1) { startX = e.touches[0].pageX; startY = e.touches[0].pageY; isMoving = true; this.addEventListener('touchmove', onTouchMove, false) } } if ('ontouchstart' in document.documentElement) { this.addEventListener('touchstart', onTouchStart, false) } }); return this } })(jQuery);
//jquery touch wipe plugin ends
//jquery flex slider
/*
* jQuery FlexSlider v1.8
* http://www.woothemes.com/flexslider/
*
* Copyright 2012 WooThemes
* Free to use under the MIT license.
* http://www.opensource.org/licenses/mit-license.php
*
* Contrib: Darin Richardson
*/
(function (a) { a.flexslider = function (c, b) { var d = c; d.init = function () { d.vars = a.extend({}, a.flexslider.defaults, b); d.data("flexslider", true); d.container = a(".slides", d).first(); d.slides = a(".slides:first > li", d); d.count = d.slides.length; d.animating = false; d.currentSlide = d.vars.slideToStart; d.animatingTo = d.currentSlide; d.atEnd = (d.currentSlide == 0) ? true : false; d.eventType = ("ontouchstart" in document.documentElement) ? "touchstart" : "click"; d.cloneCount = 0; d.cloneOffset = 0; d.manualPause = false; d.vertical = (d.vars.slideDirection == "vertical"); d.prop = (d.vertical) ? "top" : "marginLeft"; d.args = {}; d.transitions = "webkitTransition" in document.body.style; if (d.transitions) { d.prop = "-webkit-transform" } if (d.vars.controlsContainer != "") { d.controlsContainer = a(d.vars.controlsContainer).eq(a(".slides").index(d.container)); d.containerExists = d.controlsContainer.length > 0 } if (d.vars.manualControls != "") { d.manualControls = a(d.vars.manualControls, ((d.containerExists) ? d.controlsContainer : d)); d.manualExists = d.manualControls.length > 0 } if (d.vars.randomize) { d.slides.sort(function () { return (Math.round(Math.random()) - 0.5) }); d.container.empty().append(d.slides) } if (d.vars.animation.toLowerCase() == "slide") { if (d.transitions) { d.setTransition(0) } d.css({ overflow: "hidden" }); if (d.vars.animationLoop) { d.cloneCount = 2; d.cloneOffset = 1; d.container.append(d.slides.filter(":first").clone().addClass("clone")).prepend(d.slides.filter(":last").clone().addClass("clone")) } d.newSlides = a(".slides:first > li", d); var m = (-1 * (d.currentSlide + d.cloneOffset)); if (d.vertical) { d.newSlides.css({ display: "block", width: "100%", "float": "left" }); d.container.height((d.count + d.cloneCount) * 200 + "%").css("position", "absolute").width("100%"); setTimeout(function () { d.css({ position: "relative" }).height(d.slides.filter(":first").height()); d.args[d.prop] = (d.transitions) ? "translate3d(0," + m * d.height() + "px,0)" : m * d.height() + "px"; d.container.css(d.args) }, 100) } else { d.args[d.prop] = (d.transitions) ? "translate3d(" + m * d.width() + "px,0,0)" : m * d.width() + "px"; d.container.width((d.count + d.cloneCount) * 200 + "%").css(d.args); setTimeout(function () { d.newSlides.width(d.width()).css({ "float": "left", display: "block" }) }, 100) } } else { d.transitions = false; d.slides.css({ width: "100%", "float": "left", marginRight: "-100%" }).eq(d.currentSlide).fadeIn(d.vars.animationDuration) } if (d.vars.controlNav) { if (d.manualExists) { d.controlNav = d.manualControls } else { var e = a('<ol class="flex-control-nav"></ol>'); var s = 1; for (var t = 0; t < d.count; t++) { e.append("<li><a>" + s + "</a></li>"); s++ } if (d.containerExists) { a(d.controlsContainer).append(e); d.controlNav = a(".flex-control-nav li a", d.controlsContainer) } else { d.append(e); d.controlNav = a(".flex-control-nav li a", d) } } d.controlNav.eq(d.currentSlide).addClass("active"); d.controlNav.bind(d.eventType, function (i) { i.preventDefault(); if (!a(this).hasClass("active")) { (d.controlNav.index(a(this)) > d.currentSlide) ? d.direction = "next" : d.direction = "prev"; d.flexAnimate(d.controlNav.index(a(this)), d.vars.pauseOnAction) } }) } if (d.vars.directionNav) { var v = a('<ul class="flex-direction-nav"><li><a class="prev" href="#">' + d.vars.prevText + '</a></li><li><a class="next" href="#">' + d.vars.nextText + "</a></li></ul>"); if (d.containerExists) { a(d.controlsContainer).append(v); d.directionNav = a(".flex-direction-nav li a", d.controlsContainer) } else { d.append(v); d.directionNav = a(".flex-direction-nav li a", d) } if (!d.vars.animationLoop) { if (d.currentSlide == 0) { d.directionNav.filter(".prev").addClass("disabled") } else { if (d.currentSlide == d.count - 1) { d.directionNav.filter(".next").addClass("disabled") } } } d.directionNav.bind(d.eventType, function (i) { i.preventDefault(); var j = (a(this).hasClass("next")) ? d.getTarget("next") : d.getTarget("prev"); if (d.canAdvance(j)) { d.flexAnimate(j, d.vars.pauseOnAction) } }) } if (d.vars.keyboardNav && a("ul.slides").length == 1) { function h(i) { if (d.animating) { return } else { if (i.keyCode != 39 && i.keyCode != 37) { return } else { if (i.keyCode == 39) { var j = d.getTarget("next") } else { if (i.keyCode == 37) { var j = d.getTarget("prev") } } if (d.canAdvance(j)) { d.flexAnimate(j, d.vars.pauseOnAction) } } } } a(document).bind("keyup", h) } if (d.vars.mousewheel) { d.mousewheelEvent = (/Firefox/i.test(navigator.userAgent)) ? "DOMMouseScroll" : "mousewheel"; d.bind(d.mousewheelEvent, function (y) { y.preventDefault(); y = y ? y : window.event; var i = y.detail ? y.detail * -1 : y.wheelDelta / 40, j = (i < 0) ? d.getTarget("next") : d.getTarget("prev"); if (d.canAdvance(j)) { d.flexAnimate(j, d.vars.pauseOnAction) } }) } if (d.vars.slideshow) { if (d.vars.pauseOnHover && d.vars.slideshow) { d.hover(function () { d.pause() }, function () { if (!d.manualPause) { d.resume() } }) } d.animatedSlides = setInterval(d.animateSlides, d.vars.slideshowSpeed) } if (d.vars.pausePlay) { var q = a('<div class="flex-pauseplay"><span></span></div>'); if (d.containerExists) { d.controlsContainer.append(q); d.pausePlay = a(".flex-pauseplay span", d.controlsContainer) } else { d.append(q); d.pausePlay = a(".flex-pauseplay span", d) } var n = (d.vars.slideshow) ? "pause" : "play"; d.pausePlay.addClass(n).text((n == "pause") ? d.vars.pauseText : d.vars.playText); d.pausePlay.bind(d.eventType, function (i) { i.preventDefault(); if (a(this).hasClass("pause")) { d.pause(); d.manualPause = true } else { d.resume(); d.manualPause = false } }) } if ("ontouchstart" in document.documentElement) { var w, u, l, r, o, x, p = false; d.each(function () { if ("ontouchstart" in document.documentElement) { this.addEventListener("touchstart", g, false) } }); function g(i) { if (d.animating) { i.preventDefault() } else { if (i.touches.length == 1) { d.pause(); r = (d.vertical) ? d.height() : d.width(); x = Number(new Date()); l = (d.vertical) ? (d.currentSlide + d.cloneOffset) * d.height() : (d.currentSlide + d.cloneOffset) * d.width(); w = (d.vertical) ? i.touches[0].pageY : i.touches[0].pageX; u = (d.vertical) ? i.touches[0].pageX : i.touches[0].pageY; d.setTransition(0); this.addEventListener("touchmove", k, false); this.addEventListener("touchend", f, false) } } } function k(i) { o = (d.vertical) ? w - i.touches[0].pageY : w - i.touches[0].pageX; p = (d.vertical) ? (Math.abs(o) < Math.abs(i.touches[0].pageX - u)) : (Math.abs(o) < Math.abs(i.touches[0].pageY - u)); if (!p) { i.preventDefault(); if (d.vars.animation == "slide" && d.transitions) { if (!d.vars.animationLoop) { o = o / ((d.currentSlide == 0 && o < 0 || d.currentSlide == d.count - 1 && o > 0) ? (Math.abs(o) / r + 2) : 1) } d.args[d.prop] = (d.vertical) ? "translate3d(0," + (-l - o) + "px,0)" : "translate3d(" + (-l - o) + "px,0,0)"; d.container.css(d.args) } } } function f(j) { d.animating = false; if (d.animatingTo == d.currentSlide && !p && !(o == null)) { var i = (o > 0) ? d.getTarget("next") : d.getTarget("prev"); if (d.canAdvance(i) && Number(new Date()) - x < 550 && Math.abs(o) > 20 || Math.abs(o) > r / 2) { d.flexAnimate(i, d.vars.pauseOnAction) } else { d.flexAnimate(d.currentSlide, d.vars.pauseOnAction) } } this.removeEventListener("touchmove", k, false); this.removeEventListener("touchend", f, false); w = null; u = null; o = null; l = null } } if (d.vars.animation.toLowerCase() == "slide") { a(window).resize(function () { if (!d.animating) { if (d.vertical) { d.height(d.slides.filter(":first").height()); d.args[d.prop] = (-1 * (d.currentSlide + d.cloneOffset)) * d.slides.filter(":first").height() + "px"; if (d.transitions) { d.setTransition(0); d.args[d.prop] = (d.vertical) ? "translate3d(0," + d.args[d.prop] + ",0)" : "translate3d(" + d.args[d.prop] + ",0,0)" } d.container.css(d.args) } else { d.newSlides.width(d.width()); d.args[d.prop] = (-1 * (d.currentSlide + d.cloneOffset)) * d.width() + "px"; if (d.transitions) { d.setTransition(0); d.args[d.prop] = (d.vertical) ? "translate3d(0," + d.args[d.prop] + ",0)" : "translate3d(" + d.args[d.prop] + ",0,0)" } d.container.css(d.args) } } }) } d.vars.start(d) }; d.flexAnimate = function (g, f) { if (!d.animating) { d.animating = true; d.animatingTo = g; d.vars.before(d); if (f) { d.pause() } if (d.vars.controlNav) { d.controlNav.removeClass("active").eq(g).addClass("active") } d.atEnd = (g == 0 || g == d.count - 1) ? true : false; if (!d.vars.animationLoop && d.vars.directionNav) { if (g == 0) { d.directionNav.removeClass("disabled").filter(".prev").addClass("disabled") } else { if (g == d.count - 1) { d.directionNav.removeClass("disabled").filter(".next").addClass("disabled") } else { d.directionNav.removeClass("disabled") } } } if (!d.vars.animationLoop && g == d.count - 1) { d.pause(); d.vars.end(d) } if (d.vars.animation.toLowerCase() == "slide") { var e = (d.vertical) ? d.slides.filter(":first").height() : d.slides.filter(":first").width(); if (d.currentSlide == 0 && g == d.count - 1 && d.vars.animationLoop && d.direction != "next") { d.slideString = "0px" } else { if (d.currentSlide == d.count - 1 && g == 0 && d.vars.animationLoop && d.direction != "prev") { d.slideString = (-1 * (d.count + 1)) * e + "px" } else { d.slideString = (-1 * (g + d.cloneOffset)) * e + "px" } } d.args[d.prop] = d.slideString; if (d.transitions) { d.setTransition(d.vars.animationDuration); d.args[d.prop] = (d.vertical) ? "translate3d(0," + d.slideString + ",0)" : "translate3d(" + d.slideString + ",0,0)"; d.container.css(d.args).one("webkitTransitionEnd transitionend", function () { d.wrapup(e) }) } else { d.container.animate(d.args, d.vars.animationDuration, function () { d.wrapup(e) }) } } else { d.slides.eq(d.currentSlide).fadeOut(d.vars.animationDuration); d.slides.eq(g).fadeIn(d.vars.animationDuration, function () { d.wrapup() }) } } }; d.wrapup = function (e) { if (d.vars.animation == "slide") { if (d.currentSlide == 0 && d.animatingTo == d.count - 1 && d.vars.animationLoop) { d.args[d.prop] = (-1 * d.count) * e + "px"; if (d.transitions) { d.setTransition(0); d.args[d.prop] = (d.vertical) ? "translate3d(0," + d.args[d.prop] + ",0)" : "translate3d(" + d.args[d.prop] + ",0,0)" } d.container.css(d.args) } else { if (d.currentSlide == d.count - 1 && d.animatingTo == 0 && d.vars.animationLoop) { d.args[d.prop] = -1 * e + "px"; if (d.transitions) { d.setTransition(0); d.args[d.prop] = (d.vertical) ? "translate3d(0," + d.args[d.prop] + ",0)" : "translate3d(" + d.args[d.prop] + ",0,0)" } d.container.css(d.args) } } } d.animating = false; d.currentSlide = d.animatingTo; d.vars.after(d) }; d.animateSlides = function () { if (!d.animating) { d.flexAnimate(d.getTarget("next")) } }; d.pause = function () { clearInterval(d.animatedSlides); if (d.vars.pausePlay) { d.pausePlay.removeClass("pause").addClass("play").text(d.vars.playText) } }; d.resume = function () { d.animatedSlides = setInterval(d.animateSlides, d.vars.slideshowSpeed); if (d.vars.pausePlay) { d.pausePlay.removeClass("play").addClass("pause").text(d.vars.pauseText) } }; d.canAdvance = function (e) { if (!d.vars.animationLoop && d.atEnd) { if (d.currentSlide == 0 && e == d.count - 1 && d.direction != "next") { return false } else { if (d.currentSlide == d.count - 1 && e == 0 && d.direction == "next") { return false } else { return true } } } else { return true } }; d.getTarget = function (e) { d.direction = e; if (e == "next") { return (d.currentSlide == d.count - 1) ? 0 : d.currentSlide + 1 } else { return (d.currentSlide == 0) ? d.count - 1 : d.currentSlide - 1 } }; d.setTransition = function (e) { d.container.css({ "-webkit-transition-duration": (e / 1000) + "s" }) }; d.init() }; a.flexslider.defaults = { animation: "fade", slideDirection: "horizontal", slideshow: true, slideshowSpeed: 7000, animationDuration: 600, directionNav: true, controlNav: true, keyboardNav: true, mousewheel: false, prevText: "Previous", nextText: "Next", pausePlay: false, pauseText: "Pause", playText: "Play", randomize: false, slideToStart: 0, animationLoop: true, pauseOnAction: true, pauseOnHover: false, controlsContainer: "", manualControls: "", start: function () { }, before: function () { }, after: function () { }, end: function () { } }; a.fn.flexslider = function (b) { return this.each(function () { if (a(this).find(".slides li").length == 1) { a(this).find(".slides li").fadeIn(400) } else { if (a(this).data("flexslider") != true) { new a.flexslider(a(this), b) } } }) } })(jQuery);
//jquery flex slider ends here

//jquery quick sand plugin here
/*

Quicksand 1.2.2

Reorder and filter items with a nice shuffling animation.

Copyright (c) 2010 Jacek Galanciak (razorjack.net) and agilope.com
Big thanks for Piotr Petrus (riddle.pl) for deep code review and wonderful docs & demos.

Dual licensed under the MIT and GPL version 2 licenses.
http://github.com/jquery/jquery/blob/master/MIT-LICENSE.txt
http://github.com/jquery/jquery/blob/master/GPL-LICENSE.txt

Project site: http://razorjack.net/quicksand
Github site: http://github.com/razorjack/quicksand

*/

(function ($) {
    $.fn.quicksand = function (collection, customOptions) {     
        var options = {
            duration: 750,
            easing: 'swing',
            attribute: 'data-id', // attribute to recognize same items within source and dest
            adjustHeight: 'auto', // 'dynamic' animates height during shuffling (slow), 'auto' adjusts it before or after the animation, false leaves height constant
            useScaling: true, // disable it if you're not using scaling effect or want to improve performance
            enhancement: function(c) {}, // Visual enhacement (eg. font replacement) function for cloned elements
            selector: '> *',
            dx: 0,
            dy: 0
        };
        $.extend(options, customOptions);
        
        if ($.browser.msie || (typeof($.fn.scale) == 'undefined')) {
            // Got IE and want scaling effect? Kiss my ass.
            options.useScaling = false;
        }
        
        var callbackFunction;
        if (typeof(arguments[1]) == 'function') {
            var callbackFunction = arguments[1];
        } else if (typeof(arguments[2] == 'function')) {
            var callbackFunction = arguments[2];
        }
    
        
        return this.each(function (i) {
            var val;
            var animationQueue = []; // used to store all the animation params before starting the animation; solves initial animation slowdowns
            var $collection = $(collection).clone(); // destination (target) collection
            var $sourceParent = $(this); // source, the visible container of source collection
            var sourceHeight = $(this).css('height'); // used to keep height and document flow during the animation
            
            var destHeight;
            var adjustHeightOnCallback = false;
            
            var offset = $($sourceParent).offset(); // offset of visible container, used in animation calculations
            var offsets = []; // coordinates of every source collection item            
            
            var $source = $(this).find(options.selector); // source collection items
            
            // Replace the collection and quit if IE6
            if ($.browser.msie && $.browser.version.substr(0,1)<7) {
                $sourceParent.html('').append($collection);
                return;
            }

            // Gets called when any animation is finished
            var postCallbackPerformed = 0; // prevents the function from being called more than one time
            var postCallback = function () {
                
                if (!postCallbackPerformed) {
                    postCallbackPerformed = 1;
                    
                    // hack: 
                    // used to be: $sourceParent.html($dest.html()); // put target HTML into visible source container
                    // but new webkit builds cause flickering when replacing the collections
                    $toDelete = $sourceParent.find('> *');
                    $sourceParent.prepend($dest.find('> *'));
                    $toDelete.remove();
                         
                    if (adjustHeightOnCallback) {
                        $sourceParent.css('height', destHeight);
                    }
                    options.enhancement($sourceParent); // Perform custom visual enhancements on a newly replaced collection
                    if (typeof callbackFunction == 'function') {
                        callbackFunction.call(this);
                    }                    
                }
            };
            
            // Position: relative situations
            var $correctionParent = $sourceParent.offsetParent();
            var correctionOffset = $correctionParent.offset();
            if ($correctionParent.css('position') == 'relative') {
                if ($correctionParent.get(0).nodeName.toLowerCase() == 'body') {

                } else {
                    correctionOffset.top += (parseFloat($correctionParent.css('border-top-width')) || 0);
                    correctionOffset.left +=( parseFloat($correctionParent.css('border-left-width')) || 0);
                }
            } else {
                correctionOffset.top -= (parseFloat($correctionParent.css('border-top-width')) || 0);
                correctionOffset.left -= (parseFloat($correctionParent.css('border-left-width')) || 0);
                correctionOffset.top -= (parseFloat($correctionParent.css('margin-top')) || 0);
                correctionOffset.left -= (parseFloat($correctionParent.css('margin-left')) || 0);
            }
            
            // perform custom corrections from options (use when Quicksand fails to detect proper correction)
            if (isNaN(correctionOffset.left)) {
                correctionOffset.left = 0;
            }
            if (isNaN(correctionOffset.top)) {
                correctionOffset.top = 0;
            }
            
            correctionOffset.left -= options.dx;
            correctionOffset.top -= options.dy;

            // keeps nodes after source container, holding their position
            $sourceParent.css('height', $(this).height());
            
            // get positions of source collections
            $source.each(function (i) {
                offsets[i] = $(this).offset();
            });
            
            // stops previous animations on source container
            $(this).stop();
            var dx = 0; var dy = 0;
            $source.each(function (i) {
                $(this).stop(); // stop animation of collection items
                var rawObj = $(this).get(0);
                if (rawObj.style.position == 'absolute') {
                    dx = -options.dx;
                    dy = -options.dy;
                } else {
                    dx = options.dx;
                    dy = options.dy;                    
                }

                rawObj.style.position = 'absolute';
                rawObj.style.margin = '0';

                rawObj.style.top = (offsets[i].top - parseFloat(rawObj.style.marginTop) - correctionOffset.top + dy) + 'px';
                rawObj.style.left = (offsets[i].left - parseFloat(rawObj.style.marginLeft) - correctionOffset.left + dx) + 'px';
            });
                    
            // create temporary container with destination collection
            var $dest = $($sourceParent).clone();
            var rawDest = $dest.get(0);
            rawDest.innerHTML = '';
            rawDest.setAttribute('id', '');
            rawDest.style.height = 'auto';
            rawDest.style.width = $sourceParent.width() + 'px';
            $dest.append($collection);      
            // insert node into HTML
            // Note that the node is under visible source container in the exactly same position
            // The browser render all the items without showing them (opacity: 0.0)
            // No offset calculations are needed, the browser just extracts position from underlayered destination items
            // and sets animation to destination positions.
            $dest.insertBefore($sourceParent);
            $dest.css('opacity', 0.0);
            rawDest.style.zIndex = -1;
            
            rawDest.style.margin = '0';
            rawDest.style.position = 'absolute';
            rawDest.style.top = offset.top - correctionOffset.top + 'px';
            rawDest.style.left = offset.left - correctionOffset.left + 'px';
            
            
    
            

            if (options.adjustHeight === 'dynamic') {
                // If destination container has different height than source container
                // the height can be animated, adjusting it to destination height
                $sourceParent.animate({height: $dest.height()}, options.duration, options.easing);
            } else if (options.adjustHeight === 'auto') {
                destHeight = $dest.height();
                if (parseFloat(sourceHeight) < parseFloat(destHeight)) {
                    // Adjust the height now so that the items don't move out of the container
                    $sourceParent.css('height', destHeight);
                } else {
                    //  Adjust later, on callback
                    adjustHeightOnCallback = true;
                }
            }
                
            // Now it's time to do shuffling animation
            // First of all, we need to identify same elements within source and destination collections    
            $source.each(function (i) {
                var destElement = [];
                if (typeof(options.attribute) == 'function') {
                    
                    val = options.attribute($(this));
                    $collection.each(function() {
                        if (options.attribute(this) == val) {
                            destElement = $(this);
                            return false;
                        }
                    });
                } else {
                    destElement = $collection.filter('[' + options.attribute + '=' + $(this).attr(options.attribute) + ']');
                }
                if (destElement.length) {
                    // The item is both in source and destination collections
                    // It it's under different position, let's move it
                    if (!options.useScaling) {
                        animationQueue.push(
                                            {
                                                element: $(this), 
                                                animation: 
                                                    {top: destElement.offset().top - correctionOffset.top, 
                                                     left: destElement.offset().left - correctionOffset.left, 
                                                     opacity: 1.0
                                                    }
                                            });

                    } else {
                        animationQueue.push({
                                            element: $(this), 
                                            animation: {top: destElement.offset().top - correctionOffset.top, 
                                                        left: destElement.offset().left - correctionOffset.left, 
                                                        opacity: 1.0, 
                                                        scale: '1.0'
                                                       }
                                            });

                    }
                } else {
                    // The item from source collection is not present in destination collections
                    // Let's remove it
                    if (!options.useScaling) {
                        animationQueue.push({element: $(this), 
                                             animation: {opacity: '0.0'}});
                    } else {
                        animationQueue.push({element: $(this), animation: {opacity: '0.0', 
                                         scale: '0.0'}});
                    }
                }
            });
            
            $collection.each(function (i) {
                // Grab all items from target collection not present in visible source collection
                
                var sourceElement = [];
                var destElement = [];
                if (typeof(options.attribute) == 'function') {
                    val = options.attribute($(this));
                    $source.each(function() {
                        if (options.attribute(this) == val) {
                            sourceElement = $(this);
                            return false;
                        }
                    });                 

                    $collection.each(function() {
                        if (options.attribute(this) == val) {
                            destElement = $(this);
                            return false;
                        }
                    });
                } else {
                    sourceElement = $source.filter('[' + options.attribute + '=' + $(this).attr(options.attribute) + ']');
                    destElement = $collection.filter('[' + options.attribute + '=' + $(this).attr(options.attribute) + ']');
                }
                
                var animationOptions;
                if (sourceElement.length === 0) {
                    // No such element in source collection...
                    if (!options.useScaling) {
                        animationOptions = {
                            opacity: '1.0'
                        };
                    } else {
                        animationOptions = {
                            opacity: '1.0',
                            scale: '1.0'
                        };
                    }
                    // Let's create it
                    d = destElement.clone();
                    var rawDestElement = d.get(0);
                    rawDestElement.style.position = 'absolute';
                    rawDestElement.style.margin = '0';
                    rawDestElement.style.top = destElement.offset().top - correctionOffset.top + 'px';
                    rawDestElement.style.left = destElement.offset().left - correctionOffset.left + 'px';
                    d.css('opacity', 0.0); // IE
                    if (options.useScaling) {
                        d.css('transform', 'scale(0.0)');
                    }
                    d.appendTo($sourceParent);
                    
                    animationQueue.push({element: $(d), 
                                         animation: animationOptions});
                }
            });
            
            $dest.remove();
            options.enhancement($sourceParent); // Perform custom visual enhancements during the animation
            for (i = 0; i < animationQueue.length; i++) {
                animationQueue[i].element.animate(animationQueue[i].animation, options.duration, options.easing, postCallback);
            }
        });
    };
})(jQuery);
//jquery quick sand plugin ends here

//jquery carofredsel plugin
/*	
 *	jQuery carouFredSel 5.5.5
 *	Demo's and documentation:
 *	caroufredsel.frebsite.nl
 *	
 *	Copyright (c) 2012 Fred Heusschen
 *	www.frebsite.nl
 *
 *	Dual licensed under the MIT and GPL licenses.
 *	http://en.wikipedia.org/wiki/MIT_License
 *	http://en.wikipedia.org/wiki/GNU_General_Public_License
 */


(function($) {



	//	LOCAL

	if ($.fn.carouFredSel) return;

	$.fn.carouFredSel = function(options, configs) {
		if (this.length == 0) {
			debug(true, 'No element found for "'+this.selector+'".');
			return this;
		}
		if (this.length > 1) {
			return this.each(function() {
				$(this).carouFredSel(options, configs);
			});
		}

		var $cfs = this,
			$tt0 = this[0];

		if ($cfs.data('cfs_isCarousel')) {
			var starting_position = $cfs.triggerHandler('_cfs_currentPosition');
			$cfs.trigger('_cfs_destroy', true);
		} else {
			var starting_position = false;
		}

		$cfs._cfs_init = function(o, setOrig, start) {
			o = go_getObject($tt0, o);


			//	DEPRECATED
			if (o.debug) {
				conf.debug = o.debug;
				debug(conf, 'The "debug" option should be moved to the second configuration-object.');
			}
			//	/DEPRECATED


			var obs = ['items', 'scroll', 'auto', 'prev', 'next', 'pagination'];
			for (var a = 0, l = obs.length; a < l; a++) {
				o[obs[a]] = go_getObject($tt0, o[obs[a]]);
			}

			if (typeof o.scroll == 'number') {
				if (o.scroll <= 50)					o.scroll	= { 'items'		: o.scroll 	};
				else								o.scroll	= { 'duration'	: o.scroll 	};
			} else {
				if (typeof o.scroll == 'string')	o.scroll	= { 'easing'	: o.scroll 	};
			}

				 if (typeof o.items == 'number')	o.items		= { 'visible'	: o.items 	};
			else if (		o.items == 'variable')	o.items		= { 'visible'	: o.items,
																	'width'		: o.items, 
																	'height'	: o.items	};

			if (typeof o.items != 'object') o.items = {};
			if (setOrig) opts_orig = $.extend(true, {}, $.fn.carouFredSel.defaults, o);

			opts = $.extend(true, {}, $.fn.carouFredSel.defaults, o);

			if (typeof opts.items.visibleConf != 'object') opts.items.visibleConf = {};

			if (opts.items.start == 0 && typeof start == 'number') {
				opts.items.start = start;
			}

			crsl.upDateOnWindowResize = (opts.responsive);
			crsl.direction = (opts.direction == 'up' || opts.direction == 'left') ? 'next' : 'prev';

			var dims = [
				['width'	, 'innerWidth'	, 'outerWidth'	, 'height'	, 'innerHeight'	, 'outerHeight'	, 'left', 'top'	, 'marginRight'	, 0, 1, 2, 3],
				['height'	, 'innerHeight'	, 'outerHeight'	, 'width'	, 'innerWidth'	, 'outerWidth'	, 'top'	, 'left', 'marginBottom', 3, 2, 1, 0]
			];

			var dn = dims[0].length,
				dx = (opts.direction == 'right' || opts.direction == 'left') ? 0 : 1;

			opts.d = {};
			for (var d = 0; d < dn; d++) {
				opts.d[dims[0][d]] = dims[dx][d];
			}

			var	all_itm = $cfs.children();


			//	check visible items
			switch (typeof opts.items.visible) {

				//	min and max visible items
				case 'object':
					opts.items.visibleConf.min = opts.items.visible.min;
					opts.items.visibleConf.max = opts.items.visible.max;
					opts.items.visible = false;
					break;
				
				case 'string':
					//	variable visible items
					if (opts.items.visible == 'variable') {
						opts.items.visibleConf.variable = true;

					//	adjust string visible items
					} else {
						opts.items.visibleConf.adjust = opts.items.visible;
					}
					opts.items.visible = false;
					break;

				// function visible items
				case 'function':
					opts.items.visibleConf.adjust = opts.items.visible;
					opts.items.visible = false;
					break;
			}

			//	set items filter
			if (typeof opts.items.filter == 'undefined') {
				opts.items.filter = (all_itm.filter(':hidden').length > 0) ? ':visible' : '*';
			}

			//	primary size set to auto -> measure largest size and set it
			if (opts[opts.d['width']] == 'auto') {
				opts[opts.d['width']] = ms_getTrueLargestSize(all_itm, opts, 'outerWidth');
			}
			//	primary size percentage
			if (ms_isPercentage(opts[opts.d['width']]) && !opts.responsive) {
				opts[opts.d['width']] = ms_getPercentage(ms_getTrueInnerSize($wrp.parent(), opts, 'innerWidth'), opts[opts.d['width']]);
				crsl.upDateOnWindowResize = true;
			}

			//	secondary size set to auto -> measure largest size and set it
			if (opts[opts.d['height']] == 'auto') {
				opts[opts.d['height']] = ms_getTrueLargestSize(all_itm, opts, 'outerHeight');
			}

			//	primary item-size not set
			if (!opts.items[opts.d['width']]) {
//	responsive carousel -> set to largest
if (opts.responsive) {
	debug(true, 'Set a '+opts.d['width']+' for the items!');
	opts.items[opts.d['width']] = ms_getTrueLargestSize(all_itm, opts, 'outerWidth');
				//	 non-responsive -> measure it or set to "variable"
} else {
				opts.items[opts.d['width']] = (ms_hasVariableSizes(all_itm, opts, 'outerWidth')) 
					? 'variable' 
					: all_itm[opts.d['outerWidth']](true);
}
			}

			//	secondary item-size not set -> measure it or set to "variable"
			if (!opts.items[opts.d['height']]) {
				opts.items[opts.d['height']] = (ms_hasVariableSizes(all_itm, opts, 'outerHeight')) 
					? 'variable' 
					: all_itm[opts.d['outerHeight']](true);
			}

			//	secondary size not set -> set to secondary item-size
			if (!opts[opts.d['height']]) {
				opts[opts.d['height']] = opts.items[opts.d['height']];
			}

			//	visible-items not set
			if (!opts.items.visible && !opts.responsive) {
				//	primary item-size variable -> set visible items variable
				if (opts.items[opts.d['width']] == 'variable') {
					opts.items.visibleConf.variable = true;
				}
				if (!opts.items.visibleConf.variable) {
					//	primary size is number -> calculate visible-items
					if (typeof opts[opts.d['width']] == 'number') {
						opts.items.visible = Math.floor(opts[opts.d['width']] / opts.items[opts.d['width']]);
					} else {
						//	measure and calculate primary size and visible-items
						var maxS = ms_getTrueInnerSize($wrp.parent(), opts, 'innerWidth');
						opts.items.visible = Math.floor(maxS / opts.items[opts.d['width']]);
						opts[opts.d['width']] = opts.items.visible * opts.items[opts.d['width']];
						if (!opts.items.visibleConf.adjust) opts.align = false;
					}
					if (opts.items.visible == 'Infinity' || opts.items.visible < 1) {
						debug(true, 'Not a valid number of visible items: Set to "variable".');
						opts.items.visibleConf.variable = true;
					}
				}
			}

			//	primary size not set -> calculate it or set to "variable"
			if (!opts[opts.d['width']]) {
				opts[opts.d['width']] = 'variable';
				if (!opts.responsive && opts.items.filter == '*' && !opts.items.visibleConf.variable && opts.items[opts.d['width']] != 'variable') {
					opts[opts.d['width']] = opts.items.visible * opts.items[opts.d['width']];
					opts.align = false;
				}
			}

			//	variable primary item-sizes with variabe visible-items
			if (opts.items.visibleConf.variable) {
				opts.maxDimention = (opts[opts.d['width']] == 'variable')
					? ms_getTrueInnerSize($wrp.parent(), opts, 'innerWidth')
					: opts[opts.d['width']];
				if (opts.align === false) {
					opts[opts.d['width']] = 'variable';
				}
				opts.items.visible = gn_getVisibleItemsNext(all_itm, opts, 0);

			//	set visible items by filter
			} else if (opts.items.filter != '*') {
				opts.items.visibleConf.org = opts.items.visible;
				opts.items.visible = gn_getVisibleItemsNextFilter(all_itm, opts, 0);
			}

			//	align not set -> set to center if primary size is number
			if (typeof opts.align == 'undefined') {
				opts.align = (opts[opts.d['width']] == 'variable')
					? false
					: 'center';
			}

			opts.items.visible = cf_getItemsAdjust(opts.items.visible, opts, opts.items.visibleConf.adjust, $tt0);
			opts.items.visibleConf.old = opts.items.visible;
			opts.usePadding = false;


if (opts.responsive) {

	if (!opts.items.visibleConf.min) opts.items.visibleConf.min = opts.items.visible;
	if (!opts.items.visibleConf.max) opts.items.visibleConf.max = opts.items.visible;

	opts.align = false;
	opts.padding = [0, 0, 0, 0];

	var isVisible = $wrp.is(':visible');
	if (isVisible) $wrp.hide();
	var fullS = ms_getPercentage(ms_getTrueInnerSize($wrp.parent(), opts, 'innerWidth'), opts[opts.d['width']]);

	if (typeof opts[opts.d['width']] == 'number' && fullS < opts[opts.d['width']]) {
		fullS = opts[opts.d['width']];
	}
	if (isVisible) $wrp.show();

	var visb = cf_getItemAdjustMinMax(Math.ceil(fullS / opts.items[opts.d['width']]), opts.items.visibleConf);
	if (visb > all_itm.length) {
		visb = all_itm.length;
	}

	var newS = Math.floor(fullS/visb),
		seco = opts[opts.d['height']],
		secp = ms_isPercentage(seco);

	all_itm.each(function() {
		var $t = $(this),
			nw = newS - ms_getPaddingBorderMargin($t, opts, 'Width');

		$t[opts.d['width']](nw);
		if (secp) {
			$t[opts.d['height']](ms_getPercentage(nw, seco));
		}
	});

	opts.items.visible = visb;
	opts.items[opts.d['width']] = newS;
	opts[opts.d['width']] = visb * newS;
	
} else {

			opts.padding = cf_getPadding(opts.padding);

			if (opts.align == 'top') 		opts.align = 'left';
			if (opts.align == 'bottom') 	opts.align = 'right';


			switch (opts.align) {
				//	align: center, left or right
				case 'center':
				case 'left':
				case 'right':
					if (opts[opts.d['width']] != 'variable') {
						var p = cf_getAlignPadding(gi_getCurrentItems(all_itm, opts), opts);
						opts.usePadding = true;
						opts.padding[opts.d[1]] = p[1];
						opts.padding[opts.d[3]] = p[0];
					}
					break;

				//	padding
				default:
					opts.align = false;
					opts.usePadding = (
						opts.padding[0] == 0 && 
						opts.padding[1] == 0 && 
						opts.padding[2] == 0 && 
						opts.padding[3] == 0
					) ? false : true;
					break;
			}
}

			if (typeof opts.cookie == 'boolean' && opts.cookie)			opts.cookie 					= 'caroufredsel_cookie_'+$cfs.attr('id');
			if (typeof opts.items.minimum				!= 'number')	opts.items.minimum				= opts.items.visible;
			if (typeof opts.scroll.duration				!= 'number')	opts.scroll.duration			= 500;
			if (typeof opts.scroll.items				== 'undefined') opts.scroll.items 				= (opts.items.visibleConf.variable || opts.items.filter != '*') ? 'visible' : opts.items.visible;

			opts.auto		= go_getNaviObject($tt0, opts.auto, 'auto');
			opts.prev		= go_getNaviObject($tt0, opts.prev);
			opts.next		= go_getNaviObject($tt0, opts.next);
			opts.pagination	= go_getNaviObject($tt0, opts.pagination, 'pagination');

			opts.auto		= $.extend(true, {}, opts.scroll, opts.auto);
			opts.prev		= $.extend(true, {}, opts.scroll, opts.prev);
			opts.next		= $.extend(true, {}, opts.scroll, opts.next);
			opts.pagination	= $.extend(true, {}, opts.scroll, opts.pagination);

			if (typeof opts.pagination.keys				!= 'boolean')	opts.pagination.keys 			= false;
			if (typeof opts.pagination.anchorBuilder	!= 'function'
					&& opts.pagination.anchorBuilder	!== false)		opts.pagination.anchorBuilder	= $.fn.carouFredSel.pageAnchorBuilder;
			if (typeof opts.auto.play					!= 'boolean')	opts.auto.play					= true;
			if (typeof opts.auto.delay					!= 'number')	opts.auto.delay					= 0;
			if (typeof opts.auto.pauseOnEvent 			== 'undefined')	opts.auto.pauseOnEvent			= true;
			if (typeof opts.auto.pauseOnResize 			!= 'boolean')	opts.auto.pauseOnResize			= true;
			if (typeof opts.auto.pauseDuration			!= 'number')	opts.auto.pauseDuration			= (opts.auto.duration < 10) ? 2500 : opts.auto.duration * 5;

			if (opts.synchronise) {
				opts.synchronise = cf_getSynchArr(opts.synchronise);
			}
			if (conf.debug) {
				debug(conf, 'Carousel width: '+opts.width);
				debug(conf, 'Carousel height: '+opts.height);
				if (opts.maxDimention)	debug(conf, 'Available '+opts.d['width']+': '+opts.maxDimention);
				debug(conf, 'Item widths: '+opts.items.width);
				debug(conf, 'Item heights: '+opts.items.height);
				debug(conf, 'Number of items visible: '+opts.items.visible);
				if (opts.auto.play)		debug(conf, 'Number of items scrolled automatically: '+opts.auto.items);
				if (opts.prev.button)	debug(conf, 'Number of items scrolled backward: '+opts.prev.items);
				if (opts.next.button)	debug(conf, 'Number of items scrolled forward: '+opts.next.items);
			}
		};	//	/init

		$cfs._cfs_build = function() {
			$cfs.data('cfs_isCarousel', true);

			var orgCSS = {
				'textAlign'		: $cfs.css('textAlign'),
				'float'			: $cfs.css('float'),
				'position'		: $cfs.css('position'),
				'top'			: $cfs.css('top'),
				'right'			: $cfs.css('right'),
				'bottom'		: $cfs.css('bottom'),
				'left'			: $cfs.css('left'),
				'width'			: $cfs.css('width'),
				'height'		: $cfs.css('height'),
				'marginTop'		: $cfs.css('marginTop'),
				'marginRight'	: $cfs.css('marginRight'),
				'marginBottom'	: $cfs.css('marginBottom'),
				'marginLeft'	: $cfs.css('marginLeft')
			};

			switch (orgCSS.position) {
				case 'absolute':
					var newPosition = 'absolute';
					break;
				case 'fixed':
					var newPosition = 'fixed';
					break;
				default:
					var newPosition = 'relative';
			} 

			$wrp.css(orgCSS).css({
				'overflow'		: 'hidden',
				'position'		: newPosition
			});

			$cfs.data('cfs_origCss', orgCSS).css({
				'textAlign'		: 'left',
				'float'			: 'none',
				'position'		: 'absolute',
				'top'			: 0,
				'left'			: 0,
				'marginTop'		: 0,
				'marginRight'	: 0,
				'marginBottom'	: 0,
				'marginLeft'	: 0
			});

			if (opts.usePadding) {
				$cfs.children().each(function() {
					var m = parseInt($(this).css(opts.d['marginRight']));
					if (isNaN(m)) m = 0;
					$(this).data('cfs_origCssMargin', m);
				});
			}

		};	//	/build

		$cfs._cfs_bind_events = function() {
			$cfs._cfs_unbind_events();

			//	stop event
			$cfs.bind(cf_e('stop', conf), function(e, imm) {
				e.stopPropagation();

				//	button
				if (!crsl.isStopped) {
					if (opts.auto.button) {
						opts.auto.button.addClass(cf_c('stopped', conf));
					}
				}

				//	set stopped
				crsl.isStopped = true;

				if (opts.auto.play) {
					opts.auto.play = false;
					$cfs.trigger(cf_e('pause', conf), imm);
				}
				return true;
			});

			//	finish event
			$cfs.bind(cf_e('finish', conf), function(e) {
				e.stopPropagation();
				if (crsl.isScrolling) {
					sc_stopScroll(scrl);
				}
				return true;
			});

			//	pause event
			$cfs.bind(cf_e('pause', conf), function(e, imm, res) {
				e.stopPropagation();
				tmrs = sc_clearTimers(tmrs);

				//	immediately pause
				if (imm && crsl.isScrolling) {
					scrl.isStopped = true;
					var nst = getTime() - scrl.startTime;
					scrl.duration -= nst;
					if (scrl.pre) scrl.pre.duration -= nst;
					if (scrl.post) scrl.post.duration -= nst;
					sc_stopScroll(scrl, false);
				}

				//	update remaining pause-time
				if (!crsl.isPaused && !crsl.isScrolling) {
					if (res) tmrs.timePassed += getTime() - tmrs.startTime;
				}
				
				//	button
				if (!crsl.isPaused) {
					if (opts.auto.button) {
						opts.auto.button.addClass(cf_c('paused', conf));
					}
				}

				//	set paused
				crsl.isPaused = true;

				//	pause pause callback
				if (opts.auto.onPausePause) {
					var dur1 = opts.auto.pauseDuration - tmrs.timePassed,
						perc = 100 - Math.ceil( dur1 * 100 / opts.auto.pauseDuration );
					opts.auto.onPausePause.call($tt0, perc, dur1);
				}
				return true;
			});

			//	play event
			$cfs.bind(cf_e('play', conf), function(e, dir, del, res) {
				e.stopPropagation();
				tmrs = sc_clearTimers(tmrs);

				//	sort params
				var v = [dir, del, res],
					t = ['string', 'number', 'boolean'],
					a = cf_sortParams(v, t);

				var dir = a[0],
					del = a[1],
					res = a[2];

				if (dir != 'prev' && dir != 'next') dir = crsl.direction;
				if (typeof del != 'number') 		del = 0;
				if (typeof res != 'boolean') 		res = false;

				//	stopped?
				if (res) {
					crsl.isStopped = false;
					opts.auto.play = true;
				}
				if (!opts.auto.play) {
					e.stopImmediatePropagation();
					return debug(conf, 'Carousel stopped: Not scrolling.');
				}

				//	button
				if (crsl.isPaused) {
					if (opts.auto.button) {
						opts.auto.button.removeClass(cf_c('stopped', conf));
						opts.auto.button.removeClass(cf_c('paused', conf));
					}
				}

				//	set playing
				crsl.isPaused = false;
				tmrs.startTime = getTime();

				//	timeout the scrolling
				var dur1 = opts.auto.pauseDuration + del;
					dur2 = dur1 - tmrs.timePassed;
					perc = 100 - Math.ceil(dur2 * 100 / dur1);

				tmrs.auto = setTimeout(function() {
					if (opts.auto.onPauseEnd) {
						opts.auto.onPauseEnd.call($tt0, perc, dur2);
					}
					if (crsl.isScrolling) {
						$cfs.trigger(cf_e('play', conf), dir);
					} else {
						$cfs.trigger(cf_e(dir, conf), opts.auto);
					}
				}, dur2);

				//	pause start callback
				if (opts.auto.onPauseStart) {
					opts.auto.onPauseStart.call($tt0, perc, dur2);
				}

				return true;
			});

			//	resume event
			$cfs.bind(cf_e('resume', conf), function(e) {
				e.stopPropagation();
				if (scrl.isStopped) {
					scrl.isStopped = false;
					crsl.isPaused = false;
					crsl.isScrolling = true;
					scrl.startTime = getTime();
					sc_startScroll(scrl);
				} else {
					$cfs.trigger(cf_e('play', conf));
				}
				return true;
			});

			//	prev + next events
			$cfs.bind(cf_e('prev', conf)+' '+cf_e('next', conf), function(e, obj, num, clb) {
				e.stopPropagation();

				//	stopped or hidden carousel, don't scroll, don't queue
				if (crsl.isStopped || $cfs.is(':hidden')) {
					e.stopImmediatePropagation();
					return debug(conf, 'Carousel stopped or hidden: Not scrolling.');
				}

				//	not enough items
				if (opts.items.minimum >= itms.total) {
					e.stopImmediatePropagation();
					return debug(conf, 'Not enough items ('+itms.total+', '+opts.items.minimum+' needed): Not scrolling.');
				}

				//	get config
				var v = [obj, num, clb],
					t = ['object', 'number/string', 'function'],
					a = cf_sortParams(v, t);

				var obj = a[0],
					num = a[1],
					clb = a[2];

				var eType = e.type.slice(conf.events.prefix.length);

				if (typeof obj != 'object' || obj == null)	obj = opts[eType];
				if (typeof clb == 'function')				obj.onAfter = clb;

				if (typeof num != 'number') {
					if (opts.items.filter != '*') {
						num = 'visible';
					} else {
						var arr = [num, obj.items, opts[eType].items];
						for (var a = 0, l = arr.length; a < l; a++) {
							if (typeof arr[a] == 'number' || arr[a] == 'page' || arr[a] == 'visible') {
								num = arr[a];
								break;
							}
						}
					}
					switch(num) {
						case 'page':
							e.stopImmediatePropagation();
							return $cfs.triggerHandler(eType+'Page', [obj, clb]);
							break;

						case 'visible':
							if (!opts.items.visibleConf.variable && opts.items.filter == '*') {
								num = opts.items.visible;
							}
							break;
					}
				}

				//	resume animation, add current to queue
				if (scrl.isStopped) {
					$cfs.trigger(cf_e('resume', conf));
					$cfs.trigger(cf_e('queue', conf), [eType, [obj, num, clb]]);
					e.stopImmediatePropagation();
					return debug(conf, 'Carousel resumed scrolling.');
				}

				//	queue if scrolling
				if (obj.duration > 0) {
					if (crsl.isScrolling) {
						if (obj.queue) $cfs.trigger(cf_e('queue', conf), [eType, [obj, num, clb]]);
						e.stopImmediatePropagation();
						return debug(conf, 'Carousel currently scrolling.');
					}
				}

				//	test conditions callback
				if (obj.conditions && !obj.conditions.call($tt0)) {
					e.stopImmediatePropagation();
					return debug(conf, 'Callback "conditions" returned false.');
				}

				tmrs.timePassed = 0;
				$cfs.trigger('_cfs_slide_'+eType, [obj, num]);

				//	synchronise
				if (opts.synchronise) {
					var s = opts.synchronise,
						c = [obj, num];
					for (var j = 0, l = s.length; j < l; j++) {
						var d = eType;
						if (!s[j][1]) c[0] = s[j][0].triggerHandler('_cfs_configuration', eType);
						if (!s[j][2]) d = (d == 'prev') ? 'next' : 'prev';
						c[1] = num + s[j][3];
						s[j][0].trigger('_cfs_slide_'+d, c);
					}
				}
				return true;
			});

			//	prev event, accessible from outside
			$cfs.bind(cf_e('_cfs_slide_prev', conf, false), function(e, sO, nI) {
				e.stopPropagation();
				var a_itm = $cfs.children();

				//	non-circular at start, scroll to end
				if (!opts.circular) {
					if (itms.first == 0) {
						if (opts.infinite) {
							$cfs.trigger(cf_e('next', conf), itms.total-1);
						}
						return e.stopImmediatePropagation();
					}
				}

				if (opts.usePadding) sz_resetMargin(a_itm, opts);

				//	find number of items to scroll
				if (typeof nI != 'number') {
					if (opts.items.visibleConf.variable) {
						nI = gn_getVisibleItemsPrev(a_itm, opts, itms.total-1);
					} else if (opts.items.filter != '*') {
						var xI = (typeof sO.items == 'number') ? sO.items : gn_getVisibleOrg($cfs, opts);
						nI = gn_getScrollItemsPrevFilter(a_itm, opts, itms.total-1, xI);
					} else {
						nI = opts.items.visible;
					}
					nI = cf_getAdjust(nI, opts, sO.items, $tt0);
				}

				//	prevent non-circular from scrolling to far
				if (!opts.circular) {
					if (itms.total - nI < itms.first) {
						nI = itms.total - itms.first;
					}
				}

				//	set new number of visible items
				opts.items.visibleConf.old = opts.items.visible;
				if (opts.items.visibleConf.variable) {
					var vI = gn_getVisibleItemsNext(a_itm, opts, itms.total-nI);
					if (opts.items.visible+nI <= vI && nI < itms.total) {
						nI++;
						vI = gn_getVisibleItemsNext(a_itm, opts, itms.total-nI);
					}
					opts.items.visible = cf_getItemsAdjust(vI, opts, opts.items.visibleConf.adjust, $tt0);
				} else if (opts.items.filter != '*') {
					var vI = gn_getVisibleItemsNextFilter(a_itm, opts, itms.total-nI);
					opts.items.visible = cf_getItemsAdjust(vI, opts, opts.items.visibleConf.adjust, $tt0);
				}

				if (opts.usePadding) sz_resetMargin(a_itm, opts, true);

				//	scroll 0, don't scroll
				if (nI == 0) {
					e.stopImmediatePropagation();
					return debug(conf, '0 items to scroll: Not scrolling.');
				}
				debug(conf, 'Scrolling '+nI+' items backward.');

				//	save new config
				itms.first += nI;
				while (itms.first >= itms.total) {
					itms.first -= itms.total;
				}

				//	non-circular callback
				if (!opts.circular) {
					if (itms.first == 0 && sO.onEnd) sO.onEnd.call($tt0);
					if (!opts.infinite) nv_enableNavi(opts, itms.first, conf);
				}

				//	rearrange items
				$cfs.children().slice(itms.total-nI, itms.total).prependTo($cfs);
				if (itms.total < opts.items.visible + nI) {
					$cfs.children().slice(0, (opts.items.visible+nI)-itms.total).clone(true).appendTo($cfs);
				}

				//	the needed items
				var a_itm = $cfs.children(),
					c_old = gi_getOldItemsPrev(a_itm, opts, nI),
					c_new = gi_getNewItemsPrev(a_itm, opts),
					l_cur = a_itm.eq(nI-1),
					l_old = c_old.last(),
					l_new = c_new.last();

				if (opts.usePadding) sz_resetMargin(a_itm, opts);
				if (opts.align) {
					var p = cf_getAlignPadding(c_new, opts),
						pL = p[0],
						pR = p[1];
				} else {
					var pL = 0,
						pR = 0;
				}
				var oL = (pL < 0) ? opts.padding[opts.d[3]] : 0;

				//	hide items for fx directscroll
				if (sO.fx == 'directscroll' && opts.items.visible < nI) {
					var hiddenitems = a_itm.slice(opts.items.visibleConf.old, nI),
						orgW = opts.items[opts.d['width']];
					hiddenitems.each(function() {
						var hi = $(this);
						hi.data('isHidden', hi.is(':hidden')).hide();
					});
					opts.items[opts.d['width']] = 'variable';
				} else {
					var hiddenitems = false;
				}

				//	save new sizes
				var i_siz = ms_getTotalSize(a_itm.slice(0, nI), opts, 'width'),
					w_siz = cf_mapWrapperSizes(ms_getSizes(c_new, opts, true), opts, !opts.usePadding);

				if (hiddenitems) opts.items[opts.d['width']] = orgW;

				if (opts.usePadding) {
					sz_resetMargin(a_itm, opts, true);
					if (pR >= 0) {
						sz_resetMargin(l_old, opts, opts.padding[opts.d[1]]);
					}
					sz_resetMargin(l_cur, opts, opts.padding[opts.d[3]]);
				}
				if (opts.align) {
					opts.padding[opts.d[1]] = pR;
					opts.padding[opts.d[3]] = pL;
				}

				//	animation configuration
				var a_cfs = {},
					a_dur = sO.duration;

					 if (sO.fx == 'none')	a_dur = 0;
				else if (a_dur == 'auto')	a_dur = opts.scroll.duration / opts.scroll.items * nI;
				else if (a_dur <= 0)		a_dur = 0;
				else if (a_dur < 10)		a_dur = i_siz / a_dur;

				scrl = sc_setScroll(a_dur, sO.easing);

				//	animate wrapper
				if (opts[opts.d['width']] == 'variable' || opts[opts.d['height']] == 'variable') {
					scrl.anims.push([$wrp, w_siz]);
				}

				//	animate items
				if (opts.usePadding) {
					var new_m = opts.padding[opts.d[3]];

					if (l_new.not(l_cur).length) {
			 			var a_cur = {};
			 				a_cur[opts.d['marginRight']] = l_cur.data('cfs_origCssMargin');

						if (pL < 0) l_cur.css(a_cur);
						else 		scrl.anims.push([l_cur, a_cur]);
					}

					if (l_new.not(l_old).length) {
						var a_old = {};
							a_old[opts.d['marginRight']] = l_old.data('cfs_origCssMargin');
						scrl.anims.push([l_old, a_old]);
					}

					if (pR >= 0) {
						var a_new = {};
							a_new[opts.d['marginRight']] = l_new.data('cfs_origCssMargin') + opts.padding[opts.d[1]];
						scrl.anims.push([l_new, a_new]);
					}
				} else {
					var new_m = 0;
				}

				//	animate carousel
				a_cfs[opts.d['left']] = new_m;

				//	onBefore callback
				var args = [c_old, c_new, w_siz, a_dur];
				if (sO.onBefore) sO.onBefore.apply($tt0, args);
				clbk.onBefore = sc_callCallbacks(clbk.onBefore, $tt0, args);



				//	ALTERNATIVE EFFECTS

				//	extra animation arrays
				switch(sO.fx) {
					case 'fade':
					case 'crossfade':
					case 'cover':
					case 'uncover':
						scrl.pre = sc_setScroll(scrl.duration, scrl.easing);
						scrl.post = sc_setScroll(scrl.duration, scrl.easing);
						scrl.duration = 0;
						break;
				}

				//	create copy
				switch(sO.fx) {
					case 'crossfade':
					case 'cover':
					case 'uncover':
						var $cf2 = $cfs.clone().appendTo($wrp);
						break;
				}
				switch(sO.fx) {
					case 'uncover':
						$cf2.children().slice(0, nI).remove();
					case 'crossfade':
					case 'cover':
						$cf2.children().slice(opts.items.visible).remove();
						break;
				}

				//	animations
				switch(sO.fx) {
					case 'fade':
						scrl.pre.anims.push([$cfs, { 'opacity': 0 }]);
						break;
					case 'crossfade':
						$cf2.css({ 'opacity': 0 });
						scrl.pre.anims.push([$cfs, { 'width': '+=0' }, function() { $cf2.remove(); }]);
						scrl.post.anims.push([$cf2, { 'opacity': 1 }]);
						break;
					case 'cover':
						scrl = fx_cover(scrl, $cfs, $cf2, opts, true);
						break;
					case 'uncover':
						scrl = fx_uncover(scrl, $cfs, $cf2, opts, true, nI);
						break;
				}

				//	/ALTERNATIVE EFFECTS


				//	complete callback
				var a_complete = function() {

					var overFill = opts.items.visible+nI-itms.total;
					if (overFill > 0) {
						$cfs.children().slice(itms.total).remove();
						c_old = $( $cfs.children().slice(itms.total-(opts.items.visible-overFill)).get().concat( $cfs.children().slice(0, overFill).get() ) );
					}
					if (hiddenitems) {
						hiddenitems.each(function() {
							var hi = $(this);
							if (!hi.data('isHidden')) hi.show();
						});	
					}
					if (opts.usePadding) {
						var l_itm = $cfs.children().eq(opts.items.visible+nI-1);
						l_itm.css(opts.d['marginRight'], l_itm.data('cfs_origCssMargin'));
					}

					scrl.anims = [];
					if (scrl.pre) scrl.pre = sc_setScroll(scrl.orgDuration, scrl.easing);

					var fn = function() {
						switch(sO.fx) {
							case 'fade':
							case 'crossfade':
								$cfs.css('filter', '');
								break;
						}

						scrl.post = sc_setScroll(0, null);
						crsl.isScrolling = false;

						var args = [c_old, c_new, w_siz];
						if (sO.onAfter) sO.onAfter.apply($tt0, args);
						clbk.onAfter = sc_callCallbacks(clbk.onAfter, $tt0, args);

						if (queu.length) {
							$cfs.trigger(cf_e(queu[0][0], conf), queu[0][1]);
							queu.shift();
						}
						if (!crsl.isPaused) $cfs.trigger(cf_e('play', conf));
					};
					switch(sO.fx) {
						case 'fade':
							scrl.pre.anims.push([$cfs, { 'opacity': 1 }, fn]);
							sc_startScroll(scrl.pre);
							break;
						case 'uncover':
							scrl.pre.anims.push([$cfs, { 'width': '+=0' }, fn]);
							sc_startScroll(scrl.pre);
							break;
						default:
							fn();
							break;
					}
				};

				scrl.anims.push([$cfs, a_cfs, a_complete]);
				crsl.isScrolling = true;
				$cfs.css(opts.d['left'], -(i_siz-oL));
				tmrs = sc_clearTimers(tmrs);
				sc_startScroll(scrl);
				cf_setCookie(opts.cookie, $cfs.triggerHandler(cf_e('currentPosition', conf)));

				$cfs.trigger(cf_e('updatePageStatus', conf), [false, w_siz]);

				return true;
			});

			//	next event, accessible from outside
			$cfs.bind(cf_e('_cfs_slide_next', conf, false), function(e, sO, nI) {
				e.stopPropagation();
				var a_itm = $cfs.children();

				//	non-circular at end, scroll to start
				if (!opts.circular) {
					if (itms.first == opts.items.visible) {
						if (opts.infinite) {
							$cfs.trigger(cf_e('prev', conf), itms.total-1);
						}
						return e.stopImmediatePropagation();
					}
				}

				if (opts.usePadding) sz_resetMargin(a_itm, opts);

				//	find number of items to scroll
				if (typeof nI != 'number') {
					if (opts.items.filter != '*') {
						var xI = (typeof sO.items == 'number') ? sO.items : gn_getVisibleOrg($cfs, opts);
						nI = gn_getScrollItemsNextFilter(a_itm, opts, 0, xI);
					} else {
						nI = opts.items.visible;
					}
					nI = cf_getAdjust(nI, opts, sO.items, $tt0);
				}

				var lastItemNr = (itms.first == 0) ? itms.total : itms.first;

				//	prevent non-circular from scrolling to far
				if (!opts.circular) {
					if (opts.items.visibleConf.variable) {
						var vI = gn_getVisibleItemsNext(a_itm, opts, nI),
							xI = gn_getVisibleItemsPrev(a_itm, opts, lastItemNr-1);
					} else {
						var vI = opts.items.visible,
							xI = opts.items.visible;
					}

					if (nI + vI > lastItemNr) {
						nI = lastItemNr - xI;
					}
				}

				//	set new number of visible items
				opts.items.visibleConf.old = opts.items.visible;
				if (opts.items.visibleConf.variable) {
					var vI = gn_getVisibleItemsNextTestCircular(a_itm, opts, nI, lastItemNr);
					while (opts.items.visible-nI >= vI && nI < itms.total) {
						nI++;
						vI = gn_getVisibleItemsNextTestCircular(a_itm, opts, nI, lastItemNr);
					}
					opts.items.visible = cf_getItemsAdjust(vI, opts, opts.items.visibleConf.adjust, $tt0);
				} else if (opts.items.filter != '*') {
					var vI = gn_getVisibleItemsNextFilter(a_itm, opts, nI);
					opts.items.visible = cf_getItemsAdjust(vI, opts, opts.items.visibleConf.adjust, $tt0);
				}

				if (opts.usePadding) sz_resetMargin(a_itm, opts, true);

				//	scroll 0, don't scroll
				if (nI == 0) {
					e.stopImmediatePropagation();
					return debug(conf, '0 items to scroll: Not scrolling.');
				}
				debug(conf, 'Scrolling '+nI+' items forward.');

				//	save new config
				itms.first -= nI;
				while (itms.first < 0) {
					itms.first += itms.total;
				}

				//	non-circular callback
				if (!opts.circular) {
					if (itms.first == opts.items.visible && sO.onEnd) sO.onEnd.call($tt0);
					if (!opts.infinite) nv_enableNavi(opts, itms.first, conf);
				}

				//	rearrange items
				if (itms.total < opts.items.visible+nI) {
					$cfs.children().slice(0, (opts.items.visible+nI)-itms.total).clone(true).appendTo($cfs);
				}

				//	the needed items
				var a_itm = $cfs.children(),
					c_old = gi_getOldItemsNext(a_itm, opts),
					c_new = gi_getNewItemsNext(a_itm, opts, nI),
					l_cur = a_itm.eq(nI-1),
					l_old = c_old.last(),
					l_new = c_new.last();

				if (opts.usePadding) sz_resetMargin(a_itm, opts);
				if (opts.align)	{
					var p = cf_getAlignPadding(c_new, opts),
						pL = p[0],
						pR = p[1];
				} else {
					var pL = 0,
						pR = 0;
				}

				//	hide items for fx directscroll
				if (sO.fx == 'directscroll' && opts.items.visibleConf.old < nI) {
					var hiddenitems = a_itm.slice(opts.items.visibleConf.old, nI),
						orgW = opts.items[opts.d['width']];
					hiddenitems.each(function() {
						var hi = $(this);
						hi.data('isHidden', hi.is(':hidden')).hide();
					});
					opts.items[opts.d['width']] = 'variable';
				} else {
					var hiddenitems = false;
				}

				//	save new sizes
				var i_siz = ms_getTotalSize(a_itm.slice(0, nI), opts, 'width'),
					w_siz = cf_mapWrapperSizes(ms_getSizes(c_new, opts, true), opts, !opts.usePadding);

				if (hiddenitems) opts.items[opts.d['width']] = orgW;

				if (opts.align) {
					if (opts.padding[opts.d[1]] < 0) {
						opts.padding[opts.d[1]] = 0;
					}
				}
				if (opts.usePadding) {
					sz_resetMargin(a_itm, opts, true);
					sz_resetMargin(l_old, opts, opts.padding[opts.d[1]]);
				}
				if (opts.align) {
					opts.padding[opts.d[1]] = pR;
					opts.padding[opts.d[3]] = pL;
				}

				//	animation configuration
				var a_cfs = {},
					a_dur = sO.duration;

					 if (sO.fx == 'none')	a_dur = 0;
				else if (a_dur == 'auto')	a_dur = opts.scroll.duration / opts.scroll.items * nI;
				else if (a_dur <= 0)		a_dur = 0;
				else if (a_dur < 10)		a_dur = i_siz / a_dur;

				scrl = sc_setScroll(a_dur, sO.easing);

				//	animate wrapper
				if (opts[opts.d['width']] == 'variable' || opts[opts.d['height']] == 'variable') {
					scrl.anims.push([$wrp, w_siz]);
				}

				//	animate items
				if (opts.usePadding) {
					var l_new_m = l_new.data('cfs_origCssMargin');
					if (pR >= 0) {
						l_new_m += opts.padding[opts.d[1]];
					}
					l_new.css(opts.d['marginRight'], l_new_m);

					if (l_cur.not(l_old).length) {
						var a_old = {};
							a_old[opts.d['marginRight']] = l_old.data('cfs_origCssMargin');
						scrl.anims.push([l_old, a_old]);
					}

					var c_new_m = l_cur.data('cfs_origCssMargin');
					if (pL >= 0) {
						c_new_m += opts.padding[opts.d[3]];
					}
					var a_cur = {};
						a_cur[opts.d['marginRight']] = c_new_m;
					scrl.anims.push([l_cur, a_cur]);

				}

				//	animate carousel
				a_cfs[opts.d['left']] = -i_siz;
				if (pL < 0) {
					a_cfs[opts.d['left']] += pL;
				}

				//	onBefore callback
				var args = [c_old, c_new, w_siz, a_dur];
				if (sO.onBefore) sO.onBefore.apply($tt0, args);
				clbk.onBefore = sc_callCallbacks(clbk.onBefore, $tt0, args);



				//	ALTERNATIVE EFFECTS

				//	extra animation arrays
				switch(sO.fx) {
					case 'fade':
					case 'crossfade':
					case 'cover':
					case 'uncover':
						scrl.pre = sc_setScroll(scrl.duration, scrl.easing);
						scrl.post = sc_setScroll(scrl.duration, scrl.easing);
						scrl.duration = 0;
						break;
				}

				//	create copy
				switch(sO.fx) {
					case 'crossfade':
					case 'cover':
					case 'uncover':
						var $cf2 = $cfs.clone().appendTo($wrp);
						break;
				}
				switch(sO.fx) {
					case 'uncover':
						$cf2.children().slice(opts.items.visibleConf.old).remove();
						break;
					case 'crossfade':
					case 'cover':
						$cf2.children().slice(0, nI).remove();
						$cf2.children().slice(opts.items.visible).remove();
						break;
				}

				//	animations
				switch(sO.fx) {
					case 'fade':
						scrl.pre.anims.push([$cfs, { 'opacity': 0 }]);
						break;
					case 'crossfade':
						$cf2.css({ 'opacity': 0 });
						scrl.pre.anims.push([$cfs, { 'width': '+=0' }, function() { $cf2.remove(); }]);
						scrl.post.anims.push([$cf2, { 'opacity': 1 }]);
						break;
					case 'cover':
						scrl = fx_cover(scrl, $cfs, $cf2, opts, false);
						break;
					case 'uncover':
						scrl = fx_uncover(scrl, $cfs, $cf2, opts, false, nI);
						break;
				}

				//	/ALTERNATIVE EFFECTS


				//	complete callback
				var a_complete = function() {

					var overFill = opts.items.visible+nI-itms.total,
						new_m = (opts.usePadding) ? opts.padding[opts.d[3]] : 0;
					$cfs.css(opts.d['left'], new_m);
					if (overFill > 0) {
						$cfs.children().slice(itms.total).remove();
					}
					var l_itm = $cfs.children().slice(0, nI).appendTo($cfs).last();
					if (overFill > 0) {
						c_new = gi_getCurrentItems(a_itm, opts);
					}
					if (hiddenitems) {
						hiddenitems.each(function() {
							var hi = $(this);
							if (!hi.data('isHidden')) hi.show();
						});
					}
					if (opts.usePadding) {
						if (itms.total < opts.items.visible+nI) {
							var l_cur = $cfs.children().eq(opts.items.visible-1);
							l_cur.css(opts.d['marginRight'], l_cur.data('cfs_origCssMargin') + opts.padding[opts.d[3]]);
						}
						l_itm.css(opts.d['marginRight'], l_itm.data('cfs_origCssMargin'));
					}

					scrl.anims = [];
					if (scrl.pre) scrl.pre = sc_setScroll(scrl.orgDuration, scrl.easing);

					var fn = function() {
						switch(sO.fx) {
							case 'fade':
							case 'crossfade':
								$cfs.css('filter', '');
								break;
						}

						scrl.post = sc_setScroll(0, null);
						crsl.isScrolling = false;

						var args = [c_old, c_new, w_siz];
						if (sO.onAfter) sO.onAfter.apply($tt0, args);
						clbk.onAfter = sc_callCallbacks(clbk.onAfter, $tt0, args);

						if (queu.length) {
							$cfs.trigger(cf_e(queu[0][0], conf), queu[0][1]);
							queu.shift();
						}
						if (!crsl.isPaused) $cfs.trigger(cf_e('play', conf));
					};
					switch(sO.fx) {
						case 'fade':
							scrl.pre.anims.push([$cfs, { 'opacity': 1 }, fn]);
							sc_startScroll(scrl.pre);
							break;
						case 'uncover':
							scrl.pre.anims.push([$cfs, { 'width': '+=0' }, fn]);
							sc_startScroll(scrl.pre);
							break;
						default:
							fn();
							break;
					}
				};

				scrl.anims.push([$cfs, a_cfs, a_complete]);
				crsl.isScrolling = true;
				tmrs = sc_clearTimers(tmrs);
				sc_startScroll(scrl);
				cf_setCookie(opts.cookie, $cfs.triggerHandler(cf_e('currentPosition', conf)));

				$cfs.trigger(cf_e('updatePageStatus', conf), [false, w_siz]);

				return true;
			});

			//	slideTo event
			$cfs.bind(cf_e('slideTo', conf), function(e, num, dev, org, obj, dir, clb) {
				e.stopPropagation();

				var v = [num, dev, org, obj, dir, clb],
					t = ['string/number/object', 'number', 'boolean', 'object', 'string', 'function'],
					a = cf_sortParams(v, t);
				
				var obj = a[3],
					dir = a[4],
					clb = a[5];

				num = gn_getItemIndex(a[0], a[1], a[2], itms, $cfs);

				if (num == 0) return;
				if (typeof obj != 'object') obj = false;

				if (crsl.isScrolling) {
					if (typeof obj != 'object' || obj.duration > 0) return false;
				}

				if (dir != 'prev' && dir != 'next') {
					if (opts.circular) {
						if (num <= itms.total / 2) 	dir = 'next';
						else 						dir = 'prev';
					} else {
						if (itms.first == 0 ||
							itms.first > num)		dir = 'next';
						else						dir = 'prev';
					}
				}

				if (dir == 'prev') num = itms.total-num;
				$cfs.trigger(cf_e(dir, conf), [obj, num, clb]);

				return true;
			});

			//	prevPage event
			$cfs.bind(cf_e('prevPage', conf), function(e, obj, clb) {
				e.stopPropagation();
				var cur = $cfs.triggerHandler(cf_e('currentPage', conf));
				return $cfs.triggerHandler(cf_e('slideToPage', conf), [cur-1, obj, 'prev', clb]);
			});

			//	nextPage event
			$cfs.bind(cf_e('nextPage', conf), function(e, obj, clb) {
				e.stopPropagation();
				var cur = $cfs.triggerHandler(cf_e('currentPage', conf));
				return $cfs.triggerHandler(cf_e('slideToPage', conf), [cur+1, obj, 'next', clb]);
			});

			//	slideToPage event
			$cfs.bind(cf_e('slideToPage', conf), function(e, pag, obj, dir, clb) {
				e.stopPropagation();
				if (typeof pag != 'number') pag = $cfs.triggerHandler(cf_e('currentPage', conf));
				var ipp = opts.pagination.items || opts.items.visible,
					max = Math.ceil(itms.total / ipp)-1;
				if (pag < 0)	pag = max;
				if (pag > max)	pag = 0;
				return $cfs.triggerHandler(cf_e('slideTo', conf), [pag*ipp, 0, true, obj, dir, clb]);
			});

			//	jumpToStart event
			$cfs.bind(cf_e('jumpToStart', conf), function(e, s) {
				e.stopPropagation();
				if (s)	s = gn_getItemIndex(s, 0, true, itms, $cfs);
				else 	s = 0;

				s += itms.first;
				if (s != 0) {
					while (s > itms.total) s -= itms.total;
					$cfs.prepend($cfs.children().slice(s, itms.total));
				}
				return true;
			});

			//	synchronise event
			$cfs.bind(cf_e('synchronise', conf), function(e, s) {
				e.stopPropagation();
					 if (s) 				s = cf_getSynchArr(s);
				else if (opts.synchronise)	s = opts.synchronise;
				else return debug(conf, 'No carousel to synchronise.');

				var n = $cfs.triggerHandler(cf_e('currentPosition', conf)),
					x = true;
				for (var j = 0, l = s.length; j < l; j++) {
					if (!s[j][0].triggerHandler(cf_e('slideTo', conf), [n, s[j][3], true])) {
						x = false;
					}
				}
				return x;
			});

			//	queue event
			$cfs.bind(cf_e('queue', conf), function(e, dir, opt) {
				e.stopPropagation();
				if (typeof dir == 'function') {
					dir.call($tt0, queu);
				} else if (is_array(dir)) {
					queu = dir;
				} else if (typeof dir != 'undefined') {
					queu.push([dir, opt]);
				}
				return queu;
			});

			//	insertItem event
			$cfs.bind(cf_e('insertItem', conf), function(e, itm, num, org, dev) {
				e.stopPropagation();

				var v = [itm, num, org, dev],
					t = ['string/object', 'string/number/object', 'boolean', 'number'],
					a = cf_sortParams(v, t);
				
				var itm = a[0],
					num = a[1],
					org = a[2],
					dev = a[3];

				if (typeof itm == 'object' && 
					typeof itm.jquery == 'undefined')	itm = $(itm);
				if (typeof itm == 'string') 			itm = $(itm);
				if (typeof itm != 'object' ||
					typeof itm.jquery == 'undefined' || 
					itm.length == 0) return debug(conf, 'Not a valid object.');

				if (typeof num == 'undefined') num = 'end';

				if (opts.usePadding) {
					itm.each(function() {
						var m = parseInt($(this).css(opts.d['marginRight']));
						if (isNaN(m)) m = 0;
						$(this).data('cfs_origCssMargin', m);
					});
				}

				var orgNum = num,
					before = 'before';

				if (num == 'end') {
					if (org) {
						if (itms.first == 0) {
							num = itms.total-1;
							before = 'after';
						} else {
							num = itms.first;
							itms.first += itm.length
						}
						if (num < 0) num = 0;
					} else {
						num = itms.total-1;
						before = 'after';
					}
				} else {
					num = gn_getItemIndex(num, dev, org, itms, $cfs);
				}
				if (orgNum != 'end' && !org) {
					if (num < itms.first) itms.first += itm.length;
				}
				if (itms.first >= itms.total) itms.first -= itms.total;

				var $cit = $cfs.children().eq(num);
				if ($cit.length) {
					$cit[before](itm);
				} else {
					$cfs.append(itm);
				}

				itms.total = $cfs.children().length;

				var sz = $cfs.triggerHandler('updateSizes');
				nv_showNavi(opts, itms.total, conf);
				nv_enableNavi(opts, itms.first, conf);
				$cfs.trigger(cf_e('linkAnchors', conf));
				$cfs.trigger(cf_e('updatePageStatus', conf), [true, sz]);

				return true;
			});

			//	removeItem event
			$cfs.bind(cf_e('removeItem', conf), function(e, num, org, dev) {
				e.stopPropagation();
				
				var v = [num, org, dev],
					t = ['string/number/object', 'boolean', 'number'],
					a = cf_sortParams(v, t);
				
				var num = a[0],
					org = a[1],
					dev = a[2];

				if (typeof num == 'undefined' || num == 'end') {
					$cfs.children().last().remove();
				} else {
					num = gn_getItemIndex(num, dev, org, itms, $cfs);
					var $cit = $cfs.children().eq(num);
					if ($cit.length){
						if (num < itms.first) itms.first -= $cit.length;
						$cit.remove();
					}
				}
				itms.total = $cfs.children().length;

				var sz = $cfs.triggerHandler('updateSizes');
				nv_showNavi(opts, itms.total, conf);
				nv_enableNavi(opts, itms.first, conf);
				$cfs.trigger(cf_e('updatePageStatus', conf), [true, sz]);

				return true;
			});

			//	onBefore and onAfter event
			$cfs.bind(cf_e('onBefore', conf)+' '+cf_e('onAfter', conf), function(e, fn) {
				e.stopPropagation();
				var eType = e.type.slice(conf.events.prefix.length);
				if (is_array(fn))				clbk[eType] = fn;
				if (typeof fn == 'function')	clbk[eType].push(fn);
				return clbk[eType];
			});

			//	currentPosition event, accessible from outside
			$cfs.bind(cf_e('_cfs_currentPosition', conf, false), function(e, fn) {
				e.stopPropagation();
				return $cfs.triggerHandler(cf_e('currentPosition', conf), fn);
			});
			$cfs.bind(cf_e('currentPosition', conf), function(e, fn) {
				e.stopPropagation();
				if (itms.first == 0) var val = 0;
				else var val = itms.total - itms.first;
				if (typeof fn == 'function') fn.call($tt0, val);
				return val;
			});

			//	currentPage event
			$cfs.bind(cf_e('currentPage', conf), function(e, fn) {
				e.stopPropagation();
				var ipp = opts.pagination.items || opts.items.visible;
				var max = Math.ceil(itms.total/ipp-1);
				if (itms.first == 0) 							var nr = 0;
				else if (itms.first < itms.total % ipp) 		var nr = 0;
				else if (itms.first == ipp && !opts.circular) 	var nr = max;
				else 											var nr = Math.round((itms.total-itms.first)/ipp);
				if (nr < 0) nr = 0;
				if (nr > max) nr = max;
				if (typeof fn == 'function') fn.call($tt0, nr);
				return nr;
			});

			//	currentVisible event
			$cfs.bind(cf_e('currentVisible', conf), function(e, fn) {
				e.stopPropagation();
				$i = gi_getCurrentItems($cfs.children(), opts);
				if (typeof fn == 'function') fn.call($tt0, $i);
				return $i;
			});
			
			//	slice event
			$cfs.bind(cf_e('slice', conf), function(e, f, l, fn) {
				e.stopPropagation();

				var v = [f, l, fn],
					t = ['number', 'number', 'function'],
					a = cf_sortParams(v, t);

				f = (typeof a[0] == 'number') ? a[0] : 0,
				l = (typeof a[1] == 'number') ? a[1] : itms.total,
				fn = a[2];
				
				f += itms.first;
				l += itms.first;

				while (f > itms.total) { f -= itms.total }
				while (l > itms.total) { l -= itms.total }
				while (f < 0) { f += itms.total }
				while (l < 0) { l += itms.total }

				var $iA = $cfs.children();

				if (l > f) {
					var $i = $iA.slice(f, l);	
				} else {
					var $i = $( $iA.slice(f, itms.total).get().concat( $iA.slice(0, l).get() ) );
				}

				if (typeof fn == 'function') fn.call($tt0, $i);
				return $i;
			});

			//	isPaused, isStopped and isScrolling events
			$cfs.bind(cf_e('isPaused', conf)+' '+cf_e('isStopped', conf)+' '+cf_e('isScrolling', conf), function(e, fn) {
				e.stopPropagation();
				var eType = e.type.slice(conf.events.prefix.length);
				if (typeof fn == 'function') fn.call($tt0, crsl[eType]);
				return crsl[eType];
			});

			//	configuration event, accessible from outside
			$cfs.bind(cf_e('_cfs_configuration', conf, false), function(e, a, b, c) {
				e.stopPropagation();
				return $cfs.triggerHandler(cf_e('configuration', conf), [a, b, c]);
			});
			$cfs.bind(cf_e('configuration', conf), function(e, a, b, c) {
				e.stopPropagation();
				var reInit = false;

				//	return entire configuration-object
				if (typeof a == 'function') {
					a.call($tt0, opts);

				//	set multiple options via object
				} else if (typeof a == 'object') {
					opts_orig = $.extend(true, {}, opts_orig, a);
					if (b !== false) reInit = true;
					else opts = $.extend(true, {}, opts, a);

				} else if (typeof a != 'undefined') {

					//	callback function for specific option
					if (typeof b == 'function') {
						var val = eval('opts.'+a);
						if (typeof val == 'undefined') val = '';
						b.call($tt0, val);

					//	set individual option
					} else if (typeof b != 'undefined') {
						if (typeof c !== 'boolean') c = true;
						eval('opts_orig.'+a+' = b');
						if (c !== false) reInit = true;
						else eval('opts.'+a+' = b');

					//	return value for specific option
					} else {
						return eval('opts.'+a);
					}
				}
				if (reInit) {
					sz_resetMargin($cfs.children(), opts);
					$cfs._cfs_init(opts_orig);
					$cfs._cfs_bind_buttons();
					var siz = sz_setSizes($cfs, opts, false);
					$cfs.trigger(cf_e('updatePageStatus', conf), [true, siz]);
				}
				return opts;
			});

			//	linkAnchors event
			$cfs.bind(cf_e('linkAnchors', conf), function(e, $con, sel) {
				e.stopPropagation();
				if (typeof $con == 'undefined' || $con.length == 0) $con = $('body');
				else if (typeof $con == 'string') $con = $($con);
				if (typeof $con != 'object') return debug(conf, 'Not a valid object.');
				if (typeof sel != 'string' || sel.length == 0) sel = 'a.caroufredsel';
				$con.find(sel).each(function() {
					var h = this.hash || '';
					if (h.length > 0 && $cfs.children().index($(h)) != -1) {
						$(this).unbind('click').click(function(e) {
							e.preventDefault();
							$cfs.trigger(cf_e('slideTo', conf), h);
						});
					}
				});
				return true;
			});

			//	updatePageStatus event
			$cfs.bind(cf_e('updatePageStatus', conf), function(e, build, sizes) {
				e.stopPropagation();
				if (!opts.pagination.container) return;
				
				if (build) {
					var ipp = opts.pagination.items || opts.items.visible,
						l = Math.ceil(itms.total/ipp);

					if (opts.pagination.anchorBuilder) {
						opts.pagination.container.children().remove();
						opts.pagination.container.each(function() {
							for (var a = 0; a < l; a++) {
								var i = $cfs.children().eq( gn_getItemIndex(a*ipp, 0, true, itms, $cfs) );
								$(this).append(opts.pagination.anchorBuilder(a+1, i));
							}
						});
					}
					opts.pagination.container.each(function() {
						$(this).children().unbind(opts.pagination.event).each(function(a) {
							$(this).bind(opts.pagination.event, function(e) {
								e.preventDefault();
								$cfs.trigger(cf_e('slideTo', conf), [a*ipp, 0, true, opts.pagination]);
							});
						});
					});
				}
				opts.pagination.container.each(function() {
					$(this).children().removeClass(cf_c('selected', conf)).eq($cfs.triggerHandler(cf_e('currentPage', conf))).addClass(cf_c('selected', conf));
				});
				return true;
			});

			//	updateSizes event
			$cfs.bind(cf_e('updateSizes', conf), function(e) {
				var a_itm = $cfs.children(),
					vI = opts.items.visible;

					 if (opts.items.visibleConf.variable)	vI = gn_getVisibleItemsNext(a_itm, opts, 0);
				else if (opts.items.filter != '*') 			vI = gn_getVisibleItemsNextFilter(a_itm, opts, 0);

				if (!opts.circular && itms.first != 0 && vI > itms.first) {
					if (opts.items.visibleConf.variable) {
						var nI = gn_getVisibleItemsPrev(a_itm, opts, itms.first) - itms.first;
					} else if (opts.items.filter != '*') {
						var nI = gn_getVisibleItemsPrevFilter(a_itm, opts, itms.first) - itms.first;
					} else {
						nI = opts.items.visible - itms.first;
					}
					debug(conf, 'Preventing non-circular: sliding '+nI+' items backward.');
					$cfs.trigger('prev', nI);
				}
				opts.items.visible = cf_getItemsAdjust(vI, opts, opts.items.visibleConf.adjust, $tt0);
				return sz_setSizes($cfs, opts);
			});

			//	destroy event, accessible from outside
			$cfs.bind(cf_e('_cfs_destroy', conf, false), function(e, orgOrder) {
				e.stopPropagation();
				$cfs.trigger(cf_e('destroy', conf), orgOrder);
				return true;
			});
			$cfs.bind(cf_e('destroy', conf), function(e, orgOrder) {
				e.stopPropagation();
				tmrs = sc_clearTimers(tmrs);

				$cfs.data('cfs_isCarousel', false);
				$cfs.trigger(cf_e('finish', conf));
				if (orgOrder) {
					$cfs.trigger(cf_e('jumpToStart', conf));
				}
				if (opts.usePadding) {
					sz_resetMargin($cfs.children(), opts);
				}

				$cfs.css($cfs.data('cfs_origCss'));
				$cfs._cfs_unbind_events();
				$cfs._cfs_unbind_buttons();
				$wrp.replaceWith($cfs);

				return true;
			});
		};	//	/bind_events

		$cfs._cfs_unbind_events = function() {
			$cfs.unbind(cf_e('', conf));
			$cfs.unbind(cf_e('', conf, false));
		};	//	/unbind_events

		$cfs._cfs_bind_buttons = function() {
			$cfs._cfs_unbind_buttons();
			nv_showNavi(opts, itms.total, conf);
			nv_enableNavi(opts, itms.first, conf);

			if (opts.auto.pauseOnHover) {
				var pC = bt_pauseOnHoverConfig(opts.auto.pauseOnHover);
				$wrp.bind(cf_e('mouseenter', conf, false), function() { $cfs.trigger(cf_e('pause', conf), pC);	})
					.bind(cf_e('mouseleave', conf, false), function() { $cfs.trigger(cf_e('resume', conf));		});
			}

			if (opts.auto.button) {
				opts.auto.button.bind(cf_e(opts.auto.event, conf, false), function(e) {
					e.preventDefault();
					var ev = false,
						pC = null;

					if (crsl.isPaused) {
						ev = 'play';
					} else if (opts.auto.pauseOnEvent) {
						ev = 'pause';
						pC = bt_pauseOnHoverConfig(opts.auto.pauseOnEvent);
					}
					if (ev) {
						$cfs.trigger(cf_e(ev, conf), pC);
					}
				});
			}
			if (opts.prev.button) {
				opts.prev.button.bind(cf_e(opts.prev.event, conf, false), function(e) {
					e.preventDefault();
					$cfs.trigger(cf_e('prev', conf));
				});
				if (opts.prev.pauseOnHover) {
					var pC = bt_pauseOnHoverConfig(opts.prev.pauseOnHover);
					opts.prev.button.bind(cf_e('mouseenter', conf, false), function() { $cfs.trigger(cf_e('pause', conf), pC);	})
									.bind(cf_e('mouseleave', conf, false), function() { $cfs.trigger(cf_e('resume', conf));		});
				}
			}

			if (opts.next.button) {
				opts.next.button.bind(cf_e(opts.next.event, conf, false), function(e) {
					e.preventDefault();
					$cfs.trigger(cf_e('next', conf));
				});
				if (opts.next.pauseOnHover) {
					var pC = bt_pauseOnHoverConfig(opts.next.pauseOnHover);
					opts.next.button.bind(cf_e('mouseenter', conf, false), function() { $cfs.trigger(cf_e('pause', conf), pC); 	})
									.bind(cf_e('mouseleave', conf, false), function() { $cfs.trigger(cf_e('resume', conf));		});
				}
			}
			if ($.fn.mousewheel) {
				if (opts.prev.mousewheel) {
					if (!crsl.mousewheelPrev) {
						crsl.mousewheelPrev = true;
						$wrp.mousewheel(function(e, delta) { 
							if (delta > 0) {
								e.preventDefault();
								var num = bt_mousesheelNumber(opts.prev.mousewheel);
								$cfs.trigger(cf_e('prev', conf), num);
							}
						});
					}
				}
				if (opts.next.mousewheel) {
					if (!crsl.mousewheelNext) {
						crsl.mousewheelNext = true;
						$wrp.mousewheel(function(e, delta) { 
							if (delta < 0) {
								e.preventDefault();
								var num = bt_mousesheelNumber(opts.next.mousewheel);
								$cfs.trigger(cf_e('next', conf), num);
							}
						});
					}
				}
			}
			if ($.fn.touchwipe) {
				var wP = (opts.prev.wipe) ? function() { $cfs.trigger(cf_e('prev', conf)) } : null,
					wN = (opts.next.wipe) ? function() { $cfs.trigger(cf_e('next', conf)) } : null;

				if (wN || wN) {
					if (!crsl.touchwipe) {
						crsl.touchwipe = true;
						var twOps = {
							'min_move_x': 30,
							'min_move_y': 30,
							'preventDefaultEvents': true
						};
						switch (opts.direction) {
							case 'up':
							case 'down':
								twOps.wipeUp = wP;
								twOps.wipeDown = wN;
								break;
							default:
								twOps.wipeLeft = wN;
								twOps.wipeRight = wP;
						}
						$wrp.touchwipe(twOps);
					}
				}
			}
			if (opts.pagination.container) {
				if (opts.pagination.pauseOnHover) {
					var pC = bt_pauseOnHoverConfig(opts.pagination.pauseOnHover);
					opts.pagination.container.bind(cf_e('mouseenter', conf, false), function() { $cfs.trigger(cf_e('pause', conf), pC);	})
											 .bind(cf_e('mouseleave', conf, false), function() { $cfs.trigger(cf_e('resume', conf));	});
				}
			}
			if (opts.prev.key || opts.next.key) {
				$(document).bind(cf_e('keyup', conf, false, true, true), function(e) {
					var k = e.keyCode;
					if (k == opts.next.key)	{
						e.preventDefault();
						$cfs.trigger(cf_e('next', conf));
					}
					if (k == opts.prev.key) {
						e.preventDefault();
						$cfs.trigger(cf_e('prev', conf));
					}
				});
			}
			if (opts.pagination.keys) {
				$(document).bind(cf_e('keyup', conf, false, true, true), function(e) {
					var k = e.keyCode;
					if (k >= 49 && k < 58) {
						k = (k-49) * opts.items.visible;
						if (k <= itms.total) {
							e.preventDefault();
							$cfs.trigger(cf_e('slideTo', conf), [k, 0, true, opts.pagination]);
						}
					}
				});
			}
			if (opts.auto.play) {
				$cfs.trigger(cf_e('play', conf), opts.auto.delay);
			}

if (crsl.upDateOnWindowResize) {
	$(window).bind(cf_e('resize', conf, false, true, true), function(e) {
		$cfs.trigger(cf_e('finish', conf));
		if (opts.auto.pauseOnResize && !crsl.isPaused) {
			$cfs.trigger(cf_e('play', conf));
		}
		sz_resetMargin($cfs.children(), opts);
		$cfs._cfs_init(opts_orig);
		var siz = sz_setSizes($cfs, opts, false);
		$cfs.trigger(cf_e('updatePageStatus', conf), [true, siz]);
	});
}

		};	//	/bind_buttons

		$cfs._cfs_unbind_buttons = function() {
			var ns1 = cf_e('', conf),
				ns2 = cf_e('', conf, false);
				ns3 = cf_e('', conf, false, true, true);

			$(document).unbind(ns3);
			$(window).unbind(ns3);
			$wrp.unbind(ns2);

			if (opts.auto.button) opts.auto.button.unbind(ns2);
			if (opts.prev.button) opts.prev.button.unbind(ns2);
			if (opts.next.button) opts.next.button.unbind(ns2);
			if (opts.pagination.container) {
				opts.pagination.container.unbind(ns2);
				if (opts.pagination.anchorBuilder) {
					opts.pagination.container.children().remove();
				}
			}

			nv_showNavi(opts, 'hide', conf);
			nv_enableNavi(opts, 'removeClass', conf);

		};	//	/unbind_buttons



		//	START

		var crsl = {
				'direction'		: 'next',
				'isPaused'		: true,
				'isScrolling'	: false,
				'isStopped'		: false,

				'mousewheelNext': false,
				'mousewheelPrev': false,
				'touchwipe'		: false
			},
			itms = {
				'total'			: $cfs.children().length,
				'first'			: 0
			},
			tmrs = {
				'timer'			: null,
				'auto'			: null,
				'queue'			: null,
				'startTime'		: getTime(),
				'timePassed'	: 0
			},
			scrl = {
				'isStopped'		: false,
				'duration'		: 0,
				'startTime'		: 0,
				'easing'		: '',
				'anims'			: []
			},
			clbk = {
				'onBefore'		: [],
				'onAfter'		: []
			},
			queu = [],
			conf = $.extend(true, {}, $.fn.carouFredSel.configs, configs),
			opts = {},
			opts_orig = options,
			$wrp = $cfs.wrap('<'+conf.wrapper.element+' class="'+conf.wrapper.classname+'" />').parent();

		conf.selector		= $cfs.selector;
		conf.serialNumber	= $.fn.carouFredSel.serialNumber++;

		//	create carousel
		$cfs._cfs_init(opts_orig, true, starting_position);
		$cfs._cfs_build();
		$cfs._cfs_bind_events();
		$cfs._cfs_bind_buttons();

		//	find item to start
		if (is_array(opts.items.start)) {
			var start_arr = opts.items.start;
		} else {
			var start_arr = [];
			if (opts.items.start != 0) {
				start_arr.push(opts.items.start);
			}
		}
		if (opts.cookie) {
			start_arr.unshift(cf_readCookie(opts.cookie));
		}
		if (start_arr.length > 0) {
			for (var a = 0, l = start_arr.length; a < l; a++) {
				var s = start_arr[a];
				if (s == 0) {
					continue;
				}
				if (s === true) {
					s = window.location.hash;
					if (s.length < 1) {
						continue;
					}
				} else if (s === 'random') {
					s = Math.floor(Math.random()*itms.total);
				}
				if ($cfs.triggerHandler(cf_e('slideTo', conf), [s, 0, true, { fx: 'none' }])) {
					break;
				}
			}
		}
		var siz = sz_setSizes($cfs, opts, false),
			itm = gi_getCurrentItems($cfs.children(), opts);

		if (opts.onCreate) {
			opts.onCreate.call($tt0, itm, siz);
		}

		$cfs.trigger(cf_e('updatePageStatus', conf), [true, siz]);
		$cfs.trigger(cf_e('linkAnchors', conf));

		return $cfs;
	};


	//	GLOBAL PUBLIC

	$.fn.carouFredSel.serialNumber = 1;
	$.fn.carouFredSel.defaults = {
		'synchronise'	: false,
		'infinite'		: true,
		'circular'		: true,
		'responsive'	: false,
		'direction'		: 'left',
		'items'			: {
			'start'			: 0
		},
		'scroll'		: {
			'easing'		: 'swing',
			'duration'		: 500,
			'pauseOnHover'	: false,
			'mousewheel'	: false,
			'wipe'			: false,
			'event'			: 'click',
			'queue'			: false
		}
	};
	$.fn.carouFredSel.configs = {
		'debug'			: false,
		'events'		: {
			'prefix'		: '',
			'namespace'		: 'cfs'
		},
		'wrapper'		: {
			'element'		: 'div',
			'classname'		: 'caroufredsel_wrapper'
		},
		'classnames'	: {}
	};
	$.fn.carouFredSel.pageAnchorBuilder = function(nr, itm) {
		return '<a href="#"><span>'+nr+'</span></a>';
	};


	//	GLOBAL PRIVATE

	//	scrolling functions
	function sc_setScroll(d, e) {
		return {
			anims		: [],
			duration	: d,
			orgDuration	: d,
			easing		: e,
			startTime	: getTime()
		};
	}
	function sc_startScroll(s) {
		if (typeof s.pre == 'object') {
			sc_startScroll(s.pre);
		}
		for (var a = 0, l = s.anims.length; a < l; a++) {
			var b = s.anims[a];
			if (!b) continue;
			if (b[3]) b[0].stop();
			b[0].animate(b[1], {
				complete: b[2],
				duration: s.duration,
				easing: s.easing
			});
		}
		if (typeof s.post == 'object') {
			sc_startScroll(s.post);
		}
	}
	function sc_stopScroll(s, finish) {
		if (typeof finish != 'boolean') finish = true;
		if (typeof s.pre == 'object') {
			sc_stopScroll(s.pre, finish);
		}
		for (var a = 0, l = s.anims.length; a < l; a++) {
			var b = s.anims[a];
			b[0].stop(true);
			if (finish) {
				b[0].css(b[1]);
				if (typeof b[2] == 'function') b[2]();
			}
		}
		if (typeof s.post == 'object') {
			sc_stopScroll(s.post, finish);
		}
	}
	function sc_clearTimers(t) {
		if (t.auto) clearTimeout(t.auto);
		return t;
	}
	function sc_callCallbacks(cbs, t, args) {
		if (cbs.length) {
			for (var a = 0, l = cbs.length; a < l; a++) {
				cbs[a].apply(t, args);
			}
		}
		return [];
	}

	//	fx functions
	function fx_fade(sO, c, x, d, f) {
		var o = {
			'duration'	: d,
			'easing'	: sO.easing
		};
		if (typeof f == 'function') o.complete = f;
		c.animate({
			opacity: x
		}, o);
	}
	function fx_cover(sc, c1, c2, o, prev) {
		var old_w = ms_getSizes(gi_getOldItemsNext(c1.children(), o), o, true)[0],
			new_w = ms_getSizes(c2.children(), o, true)[0],
			cur_l = (prev) ? -new_w : old_w,
			css_o = {},
			ani_o = {};

		css_o[o.d['width']] = new_w;
		css_o[o.d['left']] = cur_l;
		ani_o[o.d['left']] = 0;
		
		sc.pre.anims.push([c1, { 'opacity': 1 }]);
		sc.post.anims.push([c2, ani_o, function() { $(this).remove(); }]);
		c2.css(css_o);
		return sc;
	}
	function fx_uncover(sc, c1, c2, o, prev, n) {
		var new_w = ms_getSizes(gi_getNewItemsNext(c1.children(), o, n), o, true)[0],
			old_w = ms_getSizes(c2.children(), o, true)[0],
			cur_l = (prev) ? -old_w : new_w,
			css_o = {},
			ani_o = {};

		css_o[o.d['width']] = old_w;
		css_o[o.d['left']] = 0;
		ani_o[o.d['left']] = cur_l;
		sc.post.anims.push([c2, ani_o, function() { $(this).remove(); }]);
		c2.css(css_o);
		return sc;
	}

	//	navigation functions
	function nv_showNavi(o, t, c) {
		if (t == 'show' || t == 'hide') {
			var f = t;
		} else if (o.items.minimum >= t) {
			debug(c, 'Not enough items: hiding navigation ('+t+' items, '+o.items.minimum+' needed).');
			var f = 'hide';
		} else {
			var f = 'show';
		}
		var s = (f == 'show') ? 'removeClass' : 'addClass',
			h = cf_c('hidden', c);
		if (o.auto.button) o.auto.button[f]()[s](h);
		if (o.prev.button) o.prev.button[f]()[s](h);
		if (o.next.button) o.next.button[f]()[s](h);
		if (o.pagination.container) o.pagination.container[f]()[s](h);
	}
	function nv_enableNavi(o, f, c) {
		if (o.circular || o.infinite) return;
		var fx = (f == 'removeClass' || f == 'addClass') ? f : false,
			di = cf_c('disabled', c);
		if (o.auto.button && fx) {
			o.auto.button[fx](di);
		}
		if (o.prev.button) {
			var fn = fx || (f == 0) ? 'addClass' : 'removeClass';
			o.prev.button[fn](di);
		}
		if (o.next.button) {
			var fn = fx || (f == o.items.visible) ? 'addClass' : 'removeClass';
			o.next.button[fn](di);
		}
	}

	//	get object functions
	function go_getObject($tt, obj) {
		if (typeof obj == 'function')	obj = obj.call($tt);
		if (typeof obj == 'undefined')	obj = {};
		return obj;
	}
	function go_getNaviObject($tt, obj, type) {
		if (typeof type != 'string') type = '';

		obj = go_getObject($tt, obj);
		if (typeof obj == 'string') {
			var temp = cf_getKeyCode(obj);
			if (temp == -1) obj = $(obj);
			else 			obj = temp;
		}

		//	pagination
		if (type == 'pagination') {
			if (typeof obj 				== 'boolean')	obj = { 'keys': obj };
			if (typeof obj.jquery 		!= 'undefined')	obj = { 'container': obj };
			if (typeof obj.container	== 'function')	obj.container = obj.container.call($tt);
			if (typeof obj.container	== 'string')	obj.container = $(obj.container);
			if (typeof obj.items		!= 'number')	obj.items = false;

		//	auto
		} else if (type == 'auto') {
			if (typeof obj.jquery	!= 'undefined')		obj = { 'button': obj };
			if (typeof obj == 'boolean')				obj = { 'play': obj };
			if (typeof obj == 'number')					obj = { 'pauseDuration': obj };
			if (typeof obj.button		== 'function')	obj.button = obj.button.call($tt);
			if (typeof obj.button		== 'string')	obj.button = $(obj.button);

		//	prev + next
		} else {
			if (typeof obj.jquery		!= 'undefined')	obj = { 'button': obj };
			if (typeof obj 				== 'number')	obj = { 'key': obj };
			if (typeof obj.button		== 'function')	obj.button = obj.button.call($tt);
			if (typeof obj.button		== 'string')	obj.button = $(obj.button);
			if (typeof obj.key			== 'string')	obj.key = cf_getKeyCode(obj.key);
		}			

		return obj;
	}

	//	get number functions
	function gn_getItemIndex(num, dev, org, items, $cfs) {
		if (typeof num == 'string') {
			if (isNaN(num)) num = $(num);
			else 			num = parseInt(num);
		}
		if (typeof num == 'object') {
			if (typeof num.jquery == 'undefined') num = $(num);
			num = $cfs.children().index(num);
			if (num == -1) num = 0;
			if (typeof org != 'boolean') org = false;
		} else {
			if (typeof org != 'boolean') org = true;
		}
		if (isNaN(num))	num = 0;
		else 			num = parseInt(num);
		if (isNaN(dev))	dev = 0;
		else 			dev = parseInt(dev);

		if (org) {
			num += items.first;
		}
		num += dev;
		if (items.total > 0) {
			while (num >= items.total)	{	num -= items.total; }
			while (num < 0)				{	num += items.total; }
		}
		return num;
	}

	//	items prev
	function gn_getVisibleItemsPrev(i, o, s) {
		var t = 0,
			x = 0;

		for (var a = s; a >= 0; a--) {
			var j = i.eq(a);
			t += (j.is(':visible')) ? j[o.d['outerWidth']](true) : 0;
			if (t > o.maxDimention) return x;
			if (a == 0) a = i.length;
			x++;
		}
	}
	function gn_getVisibleItemsPrevFilter(i, o, s) {
		return gn_getItemsPrevFilter(i, o.items.filter, o.items.visibleConf.org, s);
	}
	function gn_getScrollItemsPrevFilter(i, o, s, m) {
		return gn_getItemsPrevFilter(i, o.items.filter, m, s);
	}
	function gn_getItemsPrevFilter(i, f, m, s) {
		var t = 0,
			x = 0;
	
		for (var a = s, l = i.length-1; a >= 0; a--) {
			x++;
			if (x == l) return x;
	
			var j = i.eq(a);
			if (j.is(f)) {
				t++;
				if (t == m) return x;
			}
			if (a == 0) a = i.length;
		}
	}

	function gn_getVisibleOrg($c, o) {
		return o.items.visibleConf.org || $c.children().slice(0, o.items.visible).filter(o.items.filter).length;
	}

	//	items next
	function gn_getVisibleItemsNext(i, o, s) {
		var t = 0,
			x = 0;

		for (var a = s, l = i.length-1; a <= l; a++) {
			var j = i.eq(a);

			t += (j.is(':visible')) ? j[o.d['outerWidth']](true) : 0;
			if (t > o.maxDimention) return x;

			x++;
			if (x == l) return x;
			if (a == l) a = -1;
		}
	}
	function gn_getVisibleItemsNextTestCircular(i, o, s, l) {
		var v = gn_getVisibleItemsNext(i, o, s);
		if (!o.circular) {
			if (s + v > l) v = l - s;
		}
		return v;
	}
	function gn_getVisibleItemsNextFilter(i, o, s) {
		return gn_getItemsNextFilter(i, o.items.filter, o.items.visibleConf.org, s, o.circular);
	}
	function gn_getScrollItemsNextFilter(i, o, s, m) {
		return gn_getItemsNextFilter(i, o.items.filter, m+1, s, o.circular) - 1;
	}
	function gn_getItemsNextFilter(i, f, m, s, c) {
		var t = 0,
			x = 0;

		for (var a = s, l = i.length-1; a <= l; a++) {
			x++;
			if (x == l) return x;

			var j = i.eq(a);
			if (j.is(f)) {
				t++;
				if (t == m) return x;
			}
			if (a == l) a = -1;
		}
	}

	//	get items functions
	function gi_getCurrentItems(i, o) {
		return i.slice(0, o.items.visible);
	}
	function gi_getOldItemsPrev(i, o, n) {
		return i.slice(n, o.items.visibleConf.old+n);
	}
	function gi_getNewItemsPrev(i, o) {
		return i.slice(0, o.items.visible);
	}
	function gi_getOldItemsNext(i, o) {
		return i.slice(0, o.items.visibleConf.old);
	}
	function gi_getNewItemsNext(i, o, n) {
		return i.slice(n, o.items.visible+n);
	}

	//	sizes functions
	function sz_resetMargin(i, o, m) {
		var x = (typeof m == 'boolean') ? m : false;
		if (typeof m != 'number') m = 0;
		i.each(function() {
			var j = $(this);
			var t = parseInt(j.css(o.d['marginRight']));
			if (isNaN(t)) t = 0;
			j.data('cfs_tempCssMargin', t);
			j.css(o.d['marginRight'], ((x) ? j.data('cfs_tempCssMargin') : m + j.data('cfs_origCssMargin')));
		});
	}
	function sz_setSizes($c, o, p) {
		var $w = $c.parent(),
			$i = $c.children(),
			$v = gi_getCurrentItems($i, o),
			sz = cf_mapWrapperSizes(ms_getSizes($v, o, true), o, p);

		$w.css(sz);

		if (o.usePadding) {
			var p = o.padding,
				r = p[o.d[1]];
			if (o.align) {
				if (r < 0) r = 0;
			}
			var $l = $v.last();
			$l.css(o.d['marginRight'], $l.data('cfs_origCssMargin') + r);
			$c.css(o.d['top'], p[o.d[0]]);
			$c.css(o.d['left'], p[o.d[3]]);
		}

		$c.css(o.d['width'], sz[o.d['width']]+(ms_getTotalSize($i, o, 'width')*2));
		$c.css(o.d['height'], ms_getLargestSize($i, o, 'height'));
		return sz;
	}

	//	measuring functions
	function ms_getSizes(i, o, wrapper) {
		var s1 = ms_getTotalSize(i, o, 'width', wrapper),
			s2 = ms_getLargestSize(i, o, 'height', wrapper);
		return [s1, s2];
	}
	function ms_getLargestSize(i, o, dim, wrapper) {
		if (typeof wrapper != 'boolean') wrapper = false;
		if (typeof o[o.d[dim]] == 'number' && wrapper) return o[o.d[dim]];
		if (typeof o.items[o.d[dim]] == 'number') return o.items[o.d[dim]];
		var di2 = (dim.toLowerCase().indexOf('width') > -1) ? 'outerWidth' : 'outerHeight';
		return ms_getTrueLargestSize(i, o, di2);
	}
	function ms_getTrueLargestSize(i, o, dim) {
		var s = 0;

		for (var a = 0, l = i.length; a < l; a++) {
			var j = i.eq(a);

			var m = (j.is(':visible')) ? j[o.d[dim]](true) : 0;
			if (s < m) s = m;
		}
		return s;
	}
	function ms_getTrueInnerSize($el, o, dim) {
		if (!$el.is(':visible')) return 0;

		var siz = $el[o.d[dim]](),
			arr = (o.d[dim].toLowerCase().indexOf('width') > -1) ? ['paddingLeft', 'paddingRight'] : ['paddingTop', 'paddingBottom'];
		
		for (var a = 0, l = arr.length; a < l; a++) {
			var m = parseInt($el.css(arr[a]));
			siz -= (isNaN(m)) ? 0 : m;
		}
		return siz;
	}
	function ms_getTotalSize(i, o, dim, wrapper) {
		if (typeof wrapper != 'boolean') wrapper = false;
		if (typeof o[o.d[dim]] == 'number' && wrapper) return o[o.d[dim]];
		if (typeof o.items[o.d[dim]] == 'number') return o.items[o.d[dim]] * i.length;
		
		var d = (dim.toLowerCase().indexOf('width') > -1) ? 'outerWidth' : 'outerHeight',
			s = 0;
		
		for (var a = 0, l = i.length; a < l; a++) {
			var j = i.eq(a);
			s += (j.is(':visible')) ? j[o.d[d]](true) : 0;
		}
		return s;
	}
	function ms_hasVariableSizes(i, o, dim) {
		var s = false,
			v = false;
		
		for (var a = 0, l = i.length; a < l; a++) {
			var j = i.eq(a);

			var c = (j.is(':visible')) ? j[o.d[dim]](true) : 0;
			if (s === false) s = c;
			else if (s != c) v = true;
			if (s == 0)		 v = true;
		}
		return v;
	}
	function ms_getPaddingBorderMargin(i, o, d) {
		return i[o.d['outer'+d]](true) - ms_getTrueInnerSize(i, o, 'inner'+d);
	}
	function ms_isPercentage(x) {
		return (typeof x == 'string' && x.slice(-1) == '%');
	}
	function ms_getPercentage(s, o) {
		if (ms_isPercentage(o)) {
			o = o.slice(0, -1);
			if (isNaN(o)) return s;
			s *= o/100;
		}
		return s;
	}

	//	config functions
	function cf_e(n, c, pf, ns, rd) {
		if (typeof pf != 'boolean') pf = true;
		if (typeof ns != 'boolean') ns = true;
		if (typeof rd != 'boolean') rd = false;
		
		if (pf) n = c.events.prefix + n;
		if (ns) n = n +'.'+ c.events.namespace;
		if (ns && rd) n += c.serialNumber;

		return n;
	}
	function cf_c(n, c) {
		return (typeof c.classnames[n] == 'string') ? c.classnames[n] : n;
	}
	function cf_mapWrapperSizes(ws, o, p) {
		if (typeof p != 'boolean') p = true;
		var pad = (o.usePadding && p) ? o.padding : [0, 0, 0, 0];
		var wra = {};
			wra[o.d['width']] = ws[0] + pad[1] + pad[3];
			wra[o.d['height']] = ws[1] + pad[0] + pad[2];

		return wra;
	}
	function cf_sortParams(vals, typs) {
		var arr = [];
		for (var a = 0, l1 = vals.length; a < l1; a++) {
			for (var b = 0, l2 = typs.length; b < l2; b++) {
				if (typs[b].indexOf(typeof vals[a]) > -1 && typeof arr[b] == 'undefined') {
					arr[b] = vals[a];
					break;
				}
			}
		}
		return arr;
	}
	function cf_getPadding(p) {
		if (typeof p == 'undefined') return [0, 0, 0, 0];
		
		if (typeof p == 'number') return [p, p, p, p];
		else if (typeof p == 'string') p = p.split('px').join('').split('em').join('').split(' ');

		if (!is_array(p)) {
			return [0, 0, 0, 0];
		}
		for (var i = 0; i < 4; i++) {
			p[i] = parseInt(p[i]);
		}
		switch (p.length) {
			case 0:	return [0, 0, 0, 0];
			case 1: return [p[0], p[0], p[0], p[0]];
			case 2: return [p[0], p[1], p[0], p[1]];
			case 3: return [p[0], p[1], p[2], p[1]];
			default: return [p[0], p[1], p[2], p[3]];
		}
	}
	function cf_getAlignPadding(itm, o) {
		var x = (typeof o[o.d['width']] == 'number') ? Math.ceil(o[o.d['width']] - ms_getTotalSize(itm, o, 'width')) : 0;
		switch (o.align) {
			case 'left': return [0, x];
			case 'right': return [x, 0];
			case 'center':
			default:
				return [Math.ceil(x/2), Math.floor(x/2)];
		}
	}
	function cf_getAdjust(x, o, a, $t) {
		var v = x;
		if (typeof a == 'function') {
			v = a.call($t, v);

		} else if (typeof a == 'string') {
			var p = a.split('+'),
				m = a.split('-');
			
			if (m.length > p.length) {
				var neg = true,
					sta = m[0],
					adj = m[1];
			} else {
				var neg = false,
					sta = p[0],
					adj = p[1];
			}

			switch(sta) {
				case 'even':
					v = (x % 2 == 1) ? x-1 : x;
					break;
				case 'odd':
					v = (x % 2 == 0) ? x-1 : x;
					break;
				default:
					v = x;
					break;
			}
			adj = parseInt(adj);
			if (!isNaN(adj)) {
				if (neg) adj = -adj;
				v += adj;
			}
		}
		if (typeof v != 'number') v = 1;
		if (v < 1) v = 1;
		return v;
	}
	function cf_getItemsAdjust(x, o, a, $t) {
		return cf_getItemAdjustMinMax(cf_getAdjust(x, o, a, $t), o.items.visibleConf);
	}
	function cf_getItemAdjustMinMax(v, i) {
		if (typeof i.min == 'number' && v < i.min) v = i.min;
		if (typeof i.max == 'number' && v > i.max) v = i.max;
		if (v < 1) v = 1;
		return v;
	}
	function cf_getSynchArr(s) {
		if (!is_array(s)) 		s = [[s]];
		if (!is_array(s[0]))	s = [s];
		for (var j = 0, l = s.length; j < l; j++) {
			if (typeof s[j][0] == 'string')		s[j][0] = $(s[j][0]);
			if (typeof s[j][1] != 'boolean')	s[j][1] = true;
			if (typeof s[j][2] != 'boolean')	s[j][2] = true;
			if (typeof s[j][3] != 'number')		s[j][3] = 0;
		}
		return s;
	}
	function cf_getKeyCode(k) {
		if (k == 'right')	return 39;
		if (k == 'left')	return 37;
		if (k == 'up')		return 38;
		if (k == 'down')	return 40;
		return -1;
	}
	function cf_setCookie(n, v) {
		if (n) document.cookie = n+'='+v+'; path=/';
	}
	function cf_readCookie(n) {
		n += '=';
		var ca = document.cookie.split(';');
		for (var a = 0, l = ca.length; a < l; a++) {
			var c = ca[a];
			while (c.charAt(0) == ' ') {
				c = c.slice(1);
			}
			if (c.indexOf(n) == 0) {
				return c.slice(n.length);
			}
		}
		return 0;
	}

	//	buttons functions
	function bt_pauseOnHoverConfig(p) {
		if (p && typeof p == 'string') {
			var i = (p.indexOf('immediate') > -1) ? true : false,
				r = (p.indexOf('resume') 	> -1) ? true : false;
		} else {
			var i = r = false;
		}
		return [i, r];
	}
	function bt_mousesheelNumber(mw) {
		return (typeof mw == 'number') ? mw : null
	}

	//	helper functions
	function is_array(a) {
		return typeof(a) == 'object' && (a instanceof Array);
	}

	function getTime() {
		return new Date().getTime();
	}

	function debug(d, m) {
		if (typeof d == 'object') {
			var s = ' ('+d.selector+')';
			d = d.debug;
		} else {
			var s = '';
		}
		if (!d) return false;
		
		if (typeof m == 'string') m = 'carouFredSel'+s+': ' + m;
		else m = ['carouFredSel'+s+':', m];

		if (window.console && window.console.log) window.console.log(m);
		return false;
	}


	//	CAROUFREDSEL ALL LOWERCASE

	$.fn.caroufredsel = function(o, c) {
		return this.carouFredSel(o, c);
	};


	//	EASING FUNCTIONS

	$.extend($.easing, {
		'quadratic'	: function(t) {
			var t2 = t * t;
			return t * (-t2 * t + 4 * t2 - 6 * t + 4);
		},
		'cubic'		: function(t) {
			return t * (4 * t * t - 9 * t + 6);
		},
		'elastic'	: function(t) {
			var t2 = t * t;
			return t * (33 * t2 * t2 - 106 * t2 * t + 126 * t2 - 67 * t + 15);
		}
	});


})(jQuery);
//jquery carofredsel ends here

//jquery respponsive slides plugin
/*! ResponsiveSlides.js v1.23
 * http://responsive-slides.viljamis.com
 *
 * Copyright (c) 2011-2012 @viljamis
 * Available under the MIT license
 */

/* ResponsiveSlides.js is a tiny jQuery plugin that creates a responsive
 * slideshow using images inside <ul>. It works with wide range of browsers
 * including all IE versions from IE6 and up. It also adds css max-width
 * support for IE6 and other browsers that don't natively support it. Only
 * dependency is jQuery (1.4 and up) and that all the images are same size.
 *
 * Features:
 * - Fully responsive
 * - Under 1kb minified and gzipped
 * - Simple markup using unordered lists
 * - Settings for transition and timeout durations
 * - Multiple slideshows supported
 * - Automatic and manual fade
 * - Works in all major desktop and mobile browsers
 * - Captions and other html-elements supported inside slides
 * - Separate pagination and next/prev controls
 * - Possibility to choose where the controls append to
 * - Images can be wrapped inside links
 * - Optional 'before' and 'after' callbacks
 *
 */

/*jslint browser: true, sloppy: true, vars: true, plusplus: true, maxerr: 50, indent: 2 */

(function ($, window, i) {
  $.fn.responsiveSlides = function (options) {

    // Default settings
    var settings = $.extend({
      "auto": true,             // Boolean: Animate automatically, true or false
      "speed": 1000,            // Integer: Speed of the transition, in milliseconds
      "timeout": 4000,          // Integer: Time between slide transitions, in milliseconds
      "pager": false,           // Boolean: Show pager, true or false
      "nav": false,             // Boolean: Show navigation, true or false
      "prevText": "Previous",   // String: Text for the "previous" button
      "nextText": "Next",       // String: Text for the "next" button
      "maxwidth": "none",       // Integer: Max-width of the slideshow, in pixels
      "controls": "",           // Selector: Where controls should be appended to, default is after the <ul>
      "namespace": "rslides"    // String: change the default namespace used
    }, options);

    return this.each(function () {

      // Index for namespacing
      i++;

      var $this = $(this),

        // Local variables
        selectTab,
        startCycle,
        restartCycle,
        rotate,
        $tabs,

        // Helpers
        index = 0,
        $slide = $this.children(),
        length = $slide.size(),
        fadetime = parseFloat(settings.speed),

        // Namespacing
        namespace = settings.namespace,
        namespaceIdx = namespace + i,

        // Classes
        namespaceIdxClass = namespace + " " + namespaceIdx,
        navClass = namespace + "_nav " + namespaceIdx + "_nav",
        activeClass = namespace + "_here",
        visibleClass = namespaceIdx + "_on",
        slideClassPrefix = namespaceIdx + "_s",

        // Pager
       $pager = $(".mytabs"),

        // Styles for visible and hidden slides
        visible = {"float": "left", "position": "relative"},
        hidden = {"float": "none", "position": "absolute"},

        // Fading animation
        slideTo = function (idx) {
          $this.trigger(namespace + "-before");
          $slide
            .stop()
            .fadeOut(fadetime, function () {
              $(this)
                .removeClass(visibleClass)
                .css(hidden);
            })
            .eq(idx)
            .fadeIn(fadetime, function () {
              $(this)
                .addClass(visibleClass)
                .css(visible)
                .trigger(namespace + "-after");
              index = idx;
            });
        };

      // Only run if there's more than one slide
      if ($slide.size() > 1) {

        // Add ID's to each slide
        $slide.each(function (i) {
          this.id = slideClassPrefix + i;
        });

        // Add max-width and classes
        $this
          .css("max-width", settings.maxwidth)
          .addClass(namespaceIdxClass);

        // Hide all slides, then show first one
        $slide
          .hide()
          .eq(0)
          .addClass(visibleClass)
          .css(visible)
          .show();

        // Pager
        if (settings.pager === true) {
          var tabMarkup = [];
          $slide.each(function (i) {
            var n = i + 1;
            tabMarkup +=
              "<li>" +
              "<a href='#' class='" + slideClassPrefix + n + "'>" + n + "</a>" +
              "</li>";
          });
         // $pager.append(tabMarkup);

          $tabs = $pager.find("a");

          // Inject pager
          if (options.controls) {
            $(settings.controls).append($pager);
          } else {
            $this.after($pager);
          }

          // Select pager item
          selectTab = function (idx) {
            $tabs
              .closest("li")
              .removeClass(activeClass)
              .eq(idx)
              .addClass(activeClass);
          };
        }

        // Auto cycle
        if (settings.auto === true) {

          startCycle = function () {
            rotate = setInterval(function () {
              var idx = index + 1 < length ? index + 1 : 0;

              // Remove active state and set new if pager = "true"
              if (settings.pager === true) {
                selectTab(idx);
              }

              slideTo(idx);
            }, parseFloat(settings.timeout));
          };

          // Init cycle
          startCycle();
        }

        // Restarting cycle
        restartCycle = function () {
          if (settings.auto === true) {
            // Stop
            clearInterval(rotate);
            // Restart
            startCycle();
          }
        };

        // Pager click event handler
        if (settings.pager === true) {
          $tabs.bind("click", function (e) {
            e.preventDefault();
            restartCycle();

            // Get index of clicked tab
            var idx = $tabs.index(this);

            // Break if element is already active or currently animated
            if (index === idx || $("." + visibleClass + ":animated").length) {
              return;
            }

            // Remove active state from old tab and set new one
            selectTab(idx);

            // Do the animation
            slideTo(idx);
          })
            .eq(0)
            .closest("li")
            .addClass(activeClass);
        }

      }

      // Navigation
      if (settings.nav === true) {
        var navMarkup =
          "<a href='#' class='" + navClass + " prev'>" + settings.prevText + "</a>" +
          "<a href='#' class='" + navClass + " next'>" + settings.nextText + "</a>";

        // Inject navigation
        if (options.controls) {
          $(settings.controls).append(navMarkup);
        } else {
          $this.after(navMarkup);
        }

        var $trigger = $("." + namespaceIdx + "_nav"),
        $prev = false;
		
		

        // Click event handler
        $trigger.bind("click", function (e) {
          e.preventDefault();

		
		if($(this).hasClass('prev'))
			$prev = true;
		else
			$prev = false;
		
		  
		// Prevent clicking if currently animated
		if ($("." + visibleClass + ":animated").length)
		{
			return;
		}

          // Determine where to slide
          var idx = $slide.index($("." + visibleClass)),
            prevIdx = idx - 1,
            nextIdx = idx + 1 < length ? index + 1 : 0;
			
          // Go to slide
          slideTo($prev ? prevIdx : nextIdx);
          if (settings.pager === true) {
            selectTab($prev ? prevIdx : nextIdx);
          }

          restartCycle();
        });
      }

      // Max-width fallback
      if (typeof document.body.style.maxWidth === "undefined" && options && options.maxwidth) {
        var widthSupport = function () {
          $this.css("width", "100%");
          if ($this.width() > parseFloat(settings.maxwidth)) {
            $this.css("width", parseFloat(settings.maxwidth));
          }
        };

        // Init fallback
        widthSupport();
        $(window).bind("resize", function () {
          widthSupport();
        });
      }

    });

  };
})(jQuery, this, 0);

//jquery responsive slides plugin ends

