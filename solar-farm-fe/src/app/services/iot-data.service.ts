import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { parseUrl } from '../helpers/url.helper';
import { SolarPanelData } from '../models/solar-panel-data.model';
import { AppStateService } from './app-state.service';


@Injectable({
  providedIn: 'root'
})
export class IotDataService {

  constructor(private _httpClient: HttpClient,
    private _appStateService: AppStateService) { }

  sendIotData(solarPanelData: SolarPanelData) {
    const url = parseUrl('/api/iot-data', environment.apiUrl);
    return this._httpClient.post(url, solarPanelData);
  }
}
