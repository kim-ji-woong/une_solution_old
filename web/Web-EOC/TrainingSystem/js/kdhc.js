$(window).load(function(){
	$('#header .range').hover(
	  function () {
	  	$('#header .range').addClass('active');
	  	$('#header .ranges ul').stop().animate({height:291},400);
	  },
	  function () {
	  	$('#header .range').removeClass('active');
	  	$('#header .ranges ul').stop().animate({height:40},400);
	  }
	);
	$('#header .range h2').click(function(event) {
		if($('#header .range').hasClass('active')){
			$('#header .range').removeClass('active');
	  		$('#header .ranges ul').stop().animate({height:40},400);
	  	}else{
	  		$('#header .range').addClass('active');
	  	$('#header .ranges ul').stop().animate({height:291},400);
	  	}
	});

	//탭메뉴 공통
	$('.tab-menu > ul > li > a').click(function(e) {
		e.preventDefault();
		e.stopImmediatePropagation();

		if($(this).parent().hasClass('active')) return false;

		var target = $(this).attr('href');
		if(target.charAt(0) != "#") location.href=target;

		$('.tab_menu_target').removeClass('active');
		$(this).parent().parent().find('li').removeClass('active');

		$(target).addClass('active');
		$(this).parent().addClass('active');

	});
	$('.btn_popup').on('click',function(){
		var _w = $(this).attr('popup-width');
		var _h = $(this).attr('popup-height');
		window.open(this.href, "", "width="+_w+",height="+_h);
		return false;
	});
	$('.btn_dorpdown').on('click',function(e){
		e.preventDefault();
		e.stopImmediatePropagation();

		var $ele = $(this);
		var target = $ele.attr('href');
		if(target.charAt(0) != "#") location.href=target;

		if($(target).is(':hidden')){
			$(target).stop().slideDown('400', function() {
				$ele.parent().parent().addClass('active');

			});
		}
		else{
			$(target).stop().slideUp('400', function() {
				$ele.parent().parent().removeClass('active');

			});

		}
	});
});