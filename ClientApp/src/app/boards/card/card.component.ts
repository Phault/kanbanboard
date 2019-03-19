import { Component, Input } from '@angular/core';
import { DataService } from '../../shared/data.service';

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss']
})
export class CardComponent {

  @Input() card: Card;

  constructor(private boardService: DataService) {
  }

  titleChanged(title: string) {
    this.boardService.patchCard(this.card.id, {
      title: title
    }).subscribe({ error: e => console.error(e) });
  }

}
