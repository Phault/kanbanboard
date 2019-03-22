import { Component, OnInit, Input, ElementRef, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-board-thumbnail',
  templateUrl: './board-thumbnail.component.html',
  styleUrls: ['./board-thumbnail.component.scss']
})
export class BoardThumbnailComponent implements OnChanges {
  
  @Input() public board: Board;
  
  constructor(private hostRef: ElementRef) { }
  
  ngOnChanges(_changes: SimpleChanges): void {
    const hostElement = this.hostRef.nativeElement;
    const background = this.board.background;
    const isUrl = background.includes('.');

    if (isUrl) {
      const isAbsolute = background.startsWith('http');
      const finalUrl = isAbsolute ? background : `/assets/backgrounds/${background}`;
      hostElement.style['background-image'] = `url(${finalUrl})`;
    }
    else
      hostElement.style['background-color'] = background;
  }
}
