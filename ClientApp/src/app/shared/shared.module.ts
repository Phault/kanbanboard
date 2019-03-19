import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EditableTextboxComponent } from './editable-textbox/editable-textbox.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ContentTypeHeaderInterceptor } from './content-type-header.interceptor';
import { DataService } from './data.service';
import { DynamicDataService } from './dynamic-data.service';
import { ProfileService } from './profile.service';
import { FormsModule } from '@angular/forms';
import { AuthService } from './auth.service';
import { AuthHeaderInterceptor } from './auth-header.interceptor';
import { DragScrollDirective } from './drag-scroll.directive';

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule
  ],
  declarations: [
    EditableTextboxComponent,
    DragScrollDirective
  ],
  exports: [
    EditableTextboxComponent,
    DragScrollDirective
  ]
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [
        DataService,
        DynamicDataService,
        ProfileService,
        AuthService,
        { provide: HTTP_INTERCEPTORS, useClass: ContentTypeHeaderInterceptor, multi: true},
        { provide: HTTP_INTERCEPTORS, useClass: AuthHeaderInterceptor, multi: true}
      ]
    };
  }
}
