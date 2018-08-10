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


GameStates.PlayerResourceOrFight.prototype.addResourceToPlayer = function (d) {
    if (!d.HasError) {
        // Update player resource list to add the new resource
        currentPlayer.addResourceToPlayer(d.Item.ResourceType);

        // Advance to the next player and phase
        getJSONWithoutDataSync('/Game/MoveToNextPlayer', moveToNextPlayer, error); // URL, Success Function, Error Function
        //game.state.start('EnemyMove', false, false);
    }
    else {
        alert(d.Error);
    }
}

GameStates.PlayerResourceOrFight.prototype.removeEnemy = function (d) {
    if (!d.HasError) {
        //var enemy = Enemy.prototype.getEnemyById(d.Item.EnemyId);
        var tile = HexTile.prototype.getTileById(d.Item.TileId);

        $.each(tile.children, function () {
            if (this.name == 'enemycard') {
                this.destroy(); // destroy removes the object, kill() makes it hidden from view
            }
        });

        // TODo - set player overrun
        //var player = players.getPlayerBasedOnColor(enemyCard.playerColor);
        //if (player.isOverrun) {
        //    player.setPlayerOverrun(false);
        //}

        // Advance to the next player and phase

        getJSONWithoutDataSync('/Game/MoveToNextPlayer', moveToNextPlayer, error); // URL, Success Function, Error Function
        //game.state.start('EnemyMove', false, false);
    }
    else {
        alert(d.Error);
    }
}


