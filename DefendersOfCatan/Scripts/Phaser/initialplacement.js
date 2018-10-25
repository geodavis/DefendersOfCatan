GameStates.InitialPlacement = function (game) {  

};

GameStates.InitialPlacement.prototype = {
    create: function () {
        textPhase.text = 'Phase: Initial Placement';    

    },

    update: function () {



    },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    }
};

GameStates.InitialPlacement.prototype.placeInitialSettlement = function (tileId) {
    var tile = HexTile.prototype.getTileById(tileId);
    var development = new Development(game, 0, 0, 0); // always a settlement for initial placement
    tile.addChild(development);
}