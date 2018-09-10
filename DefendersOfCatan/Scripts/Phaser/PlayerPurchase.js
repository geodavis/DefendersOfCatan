GameStates.PlayerPurchase = function (game) {

};

GameStates.PlayerPurchase.prototype = {
    create: function () {
        textPhase.text = 'Phase: Purchase';
        developments.purchasePhase(true);
    },

    update: function () {
        //filter.update();


    },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    }
};

GameStates.PlayerPurchase.prototype.placeCard = function (enemy, tile) {

    enemy.setAngle(tile.type);
    enemy.currentHexName = tile.name;
    tile.addChild(enemy);

}