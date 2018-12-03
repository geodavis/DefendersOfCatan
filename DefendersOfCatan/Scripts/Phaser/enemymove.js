GameStates.EnemyMove = function (game) {  

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
    // Advance barbarians
    // ToDo: Remove settlement if barbarian hits and settlement exists. This occurs rather than flipping tile.
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        $.each(d.Item, function () { // loop each tile
            var tile = HexTile.prototype.getTileById(this.Id);
            // First check if settlement exists. If so, remove settlement from UI
            $.each(tile.children, function () {
                if (this.name == 'development' && this.type == 1) // 1 = settlement
                {
                    tile.removeChild(this); // ToDo: TEST THIS LOGIC TO ENSURE SETTLEMENT GETS REMOVED
                }
            });

            //ToDo: Update logic to look for new list of tiles specific to barbarian or overrun. remove development appropriately.
            // Next, reset barbarian index and update UI
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