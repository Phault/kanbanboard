import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../shared/profile.service';
import { Profile } from '../../shared/model/profile.interface';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  boards: Board[] = [];
  profile: Profile = { id: '', username: '', email: ''};

  constructor(private profileService: ProfileService) {
  }

  ngOnInit() {
    this.profileService.getProfile().subscribe(profile => this.profile = profile);
    this.profileService.getBoards().subscribe(boards => this.boards = boards);
  }
}
