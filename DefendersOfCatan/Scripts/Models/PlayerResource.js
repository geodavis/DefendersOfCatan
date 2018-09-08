function PlayerResource (resourceType, qty) {
    this.resourceType = resourceType;
    this.resourceTypeName = ko.observable(Object.keys(ResourcesEnum)[resourceType]);
    this.resourceCount = ko.observable(qty);
    switch (resourceType) {
        case ResourcesEnum.brick:
            this.imagePath = '../../Content/Assets/brick.png'
            break;
        case ResourcesEnum.grain:
            this.imagePath = '../../Content/Assets/grain.png'
            break;
        case ResourcesEnum.ore:
            this.imagePath = '../../Content/Assets/ore.png'
            break;
        case ResourcesEnum.wood:
            this.imagePath = '../../Content/Assets/wood.png'
            break;
        case ResourcesEnum.wool:
            this.imagePath = '../../Content/Assets/wool.png'
            break;
        default:

    }
    
};
