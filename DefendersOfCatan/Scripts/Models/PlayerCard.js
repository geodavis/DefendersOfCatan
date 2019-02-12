function PlayerCard(cardType, qty) {
    this.cardType = cardType;
    this.cardCount = ko.observable(qty);
    switch (cardType) {
        case 'a':
            this.imagePath = '../../Content/Assets/CardDevelopment.png'
            break;
        case 'b':
            this.imagePath = '../../Content/Assets/CardDevelopment.png'
            break;
        case 'c':
            this.imagePath = '../../Content/Assets/CardDevelopment.png'
            break;
        case 'd':
            this.imagePath = '../../Content/Assets/CardDevelopment.png'
            break;
        default:

    }
};