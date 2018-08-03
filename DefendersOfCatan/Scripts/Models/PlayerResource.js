function PlayerResource (resourceType, qty) {
    this.resourceType = resourceType;
    this.resourceTypeName = ko.observable(Object.keys(ResourcesEnum)[resourceType]);
    this.resourceCount = ko.observable(qty);
    switch (resourceType) {
        case ResourcesEnum.brick:
            this.imagePath = '../../Content/Assets/hexagonred.png'
            break;
        case ResourcesEnum.grain:
            this.imagePath = '../../Content/Assets/hexagonyellow.png'
            break;
        case ResourcesEnum.ore:
            this.imagePath = '../../Content/Assets/hexagongray.png'
            break;
        case ResourcesEnum.wood:
            this.imagePath = '../../Content/Assets/hexagonbrown.png'
            break;
        case ResourcesEnum.wool:
            this.imagePath = '../../Content/Assets/hexagongreen.png'
            break;
        default:

    }
    
};
