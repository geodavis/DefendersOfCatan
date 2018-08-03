// Boot will take care of initializing a few settings,

// declare the object that will hold all game states
var GameStates = {
    //quite common to add game variables/constants in here
    
};

GameStates.Boot = function (game) {  //declare the boot state

};

GameStates.Boot.prototype = {
    preload: function () {
        // load assets to be used later in the preloader e.g. for loading screen / splashscreen
        //this.load.image('preloaderBar', '../../Content/Assets/preloader-bar.png');
    },
    create: function () {
        // setup game environment
        // scale, input etc..
        this.game.scale.pageAlignHorizontally = true;
        this.game.scale.pageAlignVertically = true;
        this.game.scale.setScreenSize = true;
        this.game.scale.scaleMode = Phaser.ScaleManager.SHOW_ALL;
        this.game.scale.setMinMax(400, 300, 800, 600);

        this.game.scale.refresh();
        // call next state
        this.state.start('Preloader');
    }
};

