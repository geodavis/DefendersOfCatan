var game_vm = {
    gameLog: ko.observable('Game Log: Player 1 did this.')
};

function updateLogText(text) {
    game_vm.gameLog(game_vm.gameLog() + '\n' + text);
}