function PlayerItem (itemType, qty) {
    this.itemType = itemType;
    this.itemCount = ko.observable(qty);
};
