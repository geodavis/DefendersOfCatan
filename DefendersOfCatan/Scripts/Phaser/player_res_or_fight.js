GameStates.PlayerResourceOrFight = function (game) {

};

GameStates.PlayerResourceOrFight.prototype = {
    create: function () {
        getJSONSync('/Game/GetResourceOrFightTiles', this.highlightResourceOrFightTiles, error); // URL, Success Function, Error Function

        textPhase.text = 'Phase: Resource or Fight';
    },

    update: function () {



    },

    render: function () {

    }
};

GameStates.PlayerResourceOrFight.prototype.highlightResourceOrFightTiles = function (d) {
    if (!d.HasError) {
        $.each(d.Item.ResourceTiles, function () {
            var tile = HexTile.prototype.getTileById(this);
            var placeable = new ResourcePlaceable(game, 0, 0, tile.resource);
            tile.addChild(placeable);
        });

        $.each(d.Item.FightTiles, function () {
            var tile = HexTile.prototype.getTileById(this);
            var placeable = new PlayerAttackPlaceable(game, 0, 0);
            tile.addChild(placeable);
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


