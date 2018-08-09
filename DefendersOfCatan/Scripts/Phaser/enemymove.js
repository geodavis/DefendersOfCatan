GameStates.EnemyMove = function (game) {  

};

GameStates.EnemyMove.prototype = {
    create: function () {
        textPhase.text = 'Phase: Enemy Move';

        // Roll dice to see which players barbarians should advance
        //var playerColor = game.rnd.integerInRange(3, 6)
        
        // Execute Enemy Move phase
        getJSONWithoutDataSync('/Game/ExecuteEnemyMovePhase', GameStates.EnemyMove.prototype.updateBarbarians, error); // URL, Success Function, Error Function
        

    },

    update: function () {



    },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    }
};

GameStates.EnemyMove.prototype.updateBarbarians = function (d) {
    // Advance barbarians
    // alert('test');
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        $.each(d.Item, function () { // loop each tile
            var tile = HexTile.prototype.getTileById(this.Id);
            if (this.Enemy != null) {
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
            }
            else {
                tile.loadTexture('hexagonoverrun', 0, false);
            }

        });
    }

    this.game.state.start('EnemyOverrun', false, false);

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