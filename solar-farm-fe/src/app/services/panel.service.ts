import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { parseUrl } from '../helpers/url.helper';
import { SolarPanel } from '../models/solar-panel.model';
import { AppStateService } from './app-state.service';


@Injectable({
  providedIn: 'root'
})
export class PanelService {

  constructor(private _httpClient: HttpClient,
    private _appStateService: AppStateService) { }

  getPanels() {
    const url = parseUrl('/api/panels', environment.apiUrl);
    return this._httpClient.get<SolarPanel[]>(url);
  }
}
