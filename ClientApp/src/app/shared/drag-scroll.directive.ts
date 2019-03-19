import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appDragScroll]'
})
export class DragScrollDirective {
  isDragging = false;
  constructor(private element: ElementRef) {
  }

  @HostListener('mousedown', ['$event']) onMouseDown(e) {
    if (e.which !== 1 || !this.canDrag(e.target)) {
      return;
    }

    this.isDragging = true;
  }

  @HostListener('document:mouseup', ['$event']) onMouseUp(e) {
    if (e.which !== 1 || !this.isDragging) {
      return;
    }

    this.isDragging = false;

    e.preventDefault();
  }

  @HostListener('document:mousemove', ['$event']) onMouseMove(e) {
    if (e.which !== 1 || !this.isDragging) {
      return;
    }

    this.element.nativeElement.scrollBy(-e.movementX, -e.movementY);
  }

  private canDrag(target: Element): boolean {
    while (target !== null) {
      if (target.isSameNode(this.element.nativeElement)) {
        return true;
      }

      if (target.classList.contains('no-drag-scroll')) {
        return false;
      }

      target = target.parentElement;
    }

    return false;
  }

}
