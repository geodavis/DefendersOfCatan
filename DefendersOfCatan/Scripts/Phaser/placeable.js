//  Here is a custom game object
Placeable = function (game, x, y, developmentType, angle, anchor) {
    var developmentImage = this.getPlaceableImageBasedOnType(developmentType);
    Phaser.Sprite.call(this, game, x, y, developmentImage);
    //this.playerColor = enemy.PlayerColor; // assign each card a player color
    this.anchor.setTo(anchor, anchor);
    this.name = "placeable";
    this.angle += angle;
    //this.hasBeenPlaced = false;
    this.inputEnabled = true;
    //this.input.useHandCursor = true;
    //this.events.onInputOut.add(this.rollOut, this);
    //this.events.onInputOver.add(this.rollOver, this);
    this.events.onInputUp.add(this.onTap, this);
    this.scale.setTo(.50, .50);
    //this.bringToTop();
};

Placeable.prototype = Object.create(Phaser.Sprite.prototype);
Placeable.prototype.constructor = Placeable;

Placeable.prototype.onTap = function () {
    alert("TileId: " + this.parent.id + " " + "Angle: " + this.angle);
    //alert("Placeable z: " + placeables.z);
    //alert("HexGrid z: " + hexGrid.z);
    var clickedPlaceableTransfer = { "clickedPlaceableParentTileId": this.parent.id, "angle": this.angle };

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