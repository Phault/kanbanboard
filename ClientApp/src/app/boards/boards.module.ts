import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule, MatListModule, MatButtonModule, MatIconModule } from '@angular/material';
import { DragulaModule } from 'ng2-dragula';
import { BoardComponent } from './board/board.component';
import { RouterModule } from '@angular/router';
import { CardComponent } from './card/card.component';
import { ListComponent } from './list/list.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    MatCardModule,
    MatListModule,
    DragulaModule,
    RouterModule.forChild([
      { path: ':id', component: BoardComponent}
    ]),
    SharedModule,
    MatButtonModule,
    MatIconModule
  ],
  declarations: [
    CardComponent,
    ListComponent,
    BoardComponent
  ]
})
export class BoardsModule { }
