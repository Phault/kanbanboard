import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DragulaService } from 'ng2-dragula';
import { DataService } from '../../shared/data.service';
import { Subscription } from 'rxjs';
import { DynamicDataService } from '../../shared/dynamic-data.service';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss']
})
export class BoardComponent implements OnInit, OnDestroy {

  id: string;
  board: Board;
  private subs = new Subscription();

  constructor(private route: ActivatedRoute,
              private dragulaService: DragulaService,
              private dataService: DataService,
              private dynamicDataService: DynamicDataService,
              private router: Router) {
  }

  ngOnInit() {
    this.dragulaService.createGroup('CARDLISTS', {
      direction: 'horizontal',
      moves: (el, _source, handle) => {
        const handles = el.getElementsByClassName('list-handle');
        return handles.length > 0 && handles[0].contains(handle);
      }
    });

    this.subs.add(this.dragulaService.dropModel('CARDLISTS')
        .subscribe(({targetModel, targetIndex, item}) => {
          item.position = this.calcPosition(targetModel, targetIndex);

          this.dataService.patchList(item.id, { position: item.position })
            .subscribe({error: e => console.error(e)});
        })
    );

    this.subs.add(this.dragulaService.dropModel('CARDS')
        .subscribe(({targetModel, targetIndex, item, target}) => {
          const newListId = target.attributes.getNamedItem('data-list-id').value;

          item.position = this.calcPosition(targetModel, targetIndex);
          item.listId = newListId;

          this.dataService.patchCard(item.id, { position: item.position, listId: newListId })
            .subscribe({error: e => console.error(e)});
        })
    );

    this.subs.add(this.dynamicDataService.onBoardDeleted.subscribe(({id}) => {
      if (id === this.board.id) {
        this.router.navigate(['/']);
      }
    }));

    this.subs.add(this.route.params.subscribe(params => {
      this.id = params['id'];

      this.dynamicDataService.getBoard(this.id)
        .subscribe(
          result => this.board = result,
          error => console.error(error)
        );
    }));
  }

  ngOnDestroy() {
    this.dragulaService.destroy('CARDLISTS');

    this.subs.unsubscribe();
  }

  calcPosition(targetArray, targetIndex): number {
    const DEFAULT_POSITION_SPACING = 65535;

    const prevNeighborPosition = targetIndex > 0
        ? targetArray[targetIndex - 1].position
        : 0;
    const nextNeighborPosition = targetIndex < targetArray.length - 1
        ? targetArray[targetIndex + 1].position
        : prevNeighborPosition + DEFAULT_POSITION_SPACING;

    // todo: if difference between neighbors is too tiny, normalize positions

    return prevNeighborPosition + (nextNeighborPosition - prevNeighborPosition) / 2;
  }

  titleChanged(title: string) {
    this.dataService.patchBoard(this.id, {
      title
    }).subscribe({error: e => console.error(e)});
  }

  newListSubmitted(title: string) {
    this.dataService.createList({
      boardId: this.id,
      title
    }).subscribe();
  }
}
