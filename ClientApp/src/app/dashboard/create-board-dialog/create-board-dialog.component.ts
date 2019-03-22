import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { DialogData } from "./dialog-data.interface";
@Component({
  selector: 'app-create-board-dialog',
  templateUrl: './create-board-dialog.component.html',
  styleUrls: ['./create-board-dialog.component.scss']
})
export class CreateBoardDialog {
  constructor(public dialogRef: MatDialogRef<CreateBoardDialog>,
    @Inject(MAT_DIALOG_DATA)
    public data: DialogData) {
  }
}
