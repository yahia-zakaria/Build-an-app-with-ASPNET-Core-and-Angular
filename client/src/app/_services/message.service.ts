import { Message } from './../_models/message';
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMessages(pageNumber, pageSize, container){
    let params = getPaginationHeader(pageNumber, pageSize);
    params = params.append("container", container);

    return getPaginatedResult<Message[]>(this.baseUrl + "messages", params, this.http);
  }

  getMessageThread(username : string){
    return this.http.get<Message[]>(this.baseUrl + "messages/thread/" + username);
  }
  addMessage(username : string, content: string){
    return this.http.post<Message>(this.baseUrl + "messages", {RecipientUsername : username, content});
  }
  deleteMessage(id: number){
    return this.http.delete(this.baseUrl + "messages/" + id);
  }
}
