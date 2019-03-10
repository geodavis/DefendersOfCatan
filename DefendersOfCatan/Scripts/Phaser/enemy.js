//  Here is a custom game object
Enemy = function (game, x, y, enemyImage, enemy) {
    Phaser.Sprite.call(this, game, x, y, enemyImage);
    this.id = enemy.Id;
    this.hasBarbarian = enemy.HasBarbarian;
    this.barbarianIndex = enemy.BarbarianIndex;
    this.currentHexName = enemy.CurrentHexName;
    this.playerColor = enemy.PlayerColor; // assign each card a player color
    this.anchor.setTo(0.5, 0.5);
    this.name = "enemycard";
    this.hasBeenPlaced = false;
    this.inputEnabled = true;
    this.input.useHandCursor = true;
    this.events.onInputOut.add(this.rollOut, this);
    this.events.onInputOver.add(this.rollOver, this);
    this.events.onInputUp.add(this.onEnemyCardMouseUp, this);
    this.scale.setTo(.15, .15);
    //game.add.existing(this);
};

Enemy.prototype = Object.create(Phaser.Sprite.prototype);
Enemy.prototype.constructor = Enemy;

Enemy.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
Enemy.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

Enemy.prototype.onEnemyCardMouseUp = function (enemyCard) {
    var selectedEnemyTransfer = { "enemyId": this.id };
    postJSON('/Game/ExecuteEnemyClickedActions', "{data:" + JSON.stringify(selectedEnemyTransfer) + "}", executePostEnemyClickEvents, error);
}

function executePostEnemyClickEvents(d) {
    if (!d.HasError) {
        var gameState = d.Item.GameState;
        var enemyCard = Enemy.prototype.getEnemyById(d.Item.EnemyId);

        switch (gameState) { // switch on game state
            case 'EnemyCard': // Enemy card phase means we select the card to place and set selected flag to true
                highlightEnemyPlacementTiles(enemyCard);
                break;
            case 'PlayerMove':
                // ToDo: error handling
                break;
            case 'PlayerResourceOrFight':
                //var roll = d.Item.DiceRolls[0]; // first dice foll

                //$.each(diceGroup.children, function () {
                //    if (this.number == roll) {
                //        this.visible = true;
                //    }
                //    else {
                //        this.visible = false;
                //    }
                //});

                //GameStates.PlayerResourceOrFight.prototype.removeEnemy(d.Item.EnemyTileId);
                //var player = players.getPlayerById(d.Item.OverrunPlayerId);
                //player.setPlayerOverrun(false);

                //// Advance to the next player and phase
                //getJSONSync('/Game/MoveToNextPlayer', moveToNextPlayer, error); // URL, Success Function, Error Function
                //getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
                break;
            default:
                alert("Game state unknown!");
        }
    }
    else {
        alert(d.Error);
    }

}

function highlightEnemyPlacementTiles(enemyCard) {
    $.each(hexGrid.children, function () {
        if (this.type == enemyCard.playerColor) {
            highlight(this);
        }
    });
}

Enemy.prototype.setAngle = function (tileType) { // FYI - tile type = player color
    switch (tileType) {
        case playerColors.red:
            this.angle -= 30;
            break;
        case playerColors.blue:
            this.angle += 30;
            break;
        case playerColors.yellow:
            this.angle += 150;
            break;
        case playerColors.green:
            this.angle += 210;
            break;
        default: // ToDo
            break;
    }
}

Enemy.prototype.setHasBeenPlaced = function (hasBeenPlaced) {
    this.hasBeenPlaced = hasBeenPlaced;
}

Enemy.prototype.getEnemyById = function (id) {
    var enemy;
    $.each(enemyCards.children, function () {
        if (this.id == id) {
            enemy = this;
        }
    });

    return enemy;
}