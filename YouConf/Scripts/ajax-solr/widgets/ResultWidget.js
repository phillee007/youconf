(function ($) {

    AjaxSolr.ResultWidget = AjaxSolr.AbstractWidget.extend({
        start: 0,

        //beforeRequest: function () {
        //    $(this.target).html($('<img>').attr('src', 'images/ajax-loader.gif'));
        //},

        afterRequest: function () {
            $(this.target).empty();
            if (this.manager.response.response.docs.length == 0) {
                $(this.target).append("<p>Unfortunately no conferences were found matching your search criteria. Please try again, or check our <a href='/conferences' title='Conferences'>Conferences page</a> for current conference listings.</p>");
            }
            for (var i = 0, l = this.manager.response.response.docs.length; i < l; i++) {
                var doc = this.manager.response.response.docs[i];
                
                $(this.target).append(this.template(doc, this.manager.response.highlighting[doc.id]));
            }
        },

        template: function (doc, highlights) {
            var snippet = '';
            //Use the highlighted snippets first if any, otherwise use the conference abstract
            if (highlights.content && highlights.content.length > 0) {
                snippet = highlights.content.join('...') + "...";
            }
            else {
                if (doc.content[0].length > 300) {
                    snippet += doc.dateline + ' ' + doc.content[0].substring(0, 300);
                    snippet += '<span style="display:none;">' + doc.content[0].substring(300);
                    snippet += '</span> <a href="#" class="more">more</a>';
                }
                else {
                    snippet += doc.dateline + ' ' + doc.content[0];
                }
            }

            var output = '<div><h2><a href="/' + doc.hashtag + '" title="' + doc.title + '">' + doc.title + '</a></h2>';
            //doc.cat contains the speaker names (if any)
            if(doc.cat)
                output += '<h3 style="margin:0;">Speakers: ' + doc.cat.join(', ') + '</h3>';
            output += '<p><i>' + snippet + '</i></p></div>';
            return output;
        },

        init: function () {
            $(document).on('click', 'a.more', function () {
                var $this = $(this),
                    span = $this.parent().find('span');

                if (span.is(':visible')) {
                    span.hide();
                    $this.text('more');
                }
                else {
                    span.show();
                    $this.text('less');
                }

                return false;
            });
        }
    });

})(jQuery);