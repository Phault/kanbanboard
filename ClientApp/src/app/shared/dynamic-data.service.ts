import { Injectable, Inject, EventEmitter } from '@angular/core';
import { DataService } from './data.service';
import { BoardCache } from './board-cache';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import { AuthService } from './auth.service';

@Injectable()
export class DynamicDataService {

  public onBoardCreated = new EventEmitter<{board: Board}>();
  public onBoardChanged = new EventEmitter<{id: string, delta: BoardPatch}>();
  public onBoardDeleted = new EventEmitter<{id: string}>();

  public onListCreated = new EventEmitter<{list: CardList}>();
  public onListChanged = new EventEmitter<{id: string, delta: CardListPatch}>();
  public onListDeleted = new EventEmitter<{id: string}>();

  public onCardCreated = new EventEmitter<{card: Card}>();
  public onCardChanged = new EventEmitter<{id: string, delta: CardPatch}>();
  public onCardDeleted = new EventEmitter<{id: string}>();

  private connection: HubConnection;
  private cache = new BoardCache();

  private isConnected = false;

  private pendingSubscriptions = new Set<string>();

  constructor(private data: DataService,
    private auth: AuthService,
    @Inject('BASE_URL') private baseUrl: string) {

    const conn = this.connection = new HubConnectionBuilder()
      .withUrl(`${baseUrl}hubs/board`, { accessTokenFactory: () => auth.token })
      .build();

    conn.on('BoardCreated', this.boardCreated.bind(this));
    conn.on('BoardChanged', this.boardChanged.bind(this));
    conn.on('BoardDeleted', this.boardDeleted.bind(this));

    conn.on('ListCreated', this.listCreated.bind(this));
    conn.on('ListChanged', this.listChanged.bind(this));
    conn.on('ListDeleted', this.listDeleted.bind(this));

    conn.on('CardCreated', this.cardCreated.bind(this));
    conn.on('CardChanged', this.cardChanged.bind(this));
    conn.on('CardDeleted', this.cardDeleted.bind(this));

    auth.isAuthenticatedChange.subscribe(loggedIn => {
      if (loggedIn) {
        this.pendingSubscriptions.clear();
        this.connection.start()
          .then(() => {
            this.isConnected = true;

            this.pendingSubscriptions
              .forEach(b => this.subscribeToBoard(b), this);
          });
      } else {
        this.connection.stop();
        this.isConnected = false;
      }
    });
  }

  public getBoard(id: string): Observable<Board> {
    const board = this.cache.getBoard(id);

    if (!board) {
      return this.data.getBoard(id)
        .pipe(tap(b => {
          this.cache.addBoard(b);

          if (this.isConnected) {
            this.subscribeToBoard(id);
          } else {
            this.pendingSubscriptions.add(id);
          }
        }));
    }

    return of(board);
  }

  public getList(id: string): Observable<CardList> {
    const list = this.cache.getList(id);

    if (!list) {
      return this.data.getList(id)
        .pipe(tap(l => this.cache.addList(l)));
    }

    return of(list);
  }

  public getCard(id: string): Observable<Card> {
    const card = this.cache.getCard(id);

    if (!card) {
      return this.data.getCard(id)
        .pipe(tap(c => this.cache.addCard(c)));
    }

    return of(card);
  }

  private subscribeToBoard(boardId: string) {
    return this.connection.invoke('SubscribeToBoard', boardId);
  }

  private unsubscribeFromBoard(boardId: string) {
    return this.connection.invoke('UnsubscribeFromBoard', boardId);
  }

  private boardCreated(board: Board) {
    this.cache.addBoard(board);
    this.onBoardCreated.emit({ board });
  }

  private boardChanged(id: string, delta: BoardPatch) {
    const board = this.cache.getBoard(id);
    if (!board) { return; }

    this.patchObject(board, delta);
    this.onBoardChanged.emit({ id, delta });
  }

  private boardDeleted(id: string) {
    if (!this.cache.contains(id)) {
      return;
    }

    this.cache.removeBoardById(id);
    this.unsubscribeFromBoard(id);
    this.onBoardDeleted.emit({ id });
  }

  private listCreated(list: CardList) {
    const board = this.cache.getBoard(list.boardId);

    if (board) {
      board.lists.push(list);
      board.lists.sort((a, b) => a.position - b.position);
    }

    this.cache.addList(list);

    this.onListCreated.emit({ list });
  }

  private listChanged(id: string, delta: CardListPatch) {
    const list = this.cache.getList(id);

    if (!list) { return; }

    this.patchObject(list, delta);

    if ('position' in delta) {
      const board = this.cache.getBoard(list.boardId);
      if (board) {
        board.lists.sort((a, b) => a.position - b.position);
      }
    }

    this.onListChanged.emit({ id, delta });
  }

  private listDeleted(id: string) {
    const list = this.cache.getList(id);

    if (!list) { return; }

    const board = this.cache.getBoard(list.boardId);

    if (board) {
      const index = board.lists.indexOf(list);
      board.lists.splice(index, 1);
    }

    this.cache.removeList(list);

    this.onListDeleted.emit({ id });
  }

  private cardCreated(card: Card) {
    const list = this.cache.getList(card.listId);

    if (list) {
      list.cards.push(card);
      list.cards.sort((a, b) => a.position - b.position);
    }

    this.cache.addCard(card);

    this.onCardCreated.emit({ card });
  }

  private cardChanged(id: string, delta: CardPatch) {
    const card = this.cache.getCard(id);

    if (!card) { return; }

    const previousList = this.cache.getList(card.listId);

    this.patchObject(card, delta);

    if (card.listId !== previousList.id) {
      if (previousList) {
        previousList.cards.splice(previousList.cards.indexOf(card), 1);
      }

      const newList = this.cache.getList(card.listId);
      if (newList) {
        newList.cards.push(card);
        newList.cards.sort((a, b) => a.position - b.position);
      }
    } else if ('position' in delta && previousList) {
      previousList.cards.sort((a, b) => a.position - b.position);
    }

    this.onCardChanged.emit({ id, delta });
  }

  private cardDeleted(id: string) {
    const card = this.cache.getCard(id);

    if (!card) { return; }

    const list = this.cache.getList(card.listId);

    if (list) {
      const index = list.cards.indexOf(card);
      list.cards.splice(index, 1);
    }

    this.onCardDeleted.emit({ id });
  }

  private patchObject(target, delta) {
    Object.entries(delta).forEach(
      ([key, value]) => target[key] = value
    );
  }
}
