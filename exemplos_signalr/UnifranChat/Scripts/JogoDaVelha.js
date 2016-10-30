var vez = 'X';
var blocos = 0;

function Jogada(obj, event) {
	console.log(event);
	if (obj.className == 'box') {
		obj.className += vez == 'X' ? ' verde' : ' vermelho';
		
		if (Vitoria()) {
			setTimeout(function(){
				Reseta();
				vez = vez == 'X' ? 'O' : 'X';
				return;
			},1000);
			
		}
			
		blocos++;
		if (blocos == 9) {
			document.getElementsByClassName('titulo')[0].innerHTML = 'Ninguém venceu!';
			
			Reseta();
		}
		
		vez = vez == 'X' ? 'O' : 'X';
	}
}

function Vitoria() {
	var boxs = document.getElementsByClassName('box');
	
	if (/*1ª Horizontal*/(boxs[0].className != 'box' && boxs[0].className == boxs[1].className && boxs[1].className == boxs[2].className) ||
	    /*2ª Horizontal*/(boxs[3].className != 'box' && boxs[3].className == boxs[4].className && boxs[4].className == boxs[5].className) ||
		/*3ª Horizontal*/(boxs[6].className != 'box' && boxs[6].className == boxs[7].className && boxs[7].className == boxs[8].className) ||
		/*1ª Vertical  */(boxs[0].className != 'box' && boxs[0].className == boxs[3].className && boxs[3].className == boxs[6].className) ||
		/*2ª Vertical  */(boxs[1].className != 'box' && boxs[1].className == boxs[4].className && boxs[4].className == boxs[7].className) ||
		/*3ª Vertical  */(boxs[2].className != 'box' && boxs[2].className == boxs[5].className && boxs[5].className == boxs[8].className) ||
		/*1ª Diagonal  */(boxs[0].className != 'box' && boxs[0].className == boxs[4].className && boxs[4].className == boxs[8].className) ||
		/*2ª Diagonal  */(boxs[2].className != 'box' && boxs[2].className == boxs[4].className && boxs[4].className == boxs[6].className)){
		var mensagem = 
			'Jogador ' +
				'<span style="font-weight: bold; color: ' + (vez == 'X' ? 'lightgreen' : 'indianred') + '">' + 
					(vez == 'X' ? 'Verde' : 'Vermelho') + 
				'</span> ' +
			'ganhou!';
		document.getElementsByClassName('titulo')[0].innerHTML = mensagem;
		return true;
	}
		
	return false;
}

function Reseta() {
	//Percorre todas as divs com a classe css "box"
	Array.prototype.filter.call(document.getElementsByClassName('box'), function(divs){
		divs.className = 'box';
	});
	
	blocos = 0;
	
	setTimeout(function () {
		document.getElementsByClassName('titulo')[0].innerHTML = 'Jogo da Velha';
	}, 3000);
}

function Ajusta() {
	var divJogo = document.getElementsByClassName('jogo')[0];
	document.body.style.margin = "-25px 0px 0px 0px";
	divJogo.style.height = (divJogo.offsetWidth - 50) + 'px';
	divJogo.style.marginLeft = -(divJogo.offsetWidth / 2) + 'px';
	document.getElementsByClassName('titulo')[0].style.marginLeft = -(divJogo.offsetWidth / 2) + 'px';
}