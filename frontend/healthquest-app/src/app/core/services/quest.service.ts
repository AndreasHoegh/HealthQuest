import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Quest } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class QuestService {
  private apiUrl = 'http://localhost:5000/api/Quests';

  constructor(private http: HttpClient) {}

  getDailyQuests(): Observable<Quest[]> {
    return this.http.get<Quest[]>(`${this.apiUrl}/daily`);
  }

  generateDailyQuests(): Observable<any> {
    return this.http.post(`${this.apiUrl}/generate`, {});
  }

  completeQuest(questId: string, verificationData?: string): Observable<Quest> {
    return this.http.post<Quest>(`${this.apiUrl}/${questId}/complete`, {
      verificationData
    });
  }
}
