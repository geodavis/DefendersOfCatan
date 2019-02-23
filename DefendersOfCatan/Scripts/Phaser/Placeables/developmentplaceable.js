//  Here is a custom game object
DevelopmentPlaceable = function (game, x, y, developmentType, angle, anchor, id1, id2, scale) {
    var developmentImage = this.getPlaceableImageBasedOnType(developmentType);
    this.developmentType = developmentType;
    Phaser.Sprite.call(this, game, x, y, developmentImage);
    this.anchor.setTo(anchor, anchor);
    this.name = "placeable";
    this.angle += angle;
    this.inputEnabled = true;
    //this.input.useHandCursor = true;
    //this.events.onInputOut.add(this.rollOut, this);
    //this.events.onInputOver.add(this.rollOver, this);
    this.events.onInputUp.add(this.onTap, this);
    this.scale.setTo(scale, scale);
    this.tile1Id = id1;
    this.tile2Id = id2;
};

DevelopmentPlaceable.prototype = Object.create(Phaser.Sprite.prototype);
DevelopmentPlaceable.prototype.constructor = DevelopmentPlaceable;

DevelopmentPlaceable.prototype.onTap = function () {
    //alert("TileId1: " + this.tile1Id + " TileId2: " + this.tile2Id + " Angle: " + this.angle);

    if (this.developmentType == 0) {
        var roadPlaceableTransfer = { "tile1Id": this.tile1Id, "tile2Id": this.tile2Id };
        postJSON('/Game/PlaceRoad', "{data:" + JSON.stringify(roadPlaceableTransfer) + "}", this.executePostPlaceableClickEvents, error);
    }
    else {
        var clickedPlaceableTransfer = { "parentTileId": this.parent.id, "developmentType": this.developmentType };
        postJSON('/Game/ExecutePlaceableClickedActions', "{data:" + JSON.stringify(clickedPlaceableTransfer) + "}", this.executePostPlaceableClickEvents, error);
    }
}

DevelopmentPlaceable.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
DevelopmentPlaceable.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

DevelopmentPlaceable.prototype.removePlaceablesFromTiles = function () {
    // Remove placement image from tiles
    $.each(hexGrid.children, function () { // loop each tile
        var gridTile = this;
        $.each(gridTile.children, function () { // loop each child of tile
            if (this.name == 'developmentplaceable') {
                gridTile.removeChild(this);
            }
        });
    });
}

DevelopmentPlaceable.prototype.getPlaceableImageBasedOnType = function (developmentType) {
    switch (developmentType) {
        case 0:
            return 'roadPlacement';
            break;
        case 1:
            return 'settlementPlacement';
            break;
        case 2:
            return 'settlementPlacement';
            break;
        case 3:
            return 'settlementPlacement';
            break;
        case 4:
            return 'settlementPlacement';
            break;
        case 5: // Tile Move
            return 'playerMove';
            break;
        default: // ToDo
            break;
    }
}

DevelopmentPlaceable.prototype.executePostPlaceableClickEvents = function (d) {

    if (d.HasError) {
        alert(d.Error);
    }
    else {
        var gameState = d.Item.GameState;
        var developmentType = d.Item.DevelopmentType;

        switch (gameState) {
            case 'InitialPlacement':
                var tile = HexTile.prototype.getTileById(d.Item.ClickedTileId);
                var development = new Development(game, 0, 0, developmentType, 0, 0.5);
                tile.addChild(development);

                if (d.Item.PlayerId == 4) {
                    DevelopmentPlaceable.prototype.removePlaceablesFromTiles();
                    getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
                    removePlaceables();
                }

                // Get the next player
                getJSONSync('/Game/MoveToNextPlayer', moveToNextPlayer, error); // URL, Success Function, Error Function


                break;
            case 'EnemyCard':
                // ToDo: if you click on the hex where another card has been placed, it does not prevent placement. Need to check if card already exists on hex.
                var enemy = Enemy.prototype.getEnemyById(d.Item.EnemyId);
                var tile = HexTile.prototype.getTileById(d.Item.ClickedTileId);
                GameStates.EnemyCard.prototype.placeCard(enemy, tile);

                break;
            case 'PlayerPurchase':
                switch (developmentType) {
                    case 0:
                        var angle = d.Item.Angle;
                        var tile1 = HexTile.prototype.getTileById(d.Item.Tile1Id);
                        var tile2 = HexTile.prototype.getTileById(d.Item.Tile2Id);
                        var anchor = 0.5;
                        var width = game.cache.getImage("roadBlue").width;
                        var height = game.cache.getImage("roadBlue").height;

                        // Road calculation below with scale 0.5
                        if (angle == 90) {
                            var placeableX = ((tile1.x + tile2.x) / 2) + height / 4;
                            var placeableY = tile1.y + tile1.height / 2;
                        }
                        else if (angle == -150) {
                            var placeableX = tile1.x + (tile1.width / 2);
                            var placeableY = tile2.y;
                        }
                        else {
                            var placeableX = tile2.x + tile2.width / 4;
                            var placeableY = tile2.y;
                        }

                        var development = new Development(game, placeableX, placeableY, developmentType, angle, anchor);
                        placedDevelopments.add(development);

                        var paths = d.Item.Paths;
                        $.each(paths, function () { // loop paths
                            $.each(this, function () { // loop tiles on path
                                var tile = HexTile.prototype.getTileById(this);
                                var border = game.make.sprite(0, 0, 'hexagonborder');
                                border.name = "border";
                                border.anchor.setTo(0.5, 0.5);
                                border.scale.setTo(0.95, 0.95);
                                tile.addChild(border);
                            });
                        });


                        // remove road placeables
                        for (var i = 0, len = placeables.children.length; i < len; i++) {
                            placeables.children[0].destroy();
                        }

                        break;
                    case 1:
                        var tile = HexTile.prototype.getTileById(d.Item.ClickedTileId);                 
                        var development = new Development(game, 0, 0, developmentType, 0, 0.5);
                        tile.addChild(development);
                        removePlaceables();


                        break;
                    case 2:

                        break;
                    case 3:

                        break;
                    case 4:

                        break;
                    case 5:
                        currentPlayer.addDevelopmentToPlayer(d.Item);
                        break;
                    default:
                        alert("Do not recogize development type!");
                }


                // Remove placement image from tiles
                //DevelopmentPlaceable.prototype.removePlaceablesFromTiles();

                break;
            case 'PlayerMove':
                var tile = HexTile.prototype.getTileById(d.Item.ClickedTileId);
                tile.addChild(currentPlayer);

                break;
            case 'PlayerResourceOrFight':
                GameStates.PlayerResourceOrFight.prototype.addResourceToPlayer(d.Item.ResourceType);
                getJSONSync('/Game/MoveToNextPlayer', moveToNextPlayer, error); // URL, Success Function, Error Function

                break;
            default:
                // ToDo:
        }

        if (gameState != 'InitialPlacement') {
            getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
        }


    }
}