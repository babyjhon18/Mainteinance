/*
 * jQuery liFixar v 3.1
 *
 * Copyright 2013, Linnik Yura | LI MASS CODE | http://masscode.ru
 * http://masscode.ru/index.php/k2/item/48-lifixar
 * Free to use
 * 
 * Last Update: 27.01.2014
 */
 (function ($) {
    var methods = {
        init: function (options) {
			var p = {
				side: 'top',	
         		position: '0',	
         		fix: function (el, side) {},   
         		unfix: function (el, side) {}
			};
			if (options) {
				$.extend(p, options); 
			}
			
			return this.each(function(){			
				 
				var
				menu = $(this),
				w = $(window),
				
				//size
				menuH,
				menuW,
				
				//borders
				menuBorderTop,
				menuBorderBottom,
				menuBorderLeft,
				menuBorderRight,
				
				//padding
				menuPaddingTop,
				menuPaddingBottom,
				menuPaddingLeft,
				menuPaddingRight,
				
				//offset
				menuOffset,
				menuTop,
				menuLeft,
				
				//margin
				menuMarginTop,
				menuMarginBottom,
				menuMarginLeft,
				menuMarginRight,
				
				//position
				menuCssPos,
				menuCssLeft,
				menuCssRight,
				menuCssBottom,
				menuCssTop,
				
				//window size
				wH,
				wW,
				fMess = true,
				posLength = p.position.toString().split(' ').length,
				posTop = p.position,
				posBot = p.position;
				
				
				
				if(posLength == 2){
					posTop = p.position.split(' ')[0];
					posBot = p.position.split(' ')[1]
				}
				menu.data({
					'posTop':posTop,
					'posBot':posBot,
					'update':'false'
				})
				
				
				var sizeRecount = function(m){
					
					
					
					//inner size
					menuH = parseFloat(m.height()) || 0;
					menuW = parseFloat(m.width()) || 0;

					//borders
					menuBorderTop = parseFloat(m.css('borderTopWidth')) || 0;
					
				
					
					
					menuBorderBottom = parseFloat(m.css('borderBottomWidth')) || 0;
					menuBorderLeft = parseFloat(m.css('borderLeftWidth')) || 0;
					menuBorderRight = parseFloat(m.css('borderRightWidth')) || 0;
					
					//padding
					menuPaddingTop = parseFloat(m.css('paddingTop')) || 0;
					menuPaddingBottom = parseFloat(m.css('paddingBottom')) || 0;
					menuPaddingLeft = parseFloat(m.css('paddingLeft')) || 0;
					menuPaddingRight = parseFloat(m.css('paddingRight')) || 0;
					
					//margin
					menuMarginTop = parseFloat(m.css('margin-top')) || 0;
					menuMarginBottom = parseFloat(m.css('margin-bottom')) || 0;
					menuMarginLeft = parseFloat(m.css('margin-left')) || 0;
					menuMarginRight = parseFloat(m.css('margin-right')) || 0;
					
					
					
					
					//offset
					menuOffset = m.offset();
					menuTop = parseFloat(menuOffset.top) || 0;
					menuLeft = parseFloat(menuOffset.left) || 0;
					
					
					//position
					menuCssPos = m.css('position');
					menuCssLeft = parseFloat(m.css('left')) || m.position().left;
					menuCssRight = parseFloat(m.css('right')) || m.position().right;
					menuCssBottom = parseFloat(m.css('bottom')) || m.position().bottom;
					menuCssTop = parseFloat(m.css('top')) || m.position().top;
					
					
				
					
				};

				sizeRecount(menu);
				
				
				
				
				//dubler
				var unicId = new Date().getTime();
				var menuClear = menu.clone()
				.attr('id',unicId)
				.addClass('menuClear')
				.css({visibility:'hidden',opacity:'0'});
				menu.after(menuClear);
				menu.data({
					'unicId':unicId
				})
				
				var addFix = function(dinamicTop,side){
					
					
					
					
					menu.css({
						left: parseFloat(menuClear.offset().left) - (parseFloat(menuClear.css('margin-left')) || 0),
						bottom:'auto',
						top: dinamicTop,
						position: 'fixed',
						width: menuClear.width()
					})
					.addClass('menuFixar');
					menuClear.removeClass('elFixWidth')	
					
					if(fMess){
						fMess = false;
						
						
						if (p.fix !== undefined) {
							
							p.fix(menu, side);
							
						}
					}
				}
				var removeFix = function(side){
					menu
					.css({
						left: menuClear.css('left'),
						top:menuCssTop,
						bottom:'auto',
						right:menuCssRight,
						position:menuCssPos,
						width: menuClear.width()
					})

					.removeClass('menuFixar');
					menuClear.addClass('elFixWidth');
					if(!fMess){
						fMess = true;
						if (p.unfix !== undefined) {
							p.unfix(menu, side);
						}
					}
				}
				var dinamicTop;
				var side = p.side;
				var fixarDetect = function(){			
					if(p.side == 'bottom'){
						
						
						if(w.scrollTop() + w.height() < (menuTop + menuH + parseFloat(menuBorderTop) + parseFloat(menuBorderBottom) + parseFloat(menuPaddingTop) + parseFloat(menuPaddingBottom)) + parseFloat(menu.data('posBot'))){
							
							dinamicTop = w.height() - (parseFloat(menu.data('posBot')) + menuH + parseFloat(menuBorderTop) + parseFloat(menuBorderBottom) + parseFloat(menuMarginTop) + parseFloat(menuPaddingTop) + parseFloat(menuPaddingBottom));
							
							addFix(dinamicTop,p.side);
							side = p.side;
						}else{
							removeFix(side);
						}
					}
					if(p.side == 'top'){		
						if((w.scrollTop() + parseFloat(menu.data('posTop'))) > menuTop){
							dinamicTop = (parseFloat(menu.data('posTop')) || 0) - (parseFloat(menuMarginTop) || 0);
							addFix(dinamicTop,p.side);
							side = p.side;
						}else{
							removeFix(side);
						}
					}
					
					
					
					if(p.side == 'all'){
						
						if(w.scrollTop() + w.height() < (menuTop + menuH + parseFloat(menuBorderTop) + parseFloat(menuBorderBottom) + parseFloat(menuPaddingTop) + parseFloat(menuPaddingBottom)) + parseFloat(menu.data('posBot'))){
							
							dinamicTop = w.height() - (parseFloat(menu.data('posBot')) + menuH + parseFloat(menuBorderTop) + parseFloat(menuBorderBottom) + parseFloat(menuMarginTop) + parseFloat(menuPaddingTop) + parseFloat(menuPaddingBottom));
							
							addFix(dinamicTop,'bottom');
							side = 'bottom'	
						}else{
							if((w.scrollTop() + parseFloat(menu.data('posTop'))) > menuTop){
								dinamicTop = parseFloat(menu.data('posTop')) - parseFloat(menuMarginTop);
								addFix(dinamicTop,'top');
								side = 'top'	
							}else{
								removeFix(side);	
							}
						}
					}
				};
				if(p.side == 'bottom'){
					menu.addClass('fixarBottom');	
				}
				fixarDetect()
				$(window).on('scroll',function(){
					fixarDetect();
				})
				$(window).on('resize',function(){
					
					if(menu.is('.menuFixar') || menu.data('update') == 'true'){
						menu.data({'update':'false'})
						
						sizeRecount(menuClear);	
					}else{
						
						sizeRecount(menu);		
					}
					fixarDetect()
				})
			
			})
			
		},setPosition: function (newPos) {
			var newPos = newPos.toString();
			var posLength = newPos.split(' ').length;
			var posTop = newPos;
			var posBot = newPos;
			if(posLength == 2){
				posTop = newPos.split(' ')[0];
				posBot = newPos.split(' ')[1]
			}
			$(this).data({
				'posTop':posTop,
				'posBot':posBot
			})
			$(window).trigger('resize')
		},getPosition: function () {
			return $(this).data('posTop') +' '+$(this).data('posBot');
		},update: function () {
			
			//dubler
			$('#'+$(this).data('unicId')).html($(this).html())
			$(this).data({'updata':'true'})
			$(window).trigger('resize');
		}
	};
    $.fn.liFixar = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Метод ' + method + ' в jQuery.liFixar не существует');
        }
    };
 })(jQuery);

 (function ($) {
     $.fn.hasScrollBar = function () {
         return this.get(0).scrollHeight > this.height();
     }
 })(jQuery);