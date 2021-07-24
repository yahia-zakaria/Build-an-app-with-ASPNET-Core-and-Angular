import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Message } from './../_models/message';
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';
import { User } from '../_models/User';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { error } from 'protractor';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }

  createHubConnection(user: User, otherUser: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + "message?user=" + otherUser, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on("ReceiveMessageThread", messages => {
      this.messageThreadSource.next(messages);
    })

    this.hubConnection.on("NewMessage", message => {
      this.messageThread$.pipe(take(1)).subscribe(messages=>{
        this.messageThreadSource.next([...messages, message]);
      })
    })

  }

  stopHubConnection() {
    if (this.hubConnection)
      this.hubConnection.stop().catch(error => console.log(error))
  }

  getMessages(pageNumber, pageSize, container) {
    let params = getPaginationHeader(pageNumber, pageSize);
    params = params.append("container", container);

    return getPaginatedResult<Message[]>(this.baseUrl + "messages", params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + "messages/thread/" + username);
  }
  async addMessage(username: string, content: string) {
    return this.hubConnection.invoke("SendMessage", { RecipientUsername: username, content })
    .catch(error=>console.log(error));
  }
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + "messages/" + id);
  }
}
