import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { parseUrl } from '../helpers/url.helper';
import { DashboardTokenModel } from '../models/dashboard-token.model';
import { AppStateService } from './app-state.service';


@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(private _httpClient: HttpClient,
    private _appStateService: AppStateService) { }

  getDashboardToken() {
    const url = parseUrl('/api/dashboard/token', environment.apiUrl);
    return this._httpClient.get<DashboardTokenModel>(url);
  }
}
