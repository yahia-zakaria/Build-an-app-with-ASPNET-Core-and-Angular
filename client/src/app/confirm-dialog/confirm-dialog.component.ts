import { BsModalRef } from 'ngx-bootstrap/modal';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent implements OnInit {
  title: string;
  message: string;
  btnConfirmText: string;
  btnCancelText: string;
  result: boolean;

  constructor(public modalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  confirm() {
    this.result = true;
    this.modalRef.hide();
  }

  decline() {
    this.result = false;
    this.modalRef.hide();
  }

}
