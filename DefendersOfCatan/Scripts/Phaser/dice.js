//  Here is a custom game object
Dice = function (game, x, y, diceImage, number) {
    Phaser.Sprite.call(this, game, x, y, diceImage);
    this.number = number;
    this.anchor.setTo(0.5, 0.5);
    this.name = "dice";
    this.inputEnabled = true;
    this.input.useHandCursor = true;
    this.events.onInputOut.add(this.rollOut, this);
    this.events.onInputOver.add(this.rollOver, this);
    //this.events.onInputUp.add(this.onEnemyCardMouseUp, this);
    //this.scale.setTo(.15, .15);
    //game.add.existing(this);
};

Dice.prototype = Object.create(Phaser.Sprite.prototype);
Dice.prototype.constructor = Dice;

Dice.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
Dice.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

//Enemy.prototype.onEnemyCardMouseUp = function (enemyCard) {
//    var selectedEnemyTransfer = { "enemyId": this.id };
//    postJSON('/Game/ExecuteEnemyClickedActions', "{data:" + JSON.stringify(selectedEnemyTransfer) + "}", executePostEnemyClickEvents, error);

//    // todo: code below is being moved to the server... pick up here 
//    //switch (game.state.getCurrentState().key) { // switch on game state
//    //    case 'EnemyCard': // Enemy card phase means we select the card to place and set selected flag to true

//    //        var selectedEnemyTransfer = { "enemyId": this.id, "playerId": currentPlayer.id };
//    //        postJSON('/Game/SetSelectedEnemy', "{data:" + JSON.stringify(selectedEnemyTransfer) + "}", highlightEnemyPlacementTiles, error);

//    //        break;
//    //    case 'PlayerMove':
//    //            // ToDo: error handling
//    //        break;
//    //    case 'PlayerResourceOrFight':
//    //        var enemyTile = hexGrid.getByName(this.currentHexName);
//    //        var playerTile = hexGrid.getByName(currentPlayer.currentHexName);

//    //        // If enemy card is a neighbor to the current player, remove the card from play
//    //        if (isNeighbor(playerTile, enemyTile)) {
//    //            // Send enemy to server to be removed
//    //            var enemyTransfer = { "enemyId": enemyCard.id, "tileId": enemyTile.id };
//    //            postJSON('/Game/RemoveEnemy', "{data:" + JSON.stringify(enemyTransfer) + "}", GameStates.PlayerResourceOrFight.prototype.removeEnemy, error);
//    //        }
//    //        else {
//    //            alert('Not in range to attack!');
//    //        }

//    //        break;
//    //    default:
//    //        // ToDo:
//    //}
//}

//function executePostEnemyClickEvents(d) {
//    if (!d.HasError) {
//        var gameState = d.Item.GameState;
//        var enemyCard = Enemy.prototype.getEnemyById(d.Item.EnemyId);

//        switch (gameState) { // switch on game state
//            case 'EnemyCard': // Enemy card phase means we select the card to place and set selected flag to true
//                highlightEnemyPlacementTiles(enemyCard);
//                break;
//            case 'PlayerMove':
//                // ToDo: error handling
//                break;
//            case 'PlayerResourceOrFight':
//                GameStates.PlayerResourceOrFight.prototype.removeEnemy(d.Item.EnemyTileId)
//                break;
//            default:
//                alert("Game state unknown!");
//        }
//    }
//    else {
//        alert(d.Error);
//    }

//}


