var button;
var popup;
var tween = null;

function createPopup(game, x, y) {
    popup = game.add.sprite(x, y, 'background');
    popup.alpha = 0.8;
    popup.anchor.set(0.5);
    popup.inputEnabled = true;
    popup.input.enableDrag();

    //  Position the close button to the top-right of the popup sprite (minus 8px for spacing)
    var pw = (popup.width / 2) - 30;
    var ph = (popup.height / 2) - 8;

    //  And click the close button to close it down again
    var closeButton = game.make.sprite(pw, -ph, 'close');
    closeButton.inputEnabled = true;
    closeButton.input.priorityID = 1;
    closeButton.input.useHandCursor = true;
    closeButton.game = game;
    closeButton.events.onInputDown.add(closeWindow, this);

    //  Add the "close button" to the popup window image
    popup.addChild(closeButton);

    //  Hide it awaiting a click
    popup.scale.set(0.1);

    var gold = game.rnd.between(1, 10);
    var goldText = game.add.text(16, 16, 'Gold: ' + gold, { fontSize: '32px', fill: '#FF9E2C' });
    popup.addChild(goldText);
    return gold;
}



function openWindow(game) {

    if ((tween !== null && tween.isRunning) || popup.scale.x === 1) {
        return;
    }

    //  Create a tween that will pop-open the window, but only if it's not already tweening or open
    tween = game.add.tween(popup.scale).to({ x: 1, y: 1 }, 1000, Phaser.Easing.Elastic.Out, true);

}

function closeWindow() {

    if (tween && tween.isRunning || popup.scale.x === 0.1) {
        return;
    }

    //  Create a tween that will close the window, but only if it's not already tweening or closed
    tween = game.add.tween(popup.scale).to({ x: 0.1, y: 0.1 }, 500, Phaser.Easing.Elastic.In, true);
    player.canMove = true;

}