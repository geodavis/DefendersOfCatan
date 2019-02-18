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
        this.load.image('hexagonborder', '../../Content/Assets/borderHex.png');

        this.load.image('hexagonorange', '../../Content/Assets/hexagonCapital.png'); // capital
        this.load.image('hexagonblack', '../../Content/Assets/hexagonMountain.png'); // mountains

        // Resource Tiles
        this.load.image('hexagongraynoise', '../../Content/Assets/hexagonOre.png');
        this.load.image('hexagonbrownnoise', '../../Content/Assets/hexagonWood.png');
        this.load.image('hexagonyellownoise', '../../Content/Assets/hexagonGrain.png');
        this.load.image('hexagongreennoise', '../../Content/Assets/hexagonWool.png');
        this.load.image('hexagonrednoise', '../../Content/Assets/hexagonBrick.png');

        // Enemy Tiles
        this.load.image('hexagonrednoisewaves', '../../Content/Assets/hexagonEnemyRed.png');
        this.load.image('hexagonyellownoisewaves', '../../Content/Assets/hexagonEnemyBlue.png');
        this.load.image('hexagonbluenoisewaves', '../../Content/Assets/hexagonEnemyYellow.png');
        this.load.image('hexagongreennoisewaves', '../../Content/Assets/hexagonEnemyGreen.png');

        // Enemy Card Draw
        this.load.image('enemycarddraw', '../../Content/Assets/card-draw.png')

        // Player Move
        this.load.image('playermove', '../../Content/Assets/playerMove.png')


        //this.load.image('hexagongray', '../../Content/Assets/hexagongray.png');
        
        //this.load.image('hexagonbrown', '../../Content/Assets/hexagonbrown.png');
                        
        //this.load.image('hexagonred', '../../Content/Assets/hexagonred.png');
        //this.load.image('hexagonredwaves', '../../Content/Assets/hexagonredwaves.png');
        //this.load.image('hexagonredmovable', '../../Content/Assets/hexagonredmovable.png');

        //this.load.image('hexagonyellow', '../../Content/Assets/hexagonyellow.png');
        //this.load.image('hexagonyellowwaves', '../../Content/Assets/hexagonyellowwaves.png');

        //this.load.image('hexagonblue', '../../Content/Assets/hexagonblue.png');
        //this.load.image('hexagonbluenoise', '../../Content/Assets/hexagonbluenoise.png');
        //this.load.image('hexagonbluewaves', '../../Content/Assets/hexagonbluewaves.png');

        //this.load.image('hexagongreen', '../../Content/Assets/hexagongreen.png');
        //this.load.image('hexagongreenwaves', '../../Content/Assets/hexagongreenwaves.png');

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

        this.load.image('settlementBlue', '../../Content/Assets/settlementBlue.png');
        this.load.image('settlementPlacement', '../../Content/Assets/SettlementPlacement.png');
        this.load.image('roadPlacement', '../../Content/Assets/road_Placeable.png')
        this.load.image('roadBlue', '../../Content/Assets/RoadPlacement.png')

        this.load.image('dice1', '../../Content/Assets/dice1.png');
        this.load.image('dice2', '../../Content/Assets/dice2.png');
        this.load.image('dice3', '../../Content/Assets/dice3.png');
        this.load.image('dice4', '../../Content/Assets/dice4.png');
        this.load.image('dice5', '../../Content/Assets/dice5.png');
        this.load.image('dice6', '../../Content/Assets/dice6.png');

    },

    create: function () {
        //  We're going to be using physics, so enable the Arcade Physics system
        this.physics.startSystem(Phaser.Physics.ARCADE);
        //call next state
        this.state.start('MainMenu');
    }
};