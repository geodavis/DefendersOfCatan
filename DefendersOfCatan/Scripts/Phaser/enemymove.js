﻿GameStates.EnemyMove = function (game) {  

};

GameStates.EnemyMove.prototype = {
    create: function () {
        textPhase.text = 'Phase: Enemy Move';

        // Roll dice to see which players barbarians should advance
        //var playerColor = game.rnd.integerInRange(3, 6)
        
        // Execute Enemy Move phase
        getJSONSync('/Game/ExecuteEnemyMovePhase', GameStates.EnemyMove.prototype.updateEnemyMovePhase, error); // URL, Success Function, Error Function
        

    },

    update: function () {



    },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    }
};

GameStates.EnemyMove.prototype.updateEnemyMovePhase = function (d) {
    // Advance barbarians - overrun tiles or remove developments as necessary
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        // Loop barbarian tiles
        $.each(d.Item.BarbarianTiles, function () {
            var tile = HexTile.prototype.getTileById(this.Id);
            var barbarianIndex = this.Enemy.BarbarianIndex;
            $.each(tile.children, function () { // loop each child on tile
                if (this.name == 'enemycard') {
                    switch (barbarianIndex) {
                        case 0:
                            this.loadTexture('enemycardB0', 0, false);
                            break;
                        case 1:
                            this.loadTexture('enemycardB1', 0, false);
                            break;
                        default:
                            alert('Barbarian index out of range!');
                    }
                }
            });
        });

        // Loop overrun tiles
        $.each(d.Item.OverrunTiles, function () {
            var tile = HexTile.prototype.getTileById(this.Id);
            tile.loadTexture('hexagonoverrun', 0, false);
        });

        // Loop overrun developments
        $.each(d.Item.OverrunDevelopments, function () {
            var tile = HexTile.prototype.getTileById(this.Tile.Id);
            var developmentType = this.DevelopmentType;
            $.each(tile.children, function () {
                if (this.name == 'development' && this.developmentType == developmentType) // 1 = settlement;
                {
                    tile.removeChild(this);
                }
            });
        });
    }

    getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
    //this.game.state.start('EnemyOverrun', false, false);

    //$.each(hexGrid.children, function () { // loop each tile
    //    if (currentPlayer.playerColor == this.type && this.isEnemyTile() && this.hasEnemyCard()) { // only interested in the current player here
    //        $.each(this.children, function () { // loop each child on tile
    //            if (this.hasOwnProperty('barbarianIndex') && this.hasBarbarian) {
    //                this.barbarianIndex += 1;

    //                switch (this.barbarianIndex) {
    //                    case 1:
    //                        this.loadTexture('enemycardB1', 0, false);
    //                        break;
    //                    case 2:
    //                        this.loadTexture('enemycardB2', 0, false);
    //                        this.barbarianIndex = 0;
    //                        this.loadTexture('enemycardB0', 0, false);
    //                        // ToDo: Mark first tile in line as inactive.
    //                        // If it already inactive, mark the next.
    //                        // Making a row of inactives ends the game, or if capital is hit.
    //                        this.parent.setOverrunTile();
    //                        checkForEndGameState();

    //                        break;
    //                    default:
    //                        alert('Barbarian index out of range!');
    //                }

    //                // Pass data to server to update barbarian index
    //                var enemy = { "id": this.id, "hasBeenPlaced": this.hasBeenPlaced, "currentHexName": this.currentHexName, "barbarianIndex": this.barbarianIndex };
    //                postJSON('/Game/UpdateEnemy', "{data:" + JSON.stringify(enemy) + "}", success, error);
    //            }
    //        });
    //    }
    //});
}