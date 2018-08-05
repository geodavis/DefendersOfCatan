﻿GameStates.EnemyCard = function (game) {  

};

GameStates.EnemyCard.prototype = {
    create: function () {
        textPhase.text = 'Phase: Card';
        //highlight(enemyCards.children[3]);
        $.each(enemyCards.children, function () { // ToDo: Re-evaluate enemyCards group. After moving card to hex, it leaves the group
                highlight(this);
        });

        //alert('enemy card phase');
        // call next state
        //this.state.start('PlayerMove');
    },

    update: function () {
        //filter.update();


    },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    }
};

GameStates.EnemyCard.prototype.placeCard = function (d) {
    if (!d.HasError) {
        var tile = HexTile.prototype.getTileById(d.Item.TileId);
        var enemy = Enemy.prototype.getEnemyById(d.Item.EnemyId);

        enemy.scale.setTo(1, 1);
        enemy.setAngle(tile.type);
        enemy.currentHexName = tile.name;
        tile.addChild(enemy);

        cardSelected = false;

        game.state.start('PlayerMove', false, false);
    }
    else {
        alert(d.Error);
    }
}
