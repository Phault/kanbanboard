import { EditableTextboxComponent } from '../../shared/editable-textbox/editable-textbox.component';
import { Component, Input, ViewChild } from '@angular/core';
import { DataService } from '../../shared/data.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent {

  @Input() public list: CardList;

  @ViewChild('newCard') private textbox: EditableTextboxComponent;

  constructor(private boardService: DataService) {
  }

  titleChanged(title: string) {
    this.boardService.patchList(this.list.id, {
      title: title
    }).subscribe({error: e => console.error(e)});
  }

  cardSubmitted(title: string) {
    this.boardService.createCard({ title, listId: this.list.id }).subscribe();
    this.textbox.activate();
  }
}
