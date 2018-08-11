GameStates.EnemyOverrun = function (game) {  

};

GameStates.EnemyOverrun.prototype = {
    create: function () {
        textPhase.text = 'Phase: Overrun';
        getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
        //this.state.start('EnemyCard', false, false);
        //highlight(enemyCards.children[3]);

        //alert('enemy overrun phase');


        // Check for overrun condition for all players and set if true
        //players.setPlayerOverrun();

        // Take health if overrun
        /*if (currentPlayer.isOverrun) {
            currentPlayer.health(currentPlayer.health() - 1);

            // Pass data to server to update player health
            var playerHealthTransfer = { "id": currentPlayer.id, "health": currentPlayer.health() };
            postJSON('/Game/UpdatePlayerHealth', "{data:" + JSON.stringify(playerHealthTransfer) + "}", success, error);
            updateLogText(currentPlayer.name + " lost 1 health.")
        }

        // Call next state (draw card or player move if overrun - if player is overrun, drawing a card is not required)
        if (!currentPlayer.isOverrun) {
            this.state.start('EnemyCard', false, false);
        } else {
            this.state.start('PlayerMove', false, false);
        }*/
    },

    update: function () {



    },

    render: function () {

        //this.debug.cameraInfo(this.camera, 500, 32);
        //this.debug.spriteCoords(player, 32, 32);
    }
};

