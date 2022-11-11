import { Component, OnDestroy, OnInit } from '@angular/core';

import { Subject, takeUntil } from 'rxjs';
import { NzMessageService } from 'ng-zorro-antd/message';

import { AppStateService } from 'src/app/services/app-state.service';
import { DashboardService } from 'src/app/services/dashboard.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {

  private _destroy$: Subject<any>;

  dashboardUrl: any;

  constructor(
    private _messageService: NzMessageService,
    private _appStateService: AppStateService,
    private _sanitizer: DomSanitizer,
    private _dashboardService: DashboardService) {
    this._destroy$ = new Subject();
    this.dashboardUrl = '';
  }

  ngOnInit(): void {
    this._setupDashboardIframe();
  }

  ngOnDestroy(): void {
    this._destroy$.next(true);
  }

  private _setupDashboardIframe() {
    this._dashboardService.getDashboardToken().subscribe(tokenModel => {
      const dashboardUrl = new URL("/embed/dashboard/" + tokenModel.token + "#refresh=1&theme=night&bordered=true&titled=true",
        tokenModel.baseUrl).toString();
      const safeDashboardUrl = this._sanitizer.bypassSecurityTrustResourceUrl(dashboardUrl);
      this.dashboardUrl = safeDashboardUrl;
    })
  }

}
