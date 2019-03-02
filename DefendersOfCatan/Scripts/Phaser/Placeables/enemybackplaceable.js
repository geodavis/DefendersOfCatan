EnemyBackPlaceable = function (game, x, y) {
    Phaser.Sprite.call(this, game, x, y, 'enemyback');
    this.name = "cardplaceable";
    this.inputEnabled = true;
    this.anchor.setTo(0.5, 0.5);
    this.events.onInputUp.add(this.onTap, this);
    animateCards(this); // move up and down along y-axis
};

EnemyBackPlaceable.prototype = Object.create(Phaser.Sprite.prototype);
EnemyBackPlaceable.prototype.constructor = EnemyBackPlaceable;

EnemyBackPlaceable.prototype.onTap = function () {
    var clickedPlaceableTransfer = { "parentTileId": this.parent.id };
    postJSON('/Game/PushBarbariansBack', "{data:" + JSON.stringify(clickedPlaceableTransfer) + "}", this.executePostPlaceableClickEvents, error);
}
       
EnemyBackPlaceable.prototype.executePostPlaceableClickEvents = function (d) {
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        // Loop barbarian tiles
        $.each(d.Item.BarbarianTiles, function () {
            var tile = HexTile.prototype.getTileById(this.Id);
            var barbarianIndex = this.Enemy.BarbarianIndex;
            $.each(tile.children, function () { // loop each child on tile
                if (this.name == 'enemycard') {
                    switch (barbarianIndex) {
                        case 0:
                            this.loadTexture('enemycardB0', 0, false);
                            break;
                        case 1:
                            this.loadTexture('enemycardB1', 0, false);
                            break;
                        default:
                            alert('Barbarian index out of range!');
                    }
                }
            });
        });

        removeCardPlaceables();
    }
}