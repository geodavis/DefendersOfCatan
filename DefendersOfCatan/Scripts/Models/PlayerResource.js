function PlayerResource (resourceType, qty) {
    this.resourceType = resourceType;
    this.resourceTypeName = ko.observable(Object.keys(ResourcesEnum)[resourceType]);
    this.resourceCount = ko.observable(qty);
    switch (resourceType) {
        case ResourcesEnum.brick:
            this.imagePath = '../../Content/Assets/CardBrick.png'
            break;
        case ResourcesEnum.grain:
            this.imagePath = '../../Content/Assets/CardGrain.png'
            break;
        case ResourcesEnum.ore:
            this.imagePath = '../../Content/Assets/CardOre.png'
            break;
        case ResourcesEnum.wood:
            this.imagePath = '../../Content/Assets/CardWood.png'
            break;
        case ResourcesEnum.wool:
            this.imagePath = '../../Content/Assets/CardWool.png'
            break;
        default:

    }
    
};
