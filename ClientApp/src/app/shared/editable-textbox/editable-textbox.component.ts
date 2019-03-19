import { Component, OnInit, Input, ViewChild, ElementRef, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-editable-textbox',
  templateUrl: './editable-textbox.component.html',
  styleUrls: ['./editable-textbox.component.scss']
})
export class EditableTextboxComponent {

  @Input() public value = '';
  @Input() public placeholder = '';
  @Input() public submitOnBlur = true;
  @Input() public rows = 1;
  @Output() public valueChange = new EventEmitter<string>();

  @ViewChild('textbox') private textbox: ElementRef;

  private active = false;
  private workingValue = '';

  constructor(private root: ElementRef) { }

  activate() {
    this.active = true;
    this.workingValue = this.value;

    this.root.nativeElement.classList.add('active');
    this.textbox.nativeElement.focus();
    this.textbox.nativeElement.selectionStart = this.textbox.nativeElement.selectionEnd = 10000;
  }

  submit() {
    if (!this.active) {
      return;
    }

    this.deactivate();

    let newVal = this.workingValue;

    if (this.rows <= 1) {
      newVal = newVal.replace(/([\r\n\t\0])/gm, '');
    }

    newVal = newVal.replace(/(\s+)/g, ' ').trim();

    if (newVal !== this.value) {
      this.valueChange.emit(newVal);
    }
  }

  deactivate() {
    this.active = false;
    this.root.nativeElement.classList.remove('active');
    this.textbox.nativeElement.blur();
  }

  workingValueChanged(newValue: string) {
    this.workingValue = newValue;
  }

  get isActive(): boolean {
    return this.active;
  }
}
