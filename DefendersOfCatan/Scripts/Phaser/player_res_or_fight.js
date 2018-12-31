GameStates.PlayerResourceOrFight = function (game) {

};

GameStates.PlayerResourceOrFight.prototype = {
    create: function () {
        //alert('player resource stage');

        textPhase.text = 'Phase: Resource';
    },

    update: function () {



    },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    }
};


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


