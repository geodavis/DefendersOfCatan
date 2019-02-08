GameStates.PlayerResourceOrFight = function (game) {

};

GameStates.PlayerResourceOrFight.prototype = {
    create: function () {
        getJSONSync('/Game/GetResourceOrFightTiles', this.highlightResourceOrFightTiles, error); // URL, Success Function, Error Function


        textPhase.text = 'Phase: Resource';
    },

    update: function () {



    },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    }
};

GameStates.PlayerResourceOrFight.prototype.highlightResourceOrFightTiles = function (d) {
    if (!d.HasError) {
        $.each(d.Item, function () {
            var tile = HexTile.prototype.getTileById(this);
            var border = game.make.sprite(0, 0, 'hexagonborder');
            border.name = "border";
            border.anchor.setTo(0.5, 0.5);
            border.scale.setTo(0.95, 0.95);
            tile.addChild(border);
        });
    }
    else {
        alert(d.Error);
    }
}

GameStates.PlayerResourceOrFight.prototype.addResourceToPlayer = function (resourceType) {
    currentPlayer.addResourceToPlayer(resourceType);

}

GameStates.PlayerResourceOrFight.prototype.removeEnemy = function (enemyTileId) {
    var tile = HexTile.prototype.getTileById(enemyTileId);

    $.each(tile.children, function () {
        if (this.name == 'enemycard') {
            this.destroy(); // destroy removes the object, kill() makes it hidden from view
        }
    });


}


