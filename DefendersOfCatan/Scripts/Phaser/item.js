Item = function (game, itemType) {
    //Phaser.Sprite.call(this, game, x, y, playerImage, playerColor);
    this.itemType = itemType;
    this.itemTypeName = Object.keys(ItemsEnum)[itemType];
    this.cost = ko.observableArray([ResourcesEnum.brick, ResourcesEnum.brick]);
    this.costImagePaths = [];

    var self = this;

    // Set the cost
    $.each(this.cost(), function (index, value) { // loop each child
        switch (value) {
            case ResourcesEnum.brick:
                self.costImagePaths.push('../../Content/Assets/hexagonred.png');
                break;
            case ResourcesEnum.grain:
                self.costImagePaths.push('../../Content/Assets/hexagonyellow.png');
                break;
            case ResourcesEnum.ore:
                self.costImagePaths.push('../../Content/Assets/hexagongray.png');
                break;
            case ResourcesEnum.wood:
                self.costImagePaths.push('../../Content/Assets/hexagonbrown.png');
                break;
            case ResourcesEnum.wool:
                self.costImagePaths.push('../../Content/Assets/hexagongreen.png');
                break;
            default:

        }
    });

    switch (itemType) {
        case ItemsEnum.Item1:
            this.imagePath = '../../Content/Assets/hexagonred.png'
            break;
        case ItemsEnum.Item2:
            this.imagePath = '../../Content/Assets/hexagonyellow.png'
            break;
        case ItemsEnum.Item3:
            this.imagePath = '../../Content/Assets/hexagongray.png'
            break;
        case ItemsEnum.Item4:
            this.imagePath = '../../Content/Assets/hexagonbrown.png'
            break;
        case ItemsEnum.Item5:
            this.imagePath = '../../Content/Assets/hexagongreen.png'
            break;
        default:

    }

}

//Item.prototype = Object.create(Phaser.Sprite.prototype);
Item.prototype.constructor = Item;


