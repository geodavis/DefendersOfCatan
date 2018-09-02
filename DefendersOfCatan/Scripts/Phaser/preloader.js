// Preloader will load all of the assets like graphics and audio
GameStates.Preloader = function (game) {
    this.preloadBar = null;
}

GameStates.Preloader.prototype = {
    preload: function () {
        // common to add a loading bar sprite here...
        //this.preloadBar = this.add.sprite(this.game.width / 2 - 100, this.game.height / 2, 'preloaderBar');
        //this.load.setPreloadSprite(this.preloadBar);

        // load all game assets
        // images, spritesheets, atlases, audio etc..
        this.load.image('hexagonorange', '../../Content/Assets/desertHex.gif');
        this.load.image('hexagonblack', '../../Content/Assets/desertHex.gif');

        this.load.image('hexagongray', '../../Content/Assets/hexagongray.png');
        this.load.image('hexagongraynoise', '../../Content/Assets/oreHex.gif');

        this.load.image('hexagonbrown', '../../Content/Assets/hexagonbrown.png');
        this.load.image('hexagonbrownnoise', '../../Content/Assets/woodHex.gif');
                
        this.load.image('hexagonred', '../../Content/Assets/hexagonred.png');
        this.load.image('hexagonrednoise', '../../Content/Assets/clayhex.gif');
        this.load.image('hexagonredwaves', '../../Content/Assets/hexagonredwaves.png');
        this.load.image('hexagonrednoisewaves', '../../Content/Assets/desertHex.gif');
        //this.load.image('hexagonredmovable', '../../Content/Assets/hexagonredmovable.png');

        this.load.image('hexagonyellow', '../../Content/Assets/hexagonyellow.png');
        this.load.image('hexagonyellownoise', '../../Content/Assets/wheatHex.gif');
        this.load.image('hexagonyellowwaves', '../../Content/Assets/hexagonyellowwaves.png');
        this.load.image('hexagonyellownoisewaves', '../../Content/Assets/desertHex.gif');

        this.load.image('hexagonblue', '../../Content/Assets/hexagonblue.png');
        this.load.image('hexagonbluenoise', '../../Content/Assets/hexagonbluenoise.png');
        this.load.image('hexagonbluewaves', '../../Content/Assets/hexagonbluewaves.png');
        this.load.image('hexagonbluenoisewaves', '../../Content/Assets/desertHex.gif');

        this.load.image('hexagongreen', '../../Content/Assets/hexagongreen.png');
        this.load.image('hexagongreennoise', '../../Content/Assets/sheepHex.gif');
        this.load.image('hexagongreenwaves', '../../Content/Assets/hexagongreenwaves.png');
        this.load.image('hexagongreennoisewaves', '../../Content/Assets/desertHex.gif');

        this.load.image('hexagonoverrun', '../../Content/Assets/desertHex.gif');

        this.load.image('playerred', '../../Content/Assets/playerred.png');
        this.load.image('playerblue', '../../Content/Assets/playerblue.png');
        this.load.image('playeryellow', '../../Content/Assets/playeryellow.png');
        this.load.image('playergreen', '../../Content/Assets/playergreen.png');
        this.load.image('enemycard', '../../Content/Assets/enemycard.png');
        this.load.image('enemycardB0', '../../Content/Assets/enemycardB0.png');
        this.load.image('enemycardB1', '../../Content/Assets/enemycardB1.png');
        this.load.image('enemycardB2', '../../Content/Assets/enemycardB2.png');
        //this.load.image('barbarian', '../../Content/Assets/barbarian.png');

    },

    create: function () {
        //  We're going to be using physics, so enable the Arcade Physics system
        this.physics.startSystem(Phaser.Physics.ARCADE);
        //call next state
        this.state.start('MainMenu');
    }
};