
$(document).ready(function() {
	$('.card-view').each(function(i, cardView) {
		var container = $("<div></div>").appendTo(cardView).addClass('cards')
		var advance = function() {
			setCurCard(curCard+1);
		}
		
		var shuffleObjects = function(objs) {
			var shuffled = [];
			for (var i=0; i<objs.length; i++) {
				shuffled.splice(Math.floor(Math.random() * objs.length), 0, objs[i]);
			}
			return shuffled;
		}
		
		var cardIndices = [];
		
		var cards = [];
		for (var i=0; i<3; i++) {
			var card = $("<div></div>").appendTo(container).addClass('card');
			$(card).bind('click touchstart', function(e) {
				e.preventDefault();
				advance();
			});
			cards.push(card);
		}
		var data = $(shuffleObjects($(cardView).children('.content').children()));
		var curCard;
		
		var lastBottomCard = null;
		var lastCurrentCard = null;
		
		var dataIdx = function(i) {
			i = i % data.length;
			while (i < 0) i += data.length;
			return i;
		}
		
		var cardIdx = function(i) {
			i = i % cards.length;
			while (i < 0) i += cards.length;
			return i;
		}
		
		var setCurCard = function(i) { // 0..2
			var prev = cards[cardIdx(i-1)];
			var next = cards[cardIdx(i+1)];
			var cur = cards[cardIdx(i)];
			
			$(prev).empty().append($("<div></div>").append(data[dataIdx(i-1)]));
			$(next).empty().append($("<div></div>").append(data[dataIdx(i+1)]));
			$(cur).empty().append($("<div></div>").append(data[dataIdx(i)]));
			
			curCard = i;
			
			$(prev).attr('data-card-pos', 'prev');
			$(next).attr('data-card-pos', 'next');
			$(cur).attr('data-card-pos', 'cur');
			
			if (lastCurrentCard) {
				(function(card) {
					$(card).css('animation', '');
					setTimeout(function() {
						$(card).css('animation', 'send-card-to-back 0.3s ease')
					})
				})(lastCurrentCard);
			}
			
			lastBottomCard = prev;
			lastCurrentCard = cur;
			
			// console.log($(cur).text());
		}
		setCurCard(0);
	})
})
