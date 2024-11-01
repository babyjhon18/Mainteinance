/**
* jQuery Table Simple Navigation
*
* @author      Evgen Davliud <dzh21@tut.by>
* @copyright   Copyright (c) 2012, dzh
* @version     0.0.1
* @url         
*/

(function ($) {
    $.fn.extend({
        tableSimpleNavi: function (options) {

            var defaults = {
                curPage: 0,
                perPage: 13,
                prevLink: '#prev',
                nextLink: '#next'
            }
            options = $.extend(defaults, options);
            return this.each(function () {
                function rePaging() {                    
                    /* show only slice */
                    var rows = $table.find('tbody tr');
                    rows.hide().slice(o.curPage, o.curPage + o.perPage).show();

                    var numRows = rows.length;                    

                    /* show/hide navigation links */
                    if (o.curPage + o.perPage >= numRows) {
                        $(o.nextLink).css({ 'display': 'none' });
                    }
                    else {
                        $(o.nextLink).show();
                    }

                    if (o.curPage <= 0) {
                        $(o.prevLink).css({ 'display': 'none' });
                    }
                    else {
                        $(o.prevLink).show();
                    }

                }

                var o = options;
                var $table = $(this);

                $(o.nextLink).show();
                rePaging();

                $(o.nextLink).click(function () {
                    o.curPage = o.curPage + o.perPage;
                    rePaging();
                    return false;
                });

                $(o.prevLink).click(function () {
                    o.curPage = o.curPage - o.perPage;
                    rePaging();
                    return false;
                });
            });
        }
    });

})(jQuery);