//  Here is a custom game object
Players = function (game) {
    this.playersList = ko.observableArray();
};

Players.prototype.constructor = Players;

Players.prototype.addPlayer = function (playerData) {
    var hexTileStart = hexGrid.getByName("tile3_3");
    var player = new Player(game, 0, 0, playerData);
    //player.currentHexName = "tile3_3";
    this.playersList.push(player);
    hexTileStart.addChild(player);
}

Players.prototype.getPlayerById = function (id) {
    var player;
    $.each(this.playersList(), function () {
        if (this.id == id) {
            player = this;
        }
    });

    return player;
};

Players.prototype.getPlayerBasedOnColor = function (playerColor) {
    var player;
    $.each(this.playersList(), function () {
        if (this.playerColor == playerColor) {
            player = this;
        }
    });

    return player;
}

//Players.prototype.setPlayerOverrun = function () {
//    var dict = {};
//    dict[playerColors.red] = 0;
//    dict[playerColors.green] = 0;
//    dict[playerColors.blue] = 0;
//    dict[playerColors.yellow] = 0;

//    $.each(hexGrid.children, function () { // loop each tile
//        if (this.isEnemyTile() && this.hasEnemyCard()) {
//            dict[this.type] += 1;
//        }
//    });

//    // First reset all the values
//    $.each(players.playersList(), function () {
//        this.isOverrun = false;
//    });

//    // Next, set all the true values
//    $.each(dict, function (key, value) {
//        $.each(players.playersList(), function () {
//            if (this.playerColor == key && value == 3) {
//                this.isOverrun = true;
//                updateLogText(this.name + ' has been overrun!');
//            }
//        });
//    });
//}