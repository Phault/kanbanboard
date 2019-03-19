import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class DataService {

  constructor(private http: HttpClient,
              @Inject('BASE_URL') private baseUrl: string) { }

  getBoard(id: string): Observable<Board> {
    return this.http.get<Board>(`${this.baseUrl}api/boards/${id}`);
  }

  patchBoard(id: string, patch: BoardPatch): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}api/boards/${id}`, patch);
  }

  deleteBoard(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}api/boards/${id}`);
  }

  getList(id: string): Observable<CardList> {
    return this.http.get<CardList>(`${this.baseUrl}api/lists/${id}`);
  }

  createList(list: Partial<CardList>): Observable<CardList> {
    return this.http.post<CardList>(`${this.baseUrl}api/lists`, list);
  }

  patchList(id: string, patch: CardListPatch): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}api/lists/${id}`, patch);
  }

  deleteList(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}api/lists/${id}`);
  }

  getCard(id: string): Observable<Card> {
    return this.http.get<Card>(`${this.baseUrl}api/cards/${id}`);
  }

  createCard(card: Partial<Card>): Observable<Card> {
    return this.http.post<Card>(`${this.baseUrl}api/cards`, card);
  }

  patchCard(id: string, patch: CardPatch): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}api/cards/${id}`, patch);
  }

  deleteCard(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}api/cards/${id}`);
  }
}
