var width = 960;
var height = 540;

var canvas = document.getElementById('canvas');
canvas.width = width;
canvas.height = height;
var ctx = canvas.getContext('2d');

// http://franklinta.com/2014/09/08/computing-css-matrix3d-transforms/

var buildings = null;

function fetch() {
	// var url = 'test.json';
	var url = 'http://localhost:8080/shapes?callback=?';
	$.getJSON(url, function(data) {
		// console.log(data)
		buildings = data.buildings;
		// console.log(data.buildings.length)
		setTimeout(fetch, 300);
	})
}

function render() {
	ctx.fillStyle = "black";
	ctx.fillRect(0, 0, width, height);
	
	ctx.fillStyle = "red";
	ctx.strokeStyle = 'white';
	ctx.lineWidth = 3;
	ctx.shadowBlur = 20;
	ctx.shadowColor = "purple";
	
	(buildings || []).forEach(function(building) {
		var w = building.width * width;
		var h = building.height * height;
		var x = (building.x + 0.5) * width;
		var y = (building.y + 0.5) * height;
		
		// ctx.fillRect(x-w/2, y-h/2, w, h);
		
		// console.log(building.rotation)
		ctx.save();
		ctx.translate(x, y);
		//ctx.translate(-w/2, -h/2);
		ctx.rotate(building.rotation / 180.0 * Math.PI);
		ctx.fillRect(-w/2, -h/2, w, h);
		ctx.strokeRect(-w/2, -h/2, w, h);
		ctx.restore();
		
	});
	
	requestAnimationFrame(render);
}

fetch();
render();
