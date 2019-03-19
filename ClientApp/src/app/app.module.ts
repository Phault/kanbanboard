import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './shared/auth.guard';
import { SharedModule } from './shared/shared.module';
import { DragulaModule } from 'ng2-dragula';
import { MenubarComponent } from './menubar/menubar.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    MenubarComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'auth', loadChildren: 'app/auth/auth.module#AuthModule' },
      { path: 'boards', loadChildren: 'app/boards/boards.module#BoardsModule', canActivate: [ AuthGuard ] },
      { path: 'dashboard', loadChildren: 'app/dashboard/dashboard.module#DashboardModule', canActivate: [ AuthGuard ] },
      { path: '**', redirectTo: '' }
    ]),
    DragulaModule.forRoot(),
    SharedModule.forRoot(),
    MatButtonModule,
    MatIconModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
