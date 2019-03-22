import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../shared/profile.service';
import { MatDialog } from '@angular/material';
import { CreateBoardDialog } from '../create-board-dialog/create-board-dialog.component';
import { DataService } from '../../shared/data.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  boards: Board[] = [];

  constructor(private profileService: ProfileService, private dataService: DataService, private dialog: MatDialog) {
  }

  ngOnInit() {
    this.refreshBoards();
  }

  refreshBoards(): void {
    this.profileService.getBoards().subscribe(boards => this.boards = boards);
  }

  createBoard(): void {
    const dialogRef = this.dialog.open(CreateBoardDialog, {
      width: '400px',
      data: {
        name: '',
        background: '#a8a8a8'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.dataService.createBoard({
          title: result.name,
          background: result.background
        }).subscribe(_board => {
          this.refreshBoards();
        });
      }
    });
  }
}
