// Declare the world generator state
GameStates.PlayerMove = function (game) {  

};

GameStates.PlayerMove.prototype = {
    create: function () {
        textPhase.text = 'Phase: Move';
        developments.purchasePhase(false);
        getJSONSync('/Game/GetCurrentPlayerNeighbors', setMovableNeighbors, error); // URL, Success Function, Error Function

        // Highlight new moveable tiles
        //highlightMoveableTiles();

        currentPlayer.setPurchasableDevelopments();
        
        // call next state
        //this.state.start('MainMenu');
    },

    update: function () {
        // Loop each tile
        //var tilePos = findHexTile();
        //if (tilePos.x > 0) // tilePos returns negative when cursor is not moving
        //{
        //    $.each(hexGrid.children, function () {
        //        if (this.name == "tile" + tilePos.x + "_" + tilePos.y) { // ToDo: get rid of name and replace with x, y
        //            this.alpha = 0.5;
        //        }
        //        else {
        //            this.alpha = 1;
        //        }
        //    });
        //}
    },
};

GameStates.PlayerMove.prototype.placePlayer = function (d) {
    var tile = HexTile.prototype.getTileById(d.Item.TileId);
    //var player = Enemy.prototype.getEnemyById(d.Item.EnemyId);

    // Add player to the clicked tile
    tile.addChild(currentPlayer);

    // Update the player current hex
    currentPlayer.currentHexName = "tile" + tile.i + "_" + tile.j;
    playerMoved = true;

    // Update state
    getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function

}

function setMovableNeighbors(d) {
    $.each(d.Item, function () {
        var tile = HexTile.prototype.getTileById(this);
        var border = game.make.sprite(0, 0, 'hexagonborder');
        border.name = "border";
        border.anchor.setTo(0.5, 0.5);
        border.scale.setTo(0.95, 0.95);
        tile.addChild(border);

        //var hexImageArray = ["hexagonrednoise", "hexagongraynoise", "hexagonbrownnoise", "hexagonyellownoise", "hexagongreennoise"];
        //tile.loadTexture(hexImageArray[tile.type]); // todo: replace tile images with movable tiles - swap tile.type to tile.resourceType
    });
}

