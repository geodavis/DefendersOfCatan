developments_vm = function (game, developments) {
    var self = this;
    self.developments = ko.observableArray();
    self.purchasePhase = ko.observable(false);


    self.development = function () {
        var self = this;
        self.developmentType = '';
        self.imagePath = '';
        self.developmentName = '';
        self.resourceCostPaths = [];
        self.currentPlayerCanPurchase = ko.observable(false);
    };

    self.getImagePath = function (developmentType) {
        var path = '';
        switch (developmentType) {
            case ItemsEnum.Item1:
                path = '../../Content/Assets/hexagonred.png'
                break;
            case ItemsEnum.Item2:
                path = '../../Content/Assets/hexagonyellow.png'
                break;
            case ItemsEnum.Item3:
                path = '../../Content/Assets/hexagongray.png'
                break;
            case ItemsEnum.Item4:
                path = '../../Content/Assets/hexagonbrown.png'
                break;
            case ItemsEnum.Item5:
                path = '../../Content/Assets/hexagongreen.png'
                break;
            default:
        }

        return path;
    };

    self.getResourceCostArray = function (developmentCost) {
        var costArray = [];
        $.each(developmentCost, function () {
            for (i = 0; i < this.Qty; i++) {
                switch (this.ResourceType) {
                    case ResourcesEnum.brick:
                        costArray.push('../../Content/Assets/hexagonred.png');
                        break;
                    case ResourcesEnum.grain:
                        costArray.push('../../Content/Assets/hexagonyellow.png');
                        break;
                    case ResourcesEnum.ore:
                        costArray.push('../../Content/Assets/hexagongray.png');
                        break;
                    case ResourcesEnum.wood:
                        costArray.push('../../Content/Assets/hexagonbrown.png');
                        break;
                    case ResourcesEnum.wool:
                        costArray.push('../../Content/Assets/hexagongreen.png');
                        break;
                    default:

                }
            }
        });

        return costArray;
    };

    //self.getItemCost = function (itemType) {
    //    var itemCost;
    //    $.each(this.items(), function () {
    //        if (this.itemType == itemType) {
    //            itemCost = this.itemCost;
    //        }
    //    });

    //    return itemCost;
    //};

    self.declinePurchase = function () {
        getJSONSync('/Game/GetNextGameState', startNextGameState, error); // URL, Success Function, Error Function
    };

    self.purchaseDevelopment = function (development) {
        getJSONSync('/Game/PurchaseDevelopment?developmentType=' + development.developmentType, self.placeOrUpdatePlayerDevelopments, error); // URL, Success Function, Error Function
    };

    self.placeOrUpdatePlayerDevelopments = function (d) {
        if (!d.HasError) {
            var developmentType = d.Item

            // Remove appropriate resources from player UI
            // ToDo:

            if (developmentType == 5) { // This is an item card- add item to player inventory
                
                currentPlayer.addDevelopmentToPlayer(d.Item);
            }
            else // Prompt user to place immediately
            {
                // Highlight placeable tiles
                // When the user clicks the tile, place the item
                // ToDo: add purchase item game phase, add place item game phase
            }


            

        }
        else {
            alert(d.Error);
        }
    }

    self.setPurchasableDevelopment = function () {

    }

    // Set development array here
    $.each(developments, function (index, value) {
        var development = new self.development();
        development.developmentType = this.DevelopmentType;
        development.developmentName = this.DevelopmentTypeReadable;
        development.imagePath = self.getImagePath(this.DevelopmentType);
        development.developmentCost = this.DevelopmentCost;
        development.resourceCostPaths = self.getResourceCostArray(this.DevelopmentCost);
        self.developments.push(development);
    });
    
};

//Items.prototype.constructor = Items;

//Items.prototype.addItem = function (itemType) {
//    var item = new Item(game, itemType);
//    this.items.push(item);
//}