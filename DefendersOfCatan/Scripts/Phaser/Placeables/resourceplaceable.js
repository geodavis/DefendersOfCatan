ResourcePlaceable = function (game, x, y, resourceType) {
    Phaser.Sprite.call(this, game, x, y, this.getCardImageBasedOnResourceType(resourceType));
    this.name = "resourceplaceable";
    this.resourceType = resourceType;
    this.inputEnabled = true;
    this.anchor.setTo(0.5, 0.5);
    //this.scale.setTo(0.9, 0.9);
    this.events.onInputUp.add(this.onTap, this);
    animateCards(this); // move up and down along y-axis
};

ResourcePlaceable.prototype = Object.create(Phaser.Sprite.prototype);
ResourcePlaceable.prototype.constructor = ResourcePlaceable;

ResourcePlaceable.prototype.onTap = function () {
    var clickedPlaceableTransfer = { "parentTileId": this.parent.id };
    postJSON('/Game/AddResourceToPlayer', "{data:" + JSON.stringify(clickedPlaceableTransfer) + "}", this.executePostPlaceableClickEvents, error);
}

ResourcePlaceable.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
ResourcePlaceable.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

ResourcePlaceable.prototype.getCardImageBasedOnResourceType = function (resourceType) {
    switch (resourceType) {
        case ResourcesEnum.brick:
            return 'cardBrick';
            break;
        case ResourcesEnum.grain:
            return 'cardGrain';
            break;
        case ResourcesEnum.wood:
            return 'cardWood';
            break;
        case ResourcesEnum.ore:
            return 'cardOre';
            break;
        case ResourcesEnum.wool:
            return 'cardWool';
            break;
        default:
            // do not assign a resource
        }
}
                
ResourcePlaceable.prototype.removePlaceablesFromTiles = function () {
    // Remove placement image from tiles
    $.each(hexGrid.children, function () { // loop each tile
        var gridTile = this;
        $.each(gridTile.children, function () { // loop each child of tile
            if (this.name == 'resourceplaceable') {
                gridTile.removeChild(this);
            }
        });
    });
}

ResourcePlaceable.prototype.executePostPlaceableClickEvents = function (d) {
    if (d.HasError) {
        alert(d.Error);
    }
    else {
        GameStates.PlayerResourceOrFight.prototype.addResourceToPlayer(d.Item.ResourceType);
        $.each(hexGrid.children, function () {
            var tile = this;
            $.each(tile.children, function () { // loop each child of tile
                if (this.name == 'resourceplaceable') {
                    tile.removeChild(this);
                }
            });
        });

        getJSONSync('/Game/MoveToNextPlayer', moveToNextPlayer, error); // URL, Success Function, Error Function
        getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
    }
}