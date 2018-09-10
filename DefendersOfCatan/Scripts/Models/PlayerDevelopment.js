function PlayerDevelopment (developmentType, qty) {
    this.developmentType = developmentType;
    this.developmentCount = ko.observable(qty);
};
