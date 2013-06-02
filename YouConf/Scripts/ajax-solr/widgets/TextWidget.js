(function ($) {

    AjaxSolr.TextWidget = AjaxSolr.AbstractTextWidget.extend({
        init: function () {
            var self = this;

            var performRequest = function (val) {
                if (val && self.set("*" + val + "*")) {
                    self.manager.store.addByValue('hl', 'true');
                    self.manager.store.addByValue('hl.fl', 'content');
                    self.manager.store.addByValue('hl.snippets', '3');
                    self.doRequest();
                }
            };
            $(this.target).find('input:text').bind('keydown', function (e) {
                if (e.which == 13) {
                    var value = $(this).val();
                    performRequest(value);
                }
            });
            $(this.target).find('input:button').bind('click', function (e) {
                var value = $(this).siblings('input').val();
                performRequest(value);
            });
        },

        afterRequest: function () {
            $(this.target).find('input:text').val('');
        }
    });

})(jQuery);