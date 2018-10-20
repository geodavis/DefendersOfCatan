GameStates.PlayerPlacePurchase = function (game) {

};

GameStates.PlayerPlacePurchase.prototype = {
    create: function () {
        textPhase.text = 'Phase: Place Purchase';
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

GameStates.PlayerPlacePurchase.prototype.placeCard = function (enemy, tile) {

    enemy.setAngle(tile.type);
    enemy.currentHexName = tile.name;
    tile.addChild(enemy);

}