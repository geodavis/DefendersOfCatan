GameStates.Game = function (game) {

};

var hexagonWidth = 70;
var hexagonHeight = 80;
var gridSizeX = 15;
var gridSizeY = 7;
var columns = [Math.ceil(gridSizeX / 2), Math.floor(gridSizeX / 2)];
var moveIndex;
var sectorWidth = hexagonWidth;
var sectorHeight = hexagonHeight / 4 * 3;
var gradient = (hexagonHeight / 4) / (hexagonWidth / 2);
var marker;
var hexagonGroup;

GameStates.Game.prototype = {

    create: function () {
        // Store all hexagon images in a array
        var hexImageArray = ["hexagongray", "hexagonbrown", "hexagonorange", "hexagonred", "hexagonyellow", "hexagongreen", "hexagonblack"];
        hexagonGroup = game.add.group();
        var hexCount = 0;
        var style = { font: "32px Arial", fill: "#ff0044", wordWrap: true, wordWrapWidth: hexagonWidth, align: "center", backgroundColor: "#ffff00" };
        
        // Create all hexagon sprites
        /*var hexagonGray = game.add.sprite(0, 0, "hexagongray");
        var hexagonBrown = game.add.sprite(0, 0, "hexagonbrown");
        var hexagonOrange = game.add.sprite(0, 0, "hexagonorange");
        var hexagonRed = game.add.sprite(0, 0, "hexagonred");
        var hexagonYellow = game.add.sprite(0, 0, "hexagonyellow");
        var hexagonGreen = game.add.sprite(0, 0, "hexagongreen");
        var hexagonBlack = game.add.sprite(0, 0, "hexagonblack");

        hexagonGray.visible = false;
        hexagonBrown.visible = false;
        hexagonOrange.visible = false;
        hexagonRed.visible = false;
        hexagonYellow.visible = false;
        hexagonGreen.visible = false;
        hexagonBlack.visible = false;

        hexagonGray.scale.setTo(.15, .15);
        hexagonBrown.scale.setTo(.15, .15);
        hexagonOrange.scale.setTo(.15, .15);
        hexagonRed.scale.setTo(.15, .15);
        hexagonYellow.scale.setTo(.15, .15);
        hexagonGreen.scale.setTo(.15, .15);
        hexagonBlack.scale.setTo(.15, .15);
                

        // Generate all hexes and add to a group
        hexagonGroup = game.add.group();
        hexagonGroup.createMultiple(15, hexagonGray);
        hexagonGroup.createMultiple(15, hexagonBrown);
        hexagonGroup.createMultiple(15, hexagonOrange);
        hexagonGroup.createMultiple(15, hexagonRed);
        hexagonGroup.createMultiple(15, hexagonYellow);
        hexagonGroup.createMultiple(15, hexagonGreen);
        hexagonGroup.createMultiple(15, hexagonBlack);*/
        // exclusion list (00, 01, 
        game.stage.backgroundColor = "#ffffff"
        for (var i = 0; i < gridSizeY / 2; i++) {
            for (var j = 0; j < gridSizeX; j++) {
                if ((gridSizeY % 2 == 0 || i + 1 < gridSizeY / 2 || j % 2 == 0) && !excludeTile(i, j)) {  
                    
                    var hexagonX = hexagonWidth * j / 2;
                    var hexagonY = hexagonHeight * i * 1.5 + (hexagonHeight / 4 * 3) * (j % 2);
                    //var hexagon = hexagonGroup.getRandom(); //do {var NAME = NAMEOFGROUP.getRandom();} while (NAME.alive === true)
                    //hexagon.visible = true;
                    var hexagon = game.add.sprite(hexagonX, hexagonY, hexImageArray[game.rnd.integerInRange(0, 5)]);
                    //hexagon.x = hexagonX;
                    //hexagon.y = hexagonY;
                    hexagon.scale.setTo(.15, .15);
                    hexagon.inputEnabled = true;
                    hexagon.events.onInputDown.add(onDown, this);
                    hexagonGroup.add(hexagon);
                }
            }
        }

        hexagonGroup.x = (game.width - hexagonWidth * Math.ceil(gridSizeX / 2)) / 2;
        if (gridSizeX % 2 == 0) {
            hexagonGroup.x -= hexagonWidth / 4;
        }

        hexagonGroup.y = (game.height - Math.ceil(gridSizeY / 2) * hexagonHeight - Math.floor(gridSizeY / 2) * hexagonHeight / 2) / 2;

        if (gridSizeY % 2 == 0) {
            hexagonGroup.y -= hexagonHeight / 8;
        }

        marker = game.add.sprite(0, 0, "marker");
        marker.anchor.setTo(0.5);
        marker.visible = false;
        hexagonGroup.add(marker);
        moveIndex = game.input.addMoveCallback(checkHex, this);

        // Tag each hex with a number
        hexagonGroup.forEach(function (item) {
            item.location = hexCount;
            hexCount++;
            var text = game.add.text(0, 0, hexCount.toString(), style);
            text.anchor.set(0.5);
            text.x = Math.floor(item.x + item.width / 2);
            text.y = Math.floor(item.y + item.height / 2);
        }, this);
    },

    update: function () {
     



        },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    },
};

function checkHex() {
    var candidateX = Math.floor((game.input.worldX - hexagonGroup.x) / sectorWidth);
    var candidateY = Math.floor((game.input.worldY - hexagonGroup.y) / sectorHeight);
    var deltaX = (game.input.worldX - hexagonGroup.x) % sectorWidth;
    var deltaY = (game.input.worldY - hexagonGroup.y) % sectorHeight;
    if (candidateY % 2 == 0) {
        if (deltaY < ((hexagonHeight / 4) - deltaX * gradient)) {
            candidateX--;
            candidateY--;
        }
        if (deltaY < ((-hexagonHeight / 4) + deltaX * gradient)) {
            candidateY--;
        }
    }
    else {
        if (deltaX >= hexagonWidth / 2) {
            if (deltaY < (hexagonHeight / 2 - deltaX * gradient)) {
                candidateY--;
            }
        }
        else {
            if (deltaY < deltaX * gradient) {
                candidateY--;
            }
            else {
                candidateX--;
            }
        }
    }
    placeMarker(candidateX, candidateY);
}

function placeMarker(posX, posY) {
    if (posX < 0 || posY < 0 || posY >= gridSizeY || posX > columns[posY % 2] - 1) {
        marker.visible = false;
    }
    else {
        marker.visible = true;
        marker.x = hexagonWidth * posX;
        marker.y = hexagonHeight / 4 * 3 * posY + hexagonHeight / 2;
        if (posY % 2 == 0) {
            marker.x += hexagonWidth / 2;
        }
        else {
            marker.x += hexagonWidth;
        }
    }
}

function onDown(sprite, pointer) {
    // do something wonderful here
    alert('clicked!');
}

function excludeTile(i, j) {
    if ((i == 0 && j == 0) || (i == 0 && j == 2) || (i == 0 && j == 12) || (i == 0 && j == 14) ||
        (i == 0 && j == 1) || (i == 0 && j == 13) || 
        (i == 1 && j == 0) || (i == 1 && j == 14) ||
        (i == 2 && j == 0) || (i == 2 && j == 14) ||
        (i == 2 && j == 1) || (i == 2 && j == 13) ||
        (i == 3 && j == 0) || (i == 3 && j == 2) || (i == 3 && j == 12) || (i == 3 && j == 14))
            {
        return true;
    }
    return false;
}
    