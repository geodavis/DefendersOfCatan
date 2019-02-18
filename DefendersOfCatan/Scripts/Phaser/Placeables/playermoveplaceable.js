PlayerMovePlaceable = function (game, x, y) {
    Phaser.Sprite.call(this, game, x, y, 'playermove');
    this.name = "playermoveplaceable";
    this.inputEnabled = true;
    this.anchor.setTo(0.5, 0.5);
    this.events.onInputUp.add(this.onTap, this);
    highlight(this); // faded in and out
};

PlayerMovePlaceable.prototype = Object.create(Phaser.Sprite.prototype);
PlayerMovePlaceable.prototype.constructor = PlayerMovePlaceable;

PlayerMovePlaceable.prototype.onTap = function () {
    var clickedPlaceableTransfer = { "parentTileId": this.parent.id };
    postJSON('/Game/MovePlayer', "{data:" + JSON.stringify(clickedPlaceableTransfer) + "}", this.executePostPlaceableClickEvents, error);

}

PlayerMovePlaceable.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
PlayerMovePlaceable.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

PlayerMovePlaceable.prototype.removePlaceablesFromTiles = function () {
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

PlayerMovePlaceable.prototype.executePostPlaceableClickEvents = function (d) {
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        var tile = HexTile.prototype.getTileById(d.Item.ClickedTileId);
        tile.addChild(currentPlayer);
        $.each(hexGrid.children, function () {
            var tile = this;
            $.each(tile.children, function () { // loop each child of tile
                if (this.name == 'playermoveplaceable') {
                    tile.removeChild(this);
                }
            });
        });

        getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function

    }
}