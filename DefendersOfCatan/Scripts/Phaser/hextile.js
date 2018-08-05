//  Here is a custom game object
HexTile = function (game, x, y, tileImage, isVertical, i, j, type, id) {
    Phaser.Sprite.call(this, game, x, y, tileImage);
    this.anchor.setTo(0.5, 0.5);
    this.tileTag = game.make.text(0, 0, type);
    //this.tileTag = game.make.text(0,0,'i'+(i)+',j'+(j));
    //this.tileTag = game.make.text(0,0,'i'+(i-6)+',j'+(j-6));

    this.tileTag.anchor.setTo(0.5, 0.5);
    this.tileTag.addColor('#ffffff', 0);
    if (isVertical) {
        this.tileTag.rotation = -Math.PI / 2;
    }
    this.addChild(this.tileTag);
    this.tileTag.visible = false;
    this.revealed = false;
    this.name = "tile" + i + "_" + j;
    this.id = id;
    this.type = type;
    this.i = i;
    this.j = j;
    this.scale.setTo(.15, .15);

    if (isVertical) {
        this.rotation = Math.PI / 2;
    }
    this.inputEnabled = true;
    //this.input.useHandCursor = true;
    this.events.onInputOut.add(this.rollOut, this);
    this.events.onInputOver.add(this.rollOver, this);
    this.events.onInputUp.add(this.onTap, this);
    this.isActive = true;
    this.resource = getResourceTypeBasedOnTileImage(tileImage);
    this.isOverrun = false;
    //this.overrunTiles = [];
    //this.originali=(i-(Math.floor(j/2)));//x = x' - floor(y/2)
    //this.originalj=j;
};

HexTile.prototype = Object.create(Phaser.Sprite.prototype);
HexTile.prototype.constructor = HexTile;

HexTile.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    this.scale.setTo(.15, .15);
}

HexTile.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    this.scale.setTo(.14, .14);
}

HexTile.prototype.reveal = function () {
    this.tileTag.visible = true;
    this.revealed = true;
    if (this.type == 10) {
        this.tint = '0xcc0000';
    } else {
        this.tint = '0x00cc00';
    }
}
HexTile.prototype.toggleMark = function () {
    if (this.marked) {
        this.marked = false;
        this.tint = '0xffffff';
    } else {
        this.marked = true;
        this.tint = '0x0000cc';
    }
}

HexTile.prototype.setOverrunTile = function () {
    // Get tile number from hex overrun 2-d array using i,j
    var tileNumber = hexOverrunData[this.i][this.j];
    var overrunTile = getNextOverrunTile(this, tileNumber, 0);
    overrunTile.loadTexture('hexagonoverrun', 0, false);
    overrunTile.isOverrun = true;
}

function getNextOverrunTile(tile, tileNumber, tileCount) {
    // Get next tile in line
    // First, get neighbor with tile number
    var neighbors = getNeighbors(tile.i, tile.j);

    for (i = 0; i < neighbors.length; i++) {
        var overrunData = hexOverrunData[neighbors[i].x][neighbors[i].y];
        var splitOverrunData = overrunData.split(",");
        var tile = hexGrid.getByName("tile" + neighbors[i].x + "_" + neighbors[i].y);

        if (($.inArray(tileNumber, splitOverrunData) != -1) && !tile.isEnemyTile()) { // if is enemy tile, do not consider that neighbor tile
            if (!tile.isOverrun) { 
                return tile;
            }
            else { // tile is overrun
                tileCount += 1;
                if (tileCount == 4 || tile.type == tileTypes.capital) {
                    alert('game over!'); // todo: second blue tile (46) flipping ends game - BUG
                }
                return getNextOverrunTile(tile, tileNumber, tileCount); // if tile is overrun, move onto the next tile in line
            }
        }
    }
}

HexTile.prototype.isEnemyTile = function () {
    if (this.type == tileTypes.redenemy || this.type == tileTypes.yellowenemy || this.type == tileTypes.blueenemy || this.type == tileTypes.greenenemy) {
        return true;
    }

    return false;
}

HexTile.prototype.hasEnemyCard = function () {
    var hasEnemyCard = false;
    $.each(this.children, function () { // loop each child of tile
        if (this.name == 'enemycard') {
            hasEnemyCard = true;
            return false; // exit early
        }
    });

    return hasEnemyCard;
}

function getResourceTypeBasedOnTileImage(tileImage) {
    switch(tileImage) {
        case "hexagonrednoise":
            return ResourcesEnum.brick;
            break;
        case "hexagonyellownoise":
            return ResourcesEnum.grain;
            break;
        case "hexagonbrownnoise":
            return ResourcesEnum.wood;
            break;
        case "hexagongraynoise":
            return ResourcesEnum.ore;
            break;
        case "hexagongreennoise":
            return ResourcesEnum.wool;
            break;
        default:
            // do not assign a resource
    }
}

HexTile.prototype.onTap = function () {
    console.log(this.id);
    // Get clicked tile
    var tile = findHexTile();
    var clickedTile = hexGrid.getByName("tile" + tile.x + "_" + tile.y);
    //alert("i:" + tile.x + ", " + "j:" + tile.y);

    switch (game.state.getCurrentState().key) {
        case 'EnemyCard':
            // ToDo: if you click on the hex where another card has been placed, it does not prevent placement. Need to check if card already exists on hex.
            
            if (cardSelected == true) {
                // Send clicked tile id and enemy card id to the server
                var enemyTileTransfer = { "tileId": clickedTile.id, "enemyId": selectedEnemyCard.id };
                postJSON('/Game/AddEnemyToTile', "{data:" + JSON.stringify(enemyTileTransfer) + "}", GameStates.EnemyCard.prototype.placeCard, error);

                //// Check if selected card can be placed on it's color, if not allow it to be placed anywhere
                //var isEnemyCardColorOverrun = players.getPlayerBasedOnColor(selectedEnemyCard.playerColor).isOverrun;

                //// If the card has been selected, force it to be placed on it's color. Use flag set above to bypass this
                //if ((clickedTile.type == selectedEnemyCard.playerColor || isEnemyCardColorOverrun)) {
                //    //selectedEnemyCard.scale.setTo(1, 1);
                //    //selectedEnemyCard.setAngle(clickedTile.type);
                //    //clickedTile.addChild(selectedEnemyCard);
                //    cardSelected = false;
                //    //selectedEnemyCard.setHasBeenPlaced(true);
                //    // ToDo: Need to incorporate this code on the server to set is overrun state, then remove it from here 
                //    var playerPlaced = players.getPlayerBasedOnColor(clickedTile.type);
                //    var count = 0;
                //    $.each(hexGrid.children, function () { // loop each tile
                //        if (this.isEnemyTile() && this.hasEnemyCard() && this.type == playerPlaced.playerColor) {
                //            count += 1;
                //        }
                //    });

                //    if (count == 3) {
                //        playerPlaced.setPlayerOverrun(true);

                //        // Pass data to server
                //        var playerOverrunTransfer = { "id": playerPlaced.id, "isOverrun": playerPlaced.isOverrun};
                //        postJSON('/Game/SetPlayerOverrun', "{data:" + JSON.stringify(playerOverrunTransfer) + "}", success, error);
                //    }
                    
                //    // Update the player current hex ToDo: Make this into a function
                //    selectedEnemyCard.currentHexName = "tile" + clickedTile.i + "_" + clickedTile.j;

                //    // Pass data to server
                //    var enemy = { "id": selectedEnemyCard.id, "hasBeenPlaced": selectedEnemyCard.hasBeenPlaced, "currentHexName": selectedEnemyCard.currentHexName, "barbarianIndex": selectedEnemyCard.barbarianIndex };
                //    postJSON('/Game/UpdateEnemy', "{data:" + JSON.stringify(enemy) + "}", success, error);

                    // Advance state
                //    game.state.start('PlayerMove', false, false);
                }
                //else {
                //    alert('Cannot place card on that color.');
                //}

            break;
        case 'PlayerMove':
            if (!checkforBoundary(tile.x, tile.y) && clickedTile != null) {

                // Get player tile
                var playerTile = hexGrid.getByName(currentPlayer.currentHexName);

                // Move player to clicked tile if neighbor TODO: Move this logic to server
                if (isNeighbor(playerTile, clickedTile) || clickedTile === playerTile) {
                    var neighborsPrevious = getNeighbors(playerTile.i, playerTile.j);

                    // Set alphas back to 1 for previous tiles
                    playerTile.alpha = 1;
                    for (var i = 0, len = neighborsPrevious.length; i < len; i++) {
                        var highlightTile = hexGrid.getByName("tile" + neighborsPrevious[i].x + "_" + neighborsPrevious[i].y);
                        highlightTile.alpha = 1;
                    }

                    // Send new player tile to server
                    var playerTileTransfer = { "tileId": clickedTile.id, "playerId": currentPlayer.id };
                    postJSON('/Game/MovePlayerToTile', "{data:" + JSON.stringify(playerTileTransfer) + "}", GameStates.PlayerMove.prototype.placePlayer, error);


                    //// Add player to the clicked tile
                    //clickedTile.addChild(currentPlayer);

                    //// Update the player current hex
                    //currentPlayer.currentHexName = "tile" + clickedTile.i + "_" + clickedTile.j;
                    //playerMoved = true;

                    //// Update state
                    //game.state.start('PlayerResourceOrFight', false, false);
                }
            }
            break;
        case 'PlayerResourceOrFight':
            // Send player resource to server
            var playerResourceTransfer = { "playerId": currentPlayer.id, "resourceType": clickedTile.resource};
            postJSON('/Game/AddResourceToPlayer', "{data:" + JSON.stringify(playerResourceTransfer) + "}", GameStates.PlayerResourceOrFight.prototype.addResourceToPlayer, error);
                        
            break;
        default:
            // ToDo:
    }







            //playerTile.children.remove(player);

            //    if (checkForOccuppancy(tile.x, tile.y)) {
            //        if (tileLayout[tile.x][tile.y] == 10) {
            //            console.log('boom');
            //            var hexTile = hexGrid.getByName("tile" + tile.x + "_" + tile.y);
            //            if (!hexTile.revealed) {
            //                hexTile.reveal();
            //                //game over
            //            }
            //        }
            //    } else {
            //        var hexTile = hexGrid.getByName("tile" + tile.x + "_" + tile.y);

            //        if (!hexTile.revealed) {
            //            if (tileLayout[tile.x][tile.y] == 0) {
            //                console.log('recursive reveal');
            //                recursiveReveal(tile.x, tile.y);
            //            } else {
            //                //console.log('reveal');
            //                hexTile.reveal();
            //                revealedTiles++;
            //            }

            //        }
            //    }
        
        //infoTxt.text = 'found ' + revealedTiles + ' of ' + blankTiles;
    
}

HexTile.prototype.getTileById = function (id) {
    var tile;
    $.each(hexGrid.children, function () {
        if (this.id == id) {
            tile = this;
        }
    });

    return tile;
}