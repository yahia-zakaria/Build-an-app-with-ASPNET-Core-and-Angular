import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from './../confirm-dialog/confirm-dialog.component';
import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
 modalRef: BsModalRef;

  constructor(private modalService: BsModalService ) { }

  confirm(title='Confirmation', message='Are you sure', btnConfirmText='Ok', btnCancelText='Cancel'): Observable<boolean>{
    const config = {
      initialState : {
        title,
        message,
        btnConfirmText,
        btnCancelText
      }
    }
    this.modalRef = this.modalService.show(ConfirmDialogComponent, config);
    return new Observable<boolean>(this.getResult());
  }

  private getResult(){
    return (observer)=>{
      const subscription = this.modalRef.onHidden.subscribe(()=>{
        observer.next(this.modalRef.content.result);
        observer.complete();
      });
      return {
        unsubscribe(){
          subscription.unsubscribe();
        }
      }
    }
  }
}


