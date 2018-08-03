// Declare the world generator state
GameStates.PlayerMove = function (game) {  

};

GameStates.PlayerMove.prototype = {
    create: function () {
        textPhase.text = 'Phase: Move';

        // Highlight new moveable tiles
        highlightMoveableTiles();

        currentPlayer.setPurchasableItems();
        
        // call next state
        //this.state.start('MainMenu');
    }
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
    game.state.start('PlayerResourceOrFight', false, false);

}

