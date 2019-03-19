import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Profile } from './model/profile.interface';

@Injectable()
export class ProfileService {

  constructor(private http: HttpClient,
              @Inject('BASE_URL') private baseUrl: string) { }

  getProfile(): Observable<Profile> {
    return this.http.get<Profile>(`${this.baseUrl}api/profile`);
  }

  getBoards(): Observable<Board[]> {
    return this.http.get<Board[]>(`${this.baseUrl}api/profile/boards`);
  }
}
