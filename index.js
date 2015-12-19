function createImageFlows() {
	$('.image-flow').bind('mousemove touchmove', function(e) {
		var self = this;
		var children = $(self).children('div');
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
	}).bind('touchstart', function(e) {
		//e.preventDefault();
	});
}

$(document).ready(function() {
	createImageFlows();
});
