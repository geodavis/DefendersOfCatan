public void executePhase() {
    switch(gamePhase) {
        case START_TURN:
            updateGamePhaseOrSubPhase();
            break;
        case ENEMY :
            Log.i("i", "Enemy Phase!");
            switch(gameSubPhase){
                case ACTION:
                    Log.i("", "Enemy Action Subphase!");
                    board.executePhaseSubPhase();
                    updateGamePhaseOrSubPhase();
                    break;
                case OVERRUN:
                    Log.i("", "Enemy Overrun Subphase!");
                    board.executePhaseSubPhase();
                    updateGamePhaseOrSubPhase();
                    break;
                case CARD:
                    Log.i("", "Enemy Card Subphase");
                    break;
                default:
                    Log.i("", "Invalid Enemy Subphase!");
            }
            break;
        case PLAYER :
            switch(gameSubPhase){
                case MOVE:
                    Log.i("", "Enemy Action Subphase!");
                    break;
                case RESOURCE_OR_FIGHT:
                    Log.i("", "Enemy Overrun Subphase!");
                    break;
                default:
                    Log.i("", "Invalid Player Subphase!");
            }
            break;
        case END_TURN:
            Player player = getCurrentPlayer();
            player.setMyTurn(false);
            int nextPlayerIndex = player.getPlayerIndex() + 1;
            if (player.getPlayerIndex() == 3) { // go back to player one, if last player
                players.get(0).setMyTurn(true);
            }
            else {
                players.get(nextPlayerIndex).setMyTurn(true); // advance to next player
            }
            setCurrentPlayer();
            updateGamePhaseOrSubPhase();
            break;
        default :
            Log.i("", "No Phase Executed!");
    }
}

public void updateGamePhaseOrSubPhase() {
    switch(gamePhase) {
        case START_TURN:
            gamePhase = Types.GamePhase.ENEMY;
            gameSubPhase = Types.GameSubPhase.ACTION;
            //setupDialog("Executing Action Phase");
            prompt.setText("Executing Action Phase");
            executePhase();
            break;
        case ENEMY :
            switch(gameSubPhase){
                case ACTION:
                    Log.i("", "Switching to Overrun Subphase!");
                    gameSubPhase = Types.GameSubPhase.OVERRUN;
                    //setupDialog("Executing Overrun phase.");
                    prompt.setText("Executing Overrun phase");
                    executePhase();
                    break;
                case OVERRUN:
                    Log.i("", "Switching to Card Subphase!");
                    gameSubPhase = Types.GameSubPhase.CARD;
                    //gameSubPhaseAction = Types.GameSubPhaseAction.CARD_SELECT;
                    //setupDialog("Please place a card.");
                    prompt.setText("Please place a card");
                    executePhase();
                    break;
                case CARD:
                    Log.i("", "Switching to Player phase, Move subphase");
                    gamePhase = Types.GamePhase.PLAYER;
                    gameSubPhase = Types.GameSubPhase.MOVE;
                    //gameSubPhaseAction = Types.GameSubPhaseAction.PLAYER_PIECE_SELECT;
                    //setupDialog("Please move your player piece.");
                    prompt.setText("Please move your player piece");
                    break;
                default:
                    Log.i("", "Invalid Enemy Subphase to switch to!");
            }
            break;
        case PLAYER :
            switch(gameSubPhase){
                case MOVE:
                    Log.i("", "Switching to Resource Subphase!");
                    gameSubPhase = Types.GameSubPhase.RESOURCE_OR_FIGHT;
                    //setupDialog("Please collect a resource or fight.");
                    prompt.setText("Please collect a resource or fight");
                    break;
                case RESOURCE_OR_FIGHT:
                    Log.i("", "End of turn after this phase");
                    gamePhase = Types.GamePhase.END_TURN;
                    gameSubPhase = null;
                    //setupDialog("End turn.");
                    executePhase();
                    //prompt.setText("End turn");
                    break;
                default:
                    Log.i("", "Invalid Player Subphase to switch to!");
            }
            break;
        case END_TURN:
            gamePhase = Types.GamePhase.START_TURN;
            //gameSubPhase = Types.GameSubPhase.ACTION;
            //setupDialog("Start turn.");
            executePhase();
            break;
        default :
            Log.i("", "Invalid Phase!");
    }
}

public void updateGameSubPhaseAction() {
    switch(gameSubPhaseAction) {
        case CARD_SELECT:
            //gameSubPhaseAction = Types.GameSubPhaseAction.CARD_SELECTED;
            break;
        case CARD_SELECTED:
            break;
        case PLAYER_PIECE_SELECT:
            //gameSubPhaseAction = Types.GameSubPhaseAction.PLAYER_PIECE_SELECTED;
            break;
        case PLAYER_PIECE_SELECTED:
            break;
        default :
            Log.i("", "Invalid Phase!");
    }
}