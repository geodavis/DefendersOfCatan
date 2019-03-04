Player = function (game, x, y, playerData) {
    var playerImage = this.getPlayerImageBasedOnPlayerColor(playerData.Color);
    Phaser.Sprite.call(this, game, x, y, playerImage);
    var self = this;
    this.anchor.setTo(0.5, 0.5);
    this.name = playerData.Name;
    this.id = playerData.Id;
    this.playerColor = playerData.Color
    this.health = ko.observable(playerData.Health);
    this.isOverrun = ko.observable(true);
    this.isCurrentPlayer = ko.observable(playerData.IsCurrentPlayer);
    this.currentHexName = "tile3_3";
    this.inputEnabled = true;
    this.input.useHandCursor = true;
    this.events.onInputOut.add(this.rollOut, this);
    this.events.onInputOver.add(this.rollOver, this);
    this.scale.setTo(.75, .75);
    this.resources = ko.observableArray([new PlayerResource(ResourcesEnum.brick, 0),
                      new PlayerResource(ResourcesEnum.grain, 0),
                      new PlayerResource(ResourcesEnum.ore, 0),
                      new PlayerResource(ResourcesEnum.wood, 0),
                      new PlayerResource(ResourcesEnum.wool, 0)]);
    this.cards = ko.observableArray([
                    new PlayerCard(cards[0].CardType, cards[0].CardDescription, 0),
                    new PlayerCard(cards[1].CardType, cards[1].CardDescription, 0),
                    new PlayerCard(cards[2].CardType, cards[2].CardDescription, 0),
                    new PlayerCard(cards[3].CardType, cards[3].CardDescription, 0)
    ]);
    this.developments = ko.observableArray([
                    new PlayerDevelopment(developments.developments()[0].developmentType, 0),
                    new PlayerDevelopment(developments.developments()[1].developmentType, 0),
                    new PlayerDevelopment(developments.developments()[2].developmentType, 0),
                    new PlayerDevelopment(developments.developments()[3].developmentType, 0),
                    new PlayerDevelopment(developments.developments()[4].developmentType, 0)
                  ]);

    this.addResourceToPlayer = function (resourceType) {
        $.each(this.resources(), function () {
            if (this.resourceType == resourceType) {
                tempResourceCount = this.resourceCount() + 1;
                this.resourceCount(tempResourceCount);
            }
        });
    };

    this.addCardToPlayer = function (cardType) {
        $.each(this.cards(), function () {
            if (this.cardType == cardType) {
                tempCount = this.cardCount() + 1;
                this.cardCount(tempCount);
            }
        });
    };

    this.setPlayerOverrun = function (isOverrun) {
        if (isOverrun) { // ToDo: remove overrun when clicking a card
            this.isOverrun(true);
            updateLogText(this.name + ' has been overrun!');
        }
        else {
            this.isOverrun(false);
            updateLogText(this.name + ' is no longer overrun.');
        }
    };

    this.playCard = function () {
        getJSONSync('/Game/GetCardPlaceables?cardType=' + this.cardType, self.highlightCardPlaceables, error); // URL, Success Function, Error Function
    };

    self.highlightCardPlaceables = function (d) {
        if (!d.HasError) {
            switch (d.Item.CardType) {
                case 0: // 
                    $.each(d.Item.TileIds, function () {
                        var tile = HexTile.prototype.getTileById(this);
                        var placeable = new EnemyBackPlaceable(game, 0, 0);
                        tile.addChild(placeable);
                    });
                    break;
                case 1:
                    $.each(d.Item.TileIds, function () {
                        var tile = HexTile.prototype.getTileById(this);
                        var placeable = new EnemyRemovePlaceable(game, 0, 0);
                        tile.addChild(placeable);
                    });
                    break;
                case 2:
                    $.each(d.Item.TileIds, function () {
                        var tile = HexTile.prototype.getTileById(this);
                        var placeable = new PlayerMovePlaceable(game, 0, 0);
                        tile.addChild(placeable);
                    });
                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:

                    break;
                default:
                    alert("Do not recogize development type!");

            }


        }
        else {
            alert(d.Error);
        }
    };

};

Player.prototype = Object.create(Phaser.Sprite.prototype);
Player.prototype.constructor = Player;

Player.prototype.rollOut = function () {
    //this.scale.x = 1;
    //this.scale.y = 1;
    //this.scale.setTo(.15, .15);
}
Player.prototype.rollOver = function () {
    //this.scale.x = 0.9;
    //this.scale.y = 0.9;
    //this.scale.setTo(.14, .14);
}

Player.prototype.setPurchasableDevelopments = function () { // ToDo: Move this to the items vm
    var self = this;
    // Loop all store items and check player resources for what they can purchase
    $.each(developments.developments(), function () { // Loop store items
        var playerCanPurchase = false;

        // Loop resource cost of item
        $.each(this.itemCost, function () { // loop each item cost resource
            var requiredResourceType = this.ResourceType;
            var requiredQty = this.Qty;

            $.each(self.resources(), function () { // check resource type and quantity is available for player
                var playerOwnedResourceType = this.resourceType;
                var playerOwnedResourceQty = this.resourceCount();
                if (requiredResourceType == playerOwnedResourceType) { // If we need to consider this resource type
                    if (playerOwnedResourceQty >= requiredQty) { // check quantity is available for player
                        playerCanPurchase = true;
                    }
                    else {
                        playerCanPurchase = false;
                    }
                }
            });
        });

        // Set purchasable or not in the store
        this.currentPlayerCanPurchase(playerCanPurchase);
    });
}

Player.prototype.getPlayerImageBasedOnPlayerColor = function (playerColor) {
    switch (playerColor) {
        case playerColors.red:
            return 'playerred';
            break;
        case playerColors.blue:
            return 'playerblue';
            break;
        case playerColors.yellow:
            return 'playeryellow';
            break;
        case playerColors.green:
            return 'playergreen';
            break;
        default: // ToDo
            break;
    }
}