import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { CreateBoardDialog } from "./create-board-dialog/create-board-dialog.component";
import { BoardThumbnailComponent } from './board-thumbnail/board-thumbnail.component';
import { MatDialogModule, MatButtonModule, MatInputModule } from '@angular/material';
import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: 'home', component: HomeComponent },
      { path: '', redirectTo: 'home', pathMatch: 'full' }
    ]),
    SharedModule,
    FormsModule,
    MatButtonModule,
    MatInputModule,
    MatDialogModule,
  ],
  declarations: [
    HomeComponent,
    BoardThumbnailComponent,
    CreateBoardDialog
  ],
  bootstrap: [
    CreateBoardDialog
  ]
})
export class DashboardModule { }
