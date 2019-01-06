//  Here is a custom game object
PlaceableRoad = function (game, x, y) {
    Phaser.Sprite.call(this, game, x, y, 'roadPlacement');
    //this.anchor.setTo(0.5, 0.5);
    this.name = "placeableRoad";
    //this.hasBeenPlaced = false;
    //this.inputEnabled = true;
    //this.input.useHandCursor = true;
    //this.events.onInputOut.add(this.rollOut, this);
    //this.events.onInputOver.add(this.rollOver, this);
    //this.events.onInputUp.add(this.onEnemyCardMouseUp, this);
    //this.scale.setTo(.5, .5);
};

PlaceableRoad.prototype = Object.create(Phaser.Sprite.prototype);
PlaceableRoad.prototype.constructor = PlaceableRoad;

PlaceableRoad.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
PlaceableRoad.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}