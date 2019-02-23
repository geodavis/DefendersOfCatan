PlayerAttackPlaceable = function (game, x, y) {
    Phaser.Sprite.call(this, game, x, y, 'playerattack');
    this.name = "placeable";
    this.inputEnabled = true;
    this.anchor.setTo(0.5, 0.5);
    this.events.onInputUp.add(this.onTap, this);
    animateCards(this); // move up and down along y-axis
};

PlayerAttackPlaceable.prototype = Object.create(Phaser.Sprite.prototype);
PlayerAttackPlaceable.prototype.constructor = PlayerAttackPlaceable;

PlayerAttackPlaceable.prototype.onTap = function () {
    var clickedPlaceableTransfer = { "parentTileId": this.parent.id };
    postJSON('/Game/AttackEnemy', "{data:" + JSON.stringify(clickedPlaceableTransfer) + "}", this.executePostPlaceableClickEvents, error);
}
       
PlayerAttackPlaceable.prototype.removePlaceablesFromTiles = function () {
    // Remove placement image from tiles
    $.each(hexGrid.children, function () { // loop each tile
        var gridTile = this;
        $.each(gridTile.children, function () { // loop each child of tile
            if (this.name == 'attackplaceable') {
                gridTile.removeChild(this);
            }
        });
    });
}

PlayerAttackPlaceable.prototype.executePostPlaceableClickEvents = function (d) {
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        GameStates.PlayerResourceOrFight.prototype.removeEnemy(d.Item.EnemyTile.Id);
        var player = players.getPlayerById(d.Item.OverrunPlayerId);
        player.setPlayerOverrun(false);

        removePlaceables();

        getJSONSync('/Game/MoveToNextPlayer', moveToNextPlayer, error); // URL, Success Function, Error Function
        getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
    }
}