EnemyRemovePlaceable = function (game, x, y) {
    Phaser.Sprite.call(this, game, x, y, 'enemyremove');
    this.name = "cardplaceable";
    this.inputEnabled = true;
    this.anchor.setTo(0.5, 0.5);
    this.events.onInputUp.add(this.onTap, this);
    animateCards(this); // move up and down along y-axis
};

EnemyRemovePlaceable.prototype = Object.create(Phaser.Sprite.prototype);
EnemyRemovePlaceable.prototype.constructor = EnemyRemovePlaceable;

EnemyRemovePlaceable.prototype.onTap = function () {
    var clickedPlaceableTransfer = { "parentTileId": this.parent.id };
    postJSON('/Game/RemoveEnemy', "{data:" + JSON.stringify(clickedPlaceableTransfer) + "}", this.executePostPlaceableClickEvents, error);
}
       
EnemyRemovePlaceable.prototype.executePostPlaceableClickEvents = function (d) {
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        GameStates.PlayerResourceOrFight.prototype.removeEnemy(d.Item.EnemyTile.Id);
        var player = players.getPlayerById(d.Item.OverrunPlayerId);
        player.setPlayerOverrun(false);
        removeCardPlaceables();
    }
}