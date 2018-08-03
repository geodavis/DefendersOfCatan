var game = new Phaser.Game(800, 600, Phaser.AUTO, 'content', null, true);

window.onload = function () {

    //  Add the States your game has.
    game.state.add('Boot', GameStates.Boot);
    game.state.add('Preloader', GameStates.Preloader);
    game.state.add('EnemyMove', GameStates.EnemyMove);
    game.state.add('EnemyOverrun', GameStates.EnemyOverrun);
    game.state.add('EnemyCard', GameStates.EnemyCard);
    game.state.add('WorldGenerator', GameStates.WorldGenerator);
    game.state.add('MainMenu', GameStates.MainMenu);
    game.state.add('Game', GameStates.Game);
    game.state.add('PlayerMove', GameStates.PlayerMove);
    game.state.add('PlayerResourceOrFight', GameStates.PlayerResourceOrFight);

    //  Now start the Boot state.
    game.state.start('Boot');

};