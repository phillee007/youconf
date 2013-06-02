(function ($) {

    AjaxSolr.ResultWidget = AjaxSolr.AbstractWidget.extend({
        start: 0,

        //beforeRequest: function () {
        //    $(this.target).html($('<img>').attr('src', 'images/ajax-loader.gif'));
        //},

        facetLinks: function (facet_field, facet_values) {
            var links = [];
            if (facet_values) {
                for (var i = 0, l = facet_values.length; i < l; i++) {
                    if (facet_values[i] !== undefined) {
                        links.push(
                          $('<a href="#"></a>')
                          .text(facet_values[i])
                          .click(this.facetHandler(facet_field, facet_values[i]))
                        );
                    }
                    else {
                        links.push('no items found in current selection');
                    }
                }
            }
            return links;
        },

        facetHandler: function (facet_field, facet_value) {
            var self = this;
            return function () {
                self.manager.store.remove('fq');
                self.manager.store.addByValue('fq', facet_field + ':' + AjaxSolr.Parameter.escapeValue(facet_value));
                self.doRequest();
                return false;
            };
        },

        afterRequest: function () {
            $(this.target).empty();
            if (this.manager.response.response.docs.length == 0) {
                $(this.target).append("<p>Unfortunately no conferences were found matching your search criteria. Please try again, or check our <a href='/conferences' title='Conferences'>Conferences page</a> for current conference listings.</p>");
            }
            for (var i = 0, l = this.manager.response.response.docs.length; i < l; i++) {
                var doc = this.manager.response.response.docs[i];
                
                $(this.target).append(this.template(doc, this.manager.response.highlighting[doc.id]));

                //var items = [];
                //items = items.concat(this.facetLinks('topics', doc.topics));
                //items = items.concat(this.facetLinks('organisations', doc.organisations));
                //items = items.concat(this.facetLinks('exchanges', doc.exchanges));

                //var $links = $('#links_' + doc.id);
                //$links.empty();
                //for (var j = 0, m = items.length; j < m; j++) {
                //    $links.append($('<li></li>').append(items[j]));
                //}
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

            var output = '<div><h2>' + doc.title + '</h2>';
            //doc.cat contains the speaker names (if any)
            if(doc.cat)
                output += '<h3 style="margin:0;">Speakers: ' + doc.cat.join(', ') + '</h3>';
            output += '<p id="links_' + doc.id + '" class="links"></p>';
            output += '<p>' + snippet + '</p></div>';
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