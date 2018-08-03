//  Here is a custom game object
Player = function (game, x, y, playerName, playerColor) {
    var playerImage = getPlayerImageBasedOnPlayerColor(playerColor);

    Phaser.Sprite.call(this, game, x, y, playerImage);
    this.anchor.setTo(0.5, 0.5);
    this.name = playerName;
    this.currentHexName = "tile3_3";
    this.playerColor = playerColor;
    this.health = 5;
    this.inputEnabled = true;
    this.input.useHandCursor = true;
    this.events.onInputOut.add(this.rollOut, this);
    this.events.onInputOver.add(this.rollOver, this);
    this.scale.setTo(5, 5);
    this.resources = [new PlayerResource(ResourcesEnum.brick, 0),
                      new PlayerResource(ResourcesEnum.grain, 0),
                      new PlayerResource(ResourcesEnum.ore, 0),
                      new PlayerResource(ResourcesEnum.wood, 0),
                      new PlayerResource(ResourcesEnum.wool, 0)];

    this.items = [new PlayerItem(ItemsEnum.Item1, 0),
                      new PlayerItem(ItemsEnum.Item2, 0),
                      new PlayerItem(ItemsEnum.Item3, 0),
                      new PlayerItem(ItemsEnum.Item4, 0),
                      new PlayerItem(ItemsEnum.Item5, 0)];


    //vm.updatePlayerResources(player.resources);
}

Player.prototype = Object.create(Phaser.Sprite.prototype);
Player.prototype.constructor = Player;

Player.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
Player.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

