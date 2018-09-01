items_vm = function (game, items) {
    var self = this;
    self.items = ko.observableArray();

    self.item = function () {
        var self = this;
        self.itemType = '';
        self.imagePath = '';
        self.itemName = '';
        self.resourceCostPaths = [];
        self.currentPlayerCanPurchase = ko.observable(false);
    };

    self.getImagePath = function (itemType) {
        var path = '';
        switch (itemType) {
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

    self.getResourceCostArray = function (itemCost) {
        var costArray = [];
        $.each(itemCost, function () {
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

    self.getItemCost = function (itemType) {
        var itemCost;
        $.each(this.items(), function () {
            if (this.itemType == itemType) {
                itemCost = this.itemCost;
            }
        });

        return itemCost;
    };

    self.purchaseItem = function (item) {
        getJSONSync('/Game/PurchaseItem?itemType=' + item.itemType, updatePlayerItems, error); // URL, Success Function, Error Function
    };

    self.updatePlayerItems = function (d) {

        var itemType = item.itemType;
        var itemCost = item.itemCost;

        // Take item cost resources away from player
        //var cost = items.getItemCost(this.itemType);
        //$.each(cost, function () { // loop each item cost resource
        //    var requiredResourceType = this.ResourceType;
        //    var requiredQty = this.Qty;

        //    $.each(currentPlayer.resources(), function () { // check resource type and quantity is available for player
        //        var playerOwnedResourceType = this.resourceType;
        //        var playerOwnedResourceQty = this.resourceCount();
        //        if (requiredResourceType == playerOwnedResourceType) { // If we need to consider this resource type
        //            this.ResourceCount(this.ResourceCount() - requiredQty);
        //        }
        //    });
        //});

        // Add item to player inventory

        alert('Test');
    }

    self.setPurchasableItem = function () {

    }

    // Set item array here
    $.each(items, function (index, value) {
        var item = new self.item();
        item.itemType = this.ItemType;
        item.itemName = this.ItemName;
        item.imagePath = self.getImagePath(this.ItemType);
        item.itemCost = this.ItemCost;
        item.resourceCostPaths = self.getResourceCostArray(this.ItemCost);
        self.items.push(item);
    });
    
};

//Items.prototype.constructor = Items;

//Items.prototype.addItem = function (itemType) {
//    var item = new Item(game, itemType);
//    this.items.push(item);
//}