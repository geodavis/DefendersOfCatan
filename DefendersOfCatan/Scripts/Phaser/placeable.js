//  Here is a custom game object
Placeable = function (game, x, y, developmentType) {
    //this.developmentType = developmentType;
    //var developmentImage = this.getDevelopmentImageBasedOnType(developmentType);
    Phaser.Sprite.call(this, game, x, y, 'settlementPlacement');
    //this.id = enemy.Id;
    //this.hasBarbarian = enemy.HasBarbarian;
    //this.barbarianIndex = enemy.BarbarianIndex;
    //this.currentHexName = enemy.CurrentHexName;
    //this.playerColor = enemy.PlayerColor; // assign each card a player color
    //this.anchor.setTo(0.5, 0.5);
    this.name = "placeable";
    //this.hasBeenPlaced = false;
    //this.inputEnabled = true;
    //this.input.useHandCursor = true;
    //this.events.onInputOut.add(this.rollOut, this);
    //this.events.onInputOver.add(this.rollOver, this);
    //this.events.onInputUp.add(this.onEnemyCardMouseUp, this);
    this.scale.setTo(.5, .5);
};

Placeable.prototype = Object.create(Phaser.Sprite.prototype);
Placeable.prototype.constructor = Placeable;

Placeable.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
Placeable.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

//Development.prototype.getDevelopmentImageBasedOnType = function (developmentType) {
//    switch (developmentType) {
//        case 0:
//            return 'settlementBlue';
//            break;
//        case 1:
//            return 'settlementBlue';
//            break;
//        case 2:
//            return 'settlementBlue';
//            break;
//        case 3:
//            return 'settlementBlue';
//            break;
//        default: // ToDo
//            break;
//    }
//}

//Enemy.prototype.onEnemyCardMouseUp = function (enemyCard) {
//    var selectedEnemyTransfer = { "enemyId": this.id };
//    postJSON('/Game/ExecuteEnemyClickedActions', "{data:" + JSON.stringify(selectedEnemyTransfer) + "}", executePostEnemyClickEvents, error);

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

//function highlightEnemyPlacementTiles(enemyCard) {
//    //var cardSelected = true; // todo: clean up card selected logic (may just remove it)
//    //if (!enemyCard.hasBeenPlaced && !cardSelected) { // ToDo: add error handling to trying to select another card before placing 
//        //selectedEnemyCard = enemyCard;
//        //cardSelected = true;

//        $.each(hexGrid.children, function () {
//            if (this.type == enemyCard.playerColor) {
//                highlight(this);
//            }
//        });
//    //}
//    //else {
//      //  alert('Card has already been placed!');
//    //}
//}

//Enemy.prototype.setAngle = function (tileType) { // FYI - tile type = player color
//    switch (tileType) {
//        case playerColors.red:
//            this.angle -= 30;
//            break;
//        case playerColors.blue:
//            this.angle += 30;
//            break;
//        case playerColors.yellow:
//            this.angle += 150;
//            break;
//        case playerColors.green:
//            this.angle += 210;
//            break;
//        default: // ToDo
//            break;
//    }
//}

//Enemy.prototype.setHasBeenPlaced = function (hasBeenPlaced) {
//    this.hasBeenPlaced = hasBeenPlaced;
//}

//Enemy.prototype.getEnemyById = function (id) {
//    var enemy;
//    $.each(enemyCards.children, function () {
//        if (this.id == id) {
//            enemy = this;
//        }
//    });

//    return enemy;
//}