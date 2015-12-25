$(document).ready(function() {
	$('img[src]').each(function(i, el) {
		var src = $(el).attr('src');
		$(el).click(function() {
			if ($(el).closest('[data-disable-photo-link]').length == 0) {
				var url = 'image.html?img=' + encodeURIComponent(src);
				// window.open(url, '_blank');
				location.href = url;
			}
		})
	})
})
