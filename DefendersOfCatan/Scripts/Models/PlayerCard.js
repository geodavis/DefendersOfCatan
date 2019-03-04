function PlayerCard(cardType, cardDescription, qty) {
    this.cardType = cardType;
    this.cardDescription = cardDescription;
    this.cardCount = ko.observable(qty);
    switch (cardType) {
        case 0:
            this.imagePath = '../../Content/Assets/CardDevelopment.png'
            break;
        case 1:
            this.imagePath = '../../Content/Assets/CardDevelopment.png'
            break;
        case 2:
            this.imagePath = '../../Content/Assets/CardDevelopment.png'
            break;
        case 3:
            this.imagePath = '../../Content/Assets/CardDevelopment.png'
            break;
        default:

    }
};