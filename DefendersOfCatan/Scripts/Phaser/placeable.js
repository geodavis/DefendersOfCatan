//  Here is a custom game object
Placeable = function (game, x, y, developmentType, angle, anchor, id1, id2, scale) {
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

Placeable.prototype = Object.create(Phaser.Sprite.prototype);
Placeable.prototype.constructor = Placeable;

Placeable.prototype.onTap = function () {
    //alert("TileId1: " + this.tile1Id + " TileId2: " + this.tile2Id + " Angle: " + this.angle);

    var clickedPlaceableTransfer = { "tile1Id": this.tile1Id, "tile2Id": this.tile2Id, "parentTile": this.parent.id };
    postJSON('/Game/ExecutePlaceableClickedActions', "{data:" + JSON.stringify(clickedPlaceableTransfer) + "}", this.executePostPlaceableClickEvents, error);
}

Placeable.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
Placeable.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

Placeable.prototype.getPlaceableImageBasedOnType = function (developmentType) {
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
        default: // ToDo
            break;
    }
}

Placeable.prototype.executePostPlaceableClickEvents = function (d) {

    if (d.HasError) {
        alert(d.Error);
    }
    else {
        var gameState = d.Item.GameState;
        var angle = d.Item.Angle;
        var tile1 = HexTile.prototype.getTileById(d.Item.RoadTile1Id);
        var tile2 = HexTile.prototype.getTileById(d.Item.RoadTile2Id);
        var developmentType = d.Item.DevelopmentType;
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
        else
        {
            var placeableX = tile2.x + tile2.width / 4;
            var placeableY = tile2.y;
        }

        var development = new Development(game, placeableX, placeableY, developmentType, angle, anchor); // ToDo: try adding all road placeables upfront, then making them visible only when necessary OR make game layer groups
        placeables.add(development); // attempt to see if this brings to the top


        switch (gameState) {
            case 'InitialPlacement':
                GameStates.InitialPlacement.prototype.placeInitialSettlement(d.Item.ClickedTileId);

                if (d.Item.PlayerId == 4) {
                    getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
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
                var tile = HexTile.prototype.getTileById(d.Item.ClickedTileId);
                var developmentType = d.Item.DevelopmentType;
                var development = new Development(game, 0, 0, developmentType);
                tile.addChild(development);

                // Remove placement image from tiles
                $.each(hexGrid.children, function () { // loop each tile
                    var gridTile = this;
                    $.each(gridTile.children, function () { // loop each child of tile
                        if (this.name == 'placeable') {
                            gridTile.removeChild(this);
                        }
                    });
                });
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