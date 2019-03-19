export class BoardCache {
  private cache: { [key: string]: Board | CardList | Card } = {};

  addBoard(board: Board) {
    this.cache[board.id] = board;

    board.lists.forEach(this.addList, this);
  }

  addList(list: CardList) {
    this.cache[list.id] = list;

    list.cards.forEach(this.addCard, this);
  }

  addCard(card: Card) {
    this.cache[card.id] = card;
  }

  getBoard(id: string): Board {
    return <Board>this.cache[id];
  }

  getList(id: string): CardList {
    return <CardList>this.cache[id];
  }

  getCard(id: string): Card {
    return <Card>this.cache[id];
  }

  removeBoard(board: Board) {
    board.lists.forEach(this.removeList, this);
    this.cache[board.id] = undefined;
  }

  removeBoardById(id: string) {
    const board = this.getBoard(id);
    if (!!board) {
      this.removeBoard(board);
    }
  }

  removeList(list: CardList) {
    list.cards.forEach(this.removeCard, this);
    this.cache[list.id] = undefined;
  }

  removeListById(id: string) {
    const list = this.getList(id);
    if (!!list) {
      this.removeList(list);
    }
  }

  removeCard(card: Card) {
    this.cache[card.id] = undefined;
  }

  removeCardById(id: string) {
    this.cache[id] = undefined;
  }

  contains(id: string) {
    return !!this.cache[id];
  }

  clear() {
    this.cache = {};
  }
}
