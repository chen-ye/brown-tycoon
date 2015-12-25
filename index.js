function createImageFlows() {
	$('.image-flow > div').each(function(i, div) {
		$(div).find('p').prepend($("<strong></strong>").text((i+1) + '. ').addClass('number'));
	})
	$('.image-flow').each(function(_, flow) {
		var children = $(flow).children('div').addClass('page');
		
		// create progress bar:
		var barContainer = $("<div></div>").addClass('progress-bar-container').appendTo(flow);
		var bar = $("<div></div>").addClass('progress-bar').appendTo(barContainer).css('width', (1 * 100 / children.length) + '%');
		
		$(flow).bind('mousemove touchmove', function(e) {
			var self = this;
			var offset = $(self).offset(); 
		
			var x;
			if (e.originalEvent.touches){
				x = e.originalEvent.touches[0].pageX;
			} else if (e.pageX) {
				x = e.pageX;
			}
		
			var relX = x - offset.left;
			var i = Math.max(0, Math.min(children.length-1, Math.round(-0.5 + relX / $(self).width() * children.length)));
			children.hide().eq(i).show();
			$(bar).css('width', ((i+1) * 100 / children.length) + '%');
		}).bind('touchstart', function(e) {
			//e.preventDefault();
		});
	})
}

$(document).ready(function() {
	createImageFlows();
});
