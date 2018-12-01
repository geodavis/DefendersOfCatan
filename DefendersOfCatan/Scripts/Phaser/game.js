GameStates.Game = function (game) {

};

var textPhase;
var hexGrid;
var players;
var items;
var tiles = [];
var hexOverrunData;
var hexTileHeight = 64 * 1.5;
var hexTileWidth = 55 * 1.5;
var currentPlayer;
var playerMoved = false;
var tileTypes = Object.freeze({ "redenemy": 0, "blueenemy": 1, "yellowenemy": 2, "greenenemy": 3, "resource": 4, "capital": 5, "unused": 6, "hidden": 7  });
var playerColors = Object.freeze({"red": 0, "blue": 1, "yellow": 2, "green": 3});
var ResourcesEnum = Object.freeze({ "brick": 0, "ore": 1, "wood": 2, "grain": 3, "wool": 4 }); //https://stackoverflow.com/questions/287903/what-is-the-preferred-syntax-for-defining-enums-in-javascript
var ItemsEnum = Object.freeze({ "Item1": 0, "Item2": 1, "Item3": 2, "Item4": 3, "Item5": 4 });
var enemyCards;
var selectedEnemyCard;
var cardSelected = false;

const numEnemyCards = 40;

var fragmentSrc = [
    "precision mediump float;",

    "uniform float     time;",
    "uniform vec2      resolution;",
    "uniform vec2      mouse;",

    "// Yuldashev Mahmud Effect took from shaderToy mahmud9935@gmail.com",

    "float snoise(vec3 uv, float res)",
    "{",
        "const vec3 s = vec3(1e0, 1e2, 1e3);",

        "uv *= res;",

        "vec3 uv0 = floor(mod(uv, res))*s;",
        "vec3 uv1 = floor(mod(uv+vec3(1.), res))*s;",

        "vec3 f = fract(uv); f = f*f*(3.0-2.0*f);",

        "vec4 v = vec4(uv0.x+uv0.y+uv0.z, uv1.x+uv0.y+uv0.z,",
        "uv0.x+uv1.y+uv0.z, uv1.x+uv1.y+uv0.z);",

        "vec4 r = fract(sin(v*1e-1)*1e3);",
        "float r0 = mix(mix(r.x, r.y, f.x), mix(r.z, r.w, f.x), f.y);",

        "r = fract(sin((v + uv1.z - uv0.z)*1e-1)*1e3);",
        "float r1 = mix(mix(r.x, r.y, f.x), mix(r.z, r.w, f.x), f.y);",

        "return mix(r0, r1, f.z)*2.-1.;",
    "}",

    "void main( void ) {",

        "vec2 p = -.5 + gl_FragCoord.xy / resolution.xy;",
        "p.x *= resolution.x/resolution.y;",

        "float color = 3.0 - (3.*length(2.*p));",

        "vec3 coord = vec3(atan(p.x,p.y)/6.2832+.5, length(p)*.4, .5);",

        "for(int i = 1; i <= 7; i++)",
        "{",
            "float power = pow(2.0, float(i));",
            "color += (1.5 / power) * snoise(coord + vec3(0.,-time*.05, time*.01), power*16.);",
        "}",

        "gl_FragColor = vec4( color, pow(max(color,0.),2.)*0.4, pow(max(color,0.),3.)*0.15 , 1.0);",

    "}"
];

//var filter = new Phaser.Filter(game, null, fragmentSrc);
//filter.setResolution(800, 600);

GameStates.Game.prototype = {

    create: function () {
        game.stage.backgroundColor = "#ffffff"
        
        var style = { font: "bold 32px Arial", fill: "#fff", boundsAlignH: "center", boundsAlignV: "middle" };
        textPhase = game.add.text(16, 16, 'Phase:', { fontSize: '32px', fill: '#FF9E2C' });
        //vm = new AppViewModel();
        
        //vm.updateFirstName("TEST");
        getBoardData();
        getDevelopments();
        getPlayers();
        getEnemies();
        // ToDo: Make this a common function and call the common in all places
        getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
        //this.state.start('EnemyMove', false, false);

        //vm.updatePlayerResources(player.resources);
    },

    update: function () {

        //alert('GameLoop');


        },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    },
};

//function markHoveredTile() {
//    var tilePos = findHexTile();
//    var tile = hexGrid.getByName("tile" + tilePos.x + "_" + tilePos.y);
//    if (tile != null) {
//        tile.alpha = 0.5;
//    }
//}

function startNextGameState(d) {
    if (d.HasError) {
        alert('Error occurred during game state transition: ' + d.Error);
    }
    else {
        var gameState = d.Item;
        this.game.state.start(gameState, false, false);
    }
}

function getBoardData() {
    // Get resource tile types from the server
    getJSONSync('/Game/GetBoardData', initializeBoard, error); // URL, Success Function, Error Function
}

function getDevelopments() {
    getJSONSync('/Game/GetDevelopments', initializeItems, error); // URL, Success Function, Error Function
}

function getPlayers() {
    // Get players from the server
    getJSONSync('/Game/GetPlayers', initializePlayers, error); // URL, Success Function, Error Function
}

function getEnemies() {
    // Get players from the server
    getJSONSync('/Game/GetEnemies', initializeEnemyCards, error); // URL, Success Function, Error Function
}

function initializeBoard(d) {
    if (d.HasError) {
        alert('Error occurred during board initialization: ' + d.Error);
    }
    else {
        while (d.Item.length) tiles.push(d.Item.splice(0, 8)); // convert to 2-d as serialization converted to 1-d

        hexGrid = game.add.group();
        hexGrid.inputEnableChildren = true;
        //hexGrid.onChildInputOver.add(markHoveredTile, this);
        var hexCount = 0;

        //game.input.onTap.add(onTap);
        //var style = { font: "32px Arial", fill: "#ff0044", wordWrap: true, wordWrapWidth: hexagonWidth, align: "center", backgroundColor: "#ffff00" };

        //tileLayout =
        //    [
        //    [9, 9, 0, 0, 1, 1, 9, 9],
        //    [9, 0, 7, 7, 7, 1, 9, 9],
        //    [9, 8, 7, 7, 7, 7, 8, 9],
        //    [8, 7, 7, 6, 7, 7, 8, 9],
        //    [9, 8, 7, 7, 7, 7, 8, 9],
        //    [9, 3, 7, 7, 7, 2, 9, 9],
        //    [9, 9, 3, 3, 2, 2, 9, 9]
        //    ];

        // Enemy hexes go 35, 36, 37; 46, 47, 48; 53, 54, 55; 64, 65, 66 (e.g. 35 goes from tile type 3 -> 5, then increment the next)
        hexOverrunData =
        [
        ['0', '0', '36', '37', '46', '47', '0', '0'],
        ['0', '35', '36,54', '37,46,53,66', '47,65', '48', '0', '0'],
        ['0', '7', '35,55', '36,46,54,66', '37,47,53,65', '48,64', '7', '0'],
        ['7', '1', '35,46,55,66', '36,47,54,65', '37,48,53,64', '1', '7', '0'],
        ['0', '7', '46,66', '35,47,55,65', '36,48,54,64', '37,53', '7', '0'],
        ['0', '66', '47,65', '35,48,55,64', '36,54', '53', '0', '0'],
        ['0', '0', '65', '64', '55', '54', '0', '0']
        ];

        //tileLayout = transpose(tileLayout); - for vertical view

        var verticalOffset = hexTileHeight * 3 / 4;
        var horizontalOffset = hexTileWidth;
        var startX;
        var startY;
        var startXInit = hexTileWidth / 2;
        var startYInit = hexTileHeight / 2;

        for (var i = 0; i < tiles.length; i++) {
            if (i % 2 !== 0) {
                startX = 2 * startXInit;
            } else {
                startX = startXInit;
            }
            startY = startYInit + (i * verticalOffset);
            for (var j = 0; j < tiles[0].length; j++) {
                if (tiles[i][j] != tileTypes.hidden) {
                    var tileId = tiles[i][j].Id;
                    var tileType = tiles[i][j].Type;
                    var resourceType = tiles[i][j].ResourceType;
                    var hexTile = new HexTile(game, startX, startY, getTileImage(tileType, resourceType), false, i, j, tileType, tileId);
                    hexGrid.add(hexTile);
                }

                startX += horizontalOffset;
            }
        }
        hexGrid.x = 50;
        hexGrid.y = 50;

        var element = document.getElementById('sidebar-right');
        ko.applyBindings(game_vm, element);
    }
}

function initializeItems(d) {
    developments = new developments_vm(game, d.Item);
    var element = document.getElementById('developments');
    ko.applyBindings(developments, element);

}

function initializePlayers(d) {
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        players = new Players(game);
        $.each(d.Item, function (index, value) {
            players.addPlayer(this);
        });
        currentPlayer = players.playersList()[0];
        var element = document.getElementById('players');
        ko.applyBindings(players, element);
    }
};

function initializeEnemyCards(d) {
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        var enemies = d.Item;
        enemyCards = game.add.group();
        for (i = 0; i < enemies.length; i++) {
            var image = 'enemycard';
            if (enemies[i].HasBarbarian) {
                image = 'enemycardB0';
            }

            var enemy = new Enemy(game, 0, 0, image, enemies[i]);
            enemyCards.add(enemy);
        }

        enemyCards.x = 500;
        enemyCards.y = 50;
    }
}

function findHexTile() {
    var pos = game.input.activePointer.position;
    pos.x -= hexGrid.x;
    pos.y -= hexGrid.y;
    var xVal = Math.floor((pos.x) / hexTileWidth);
    var yVal = Math.floor((pos.y) / (hexTileHeight * 3 / 4));
    var dX = (pos.x) % hexTileWidth;
    var dY = (pos.y) % (hexTileHeight * 3 / 4);
    var slope = (hexTileHeight / 4) / (hexTileWidth / 2);
    var caldY = dX * slope;
    var delta = hexTileHeight / 4 - caldY;

    if (yVal % 2 === 0) {
        //correction needs to happen in triangular portions & the offset rows
        if (Math.abs(delta) > dY) {
            if (delta > 0) {//odd row bottom right half
                xVal--;
                yVal--;
            } else {//odd row bottom left half
                yVal--;
            }
        }
    } else {
        if (dX > hexTileWidth / 2) {// available values don't work for even row bottom right half
            if (dY < ((hexTileHeight / 2) - caldY)) {//even row bottom right half
                yVal--;
            }
        } else {
            if (dY > caldY) {//odd row top right & mid right halves
                xVal--;
            } else {//even row bottom left half
                yVal--;
            }
        }
    }
    pos.x = yVal;
    pos.y = xVal;
    return pos;
}

function checkforBoundary(i, j) {//check if the tile is outside level data array
    if (i < 0 || j < 0 || i > tiles.length - 1 || j > tiles[0].length - 1) {
        return true;
    }
    return false;
}

function getNeighbors(i, j) {
    //first add common elements for odd & even rows
    var tempArray = [];
    var newi = i - 1;//tr even tl odd
    var newj = j;
    populateNeighbor(newi, newj, tempArray);
    newi = i;
    newj = j - 1;//l even odd
    populateNeighbor(newi, newj, tempArray);
    newi = i + 1;
    newj = j;//br even bl odd
    populateNeighbor(newi, newj, tempArray);
    newi = i;//r even odd
    newj = j + 1;
    populateNeighbor(newi, newj, tempArray);
    //now add the different neighbours for odd & even rows
    if (i % 2 == 0) {
        newi = i - 1;
        newj = j - 1;//tl even
        populateNeighbor(newi, newj, tempArray);
        newi = i + 1;//bl even 
        populateNeighbor(newi, newj, tempArray);
    } else {
        newi = i - 1;
        newj = j + 1;//tr odd
        populateNeighbor(newi, newj, tempArray);
        newi = i + 1;//br odd
        populateNeighbor(newi, newj, tempArray);
    }

    return tempArray;
}

function populateNeighbor(i, j, tempArray) {//check & add new neighbor
    var newPt = new Phaser.Point();
    if (!checkforBoundary(i, j)) {
        //if (!checkForOccuppancy(i, j)) {
        newPt = new Phaser.Point();
        newPt.x = i;
        newPt.y = j;
        tempArray.push(newPt);
        //}
    }
}

function isNeighbor(currentTile, toTile) {
    var neighbors = getNeighbors(currentTile.i, currentTile.j);
    var toTilePoint = new Phaser.Point(toTile.i, toTile.j);
    for (i = 0; i < neighbors.length; i++) {
        if (Phaser.Point.equals(toTilePoint, neighbors[i])) {
            return true;
        }
    }

    return false;
}

function highlightMoveableTiles() {
    // Remove all tweens
    game.tweens.removeAll();

    // Get new move to tiles and highlight
    var playerTile = hexGrid.getByName(currentPlayer.currentHexName);
    //getJSONSync('/Game/GetNeighbors?tileId=' + playerTile.id, highlightTiles, error); // URL, Success Function, Error Function ToDo: return highlighting!
    //highlight(playerTile);
    //var neighbors = getNeighbors(playerTile.i, playerTile.j); // TODO: Store neighbors on each tile upfront, so you do not have to calculate it every move

    //for (var i = 0, len = neighbors.length; i < len; i++) {
    //    var highlightTile = hexGrid.getByName("tile" + neighbors[i].x + "_" + neighbors[i].y);
    //    highlight(highlightTile);
    //}
}

function highlightTiles(d)
{
    for (var i = 0, len = d.Item.length; i < len; i++) {
        var highlightTile = HexTile.prototype.getTileById(d.Item[i]);
        highlight(highlightTile);
    }
}

function highlight(tile) {
    tile.alpha = 0.2;
    game.add.tween(tile).to({ alpha: 1 }, 1000, Phaser.Easing.Linear.None, true, 0, 1000, true);
}

function getTileImage(tileType, resourceType) {
    // Store all hexagon images in a array
    var hexImageArray = ["hexagonrednoise", "hexagongraynoise", "hexagonbrownnoise", "hexagonyellownoise", "hexagongreennoise"];
    switch (tileType) {
        case tileTypes.resource:
            return hexImageArray[resourceType];
            break;
        case tileTypes.redenemy:
            return "hexagonrednoisewaves";
            break;
        case tileTypes.yellowenemy:
            return "hexagonyellownoisewaves";
            break;
        case tileTypes.greenenemy:
            return "hexagongreennoisewaves";
            break;
        case tileTypes.blueenemy:
            return "hexagonbluenoisewaves";
            break;
        case tileTypes.capital:
            return "hexagonorange";
            break;
        case tileTypes.unused:
            return "hexagonblack";
            break;
        default:
            // do not assign a resource
    }

}

function moveToNextPlayer(d) {
    currentPlayer.isCurrentPlayer(false);
    $.each(players.playersList(), function () {
        if (this.playerColor == d.Item) {
            currentPlayer = this;
            currentPlayer.isCurrentPlayer(true);
            updateLogText(currentPlayer.name + "'s turn.");
        }
    });
}

function checkForEndGameState() {
    // Loop each tile
    $.each(hexGrid.children, function () {
        if (this.isEnemyTile()) {
            // Check each tile in path
            var tileNumber = hexOverrunData[this.i][this.j];
            var overrunTile = getNextOverrunTile(this, tileNumber, 0);


            //return false; // exit early
        }
    });

}



    /**
    * @description Will tween a display object between two hex values
    *
    * @param {Object} obj the object to tween
    * @param {number} startColour the starting colour (hex)
    * @param {number} endColour the ending colour (hex)
    * @param {number} time the duration of the tween
    * @param {function} callback a function to be called on tween completion
    *
    * @returns {void}
    */
    //tweenTint(obj, startColor, endColor, time = 250, delay = 0, callback = null) {
    //    if (obj) {
    //        let colorBlend = { step: 0 };
    //        let colorTween = this.game.add.tween(colorBlend).to({ step: 100 }, time, Phaser.Easing.Linear.None, delay);
    //        colorTween.onUpdateCallback(() => {
    //            obj.tint = Phaser.Color.interpolateColor(startColor, endColor, 100, colorBlend.step);
    //        });
    //        obj.tint = startColor;
    //        if (callback) {
    //            colorTween.onComplete.add(callback, this);
    //        }
    //        colorTween.start();
    //    }
    //}





































































    //function transpose(a) {
    //    return Object.keys(a[0]).map(
    //        function (c) { return a.map(function (r) { return r[c]; }); }
    //        );
    //}
















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
            (i == 3 && j == 0) || (i == 3 && j == 2) || (i == 3 && j == 12) || (i == 3 && j == 14)) {
            return true;
        }
        return false;
    }