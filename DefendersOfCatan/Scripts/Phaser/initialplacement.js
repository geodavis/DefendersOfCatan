GameStates.InitialPlacement = function (game) {  

};

GameStates.InitialPlacement.prototype = {
    create: function () {
        textPhase.text = 'Phase: Initial Placement';

        // Populate all settlement placeables
        getJSONSync('/Game/GetInitialSettlementPlacement', this.highlightInitialPlacement, error); // URL, Success Function, Error Function
    },

    update: function () {



    },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    }
};

GameStates.InitialPlacement.prototype.highlightInitialPlacement = function (d) {
    var tilesCanPlace = d.Item;
    $.each(tilesCanPlace, function () {
        var tile = HexTile.prototype.getTileById(this.Id);
        var placeable = new Placeable(game, 0, 0, 1, 0, 0.5, 0, 0, 1);
        tile.addChild(placeable);
    });
}