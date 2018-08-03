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
    switch (game.state.getCurrentState().key) { // switch on game state
        case 'EnemyCard': // Enemy card phase means we select the card to place and set selected flag to true
            if (!enemyCard.hasBeenPlaced && !cardSelected) { // ToDo: add error handling to trying to select another card before placing 
                selectedEnemyCard = enemyCard;
                cardSelected = true;

                $.each(hexGrid.children, function () {
                    if (this.type == enemyCard.playerColor) {
                        highlight(this);
                    }
                });
            }
            else {
                alert('Card has already been placed!');
            }
            break;
        case 'PlayerMove':
                // ToDo: error handling
            break;
        case 'PlayerResourceOrFight':
            var enemyTile = hexGrid.getByName(this.currentHexName);
            var playerTile = hexGrid.getByName(currentPlayer.currentHexName);

            // If enemy card is a neighbor to the current player, remove the card from play
            if (isNeighbor(playerTile, enemyTile)) {
                // Send enemy to server to be removed
                var enemyTransfer = { "enemyId": enemyCard.id, "tileId": enemyTile.id };
                postJSON('/Game/RemoveEnemy', "{data:" + JSON.stringify(enemyTransfer) + "}", GameStates.PlayerResourceOrFight.prototype.removeEnemy, error);
            }
            else {
                alert('Not in range to attack!');
            }

            break;
        default:
            // ToDo:
    }
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