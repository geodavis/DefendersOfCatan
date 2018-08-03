//  Here is a custom game object
Barbarian = function (game, x, y, image) {
    Phaser.Sprite.call(this, game, x, y, image);
    this.anchor.setTo(0.25, 0.25);
    this.name = "barbarian";
    this.barbarianIndex = 0;
    this.inputEnabled = true;
    this.input.useHandCursor = true;
    //this.events.onInputOut.add(this.rollOut, this);
    //this.events.onInputOver.add(this.rollOver, this);
    this.scale.setTo(5, 5);

};

Barbarian.prototype = Object.create(Phaser.Sprite.prototype);
Barbarian.prototype.constructor = Barbarian;

Barbarian.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
Barbarian.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

Barbarian.prototype.setIndex = function () {

}