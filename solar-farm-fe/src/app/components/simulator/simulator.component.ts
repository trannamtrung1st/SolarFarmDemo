import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { SolarPanelData } from 'src/app/models/solar-panel-data.model';
import { SolarPanel } from 'src/app/models/solar-panel.model';
import { IotDataService } from 'src/app/services/iot-data.service';
import { PanelService } from 'src/app/services/panel.service';

@Component({
  selector: 'app-simulator',
  templateUrl: './simulator.component.html',
  styleUrls: ['./simulator.component.scss']
})
export class SimulatorComponent implements OnInit, OnDestroy {
  private _destroy$: Subject<any>;
  private _destroy: boolean;

  loading: boolean;
  panelData: {
    panel: SolarPanel,
    data: SolarPanelData,
    auto: boolean
  }[];

  constructor(private _panelService: PanelService,
    private _iotDataService: IotDataService) {
    this.panelData = [];
    this.loading = false;
    this._destroy$ = new Subject();
    this._destroy = false;
  }

  ngOnInit(): void {
    this._fetchPanels();
  }

  ngOnDestroy(): void {
    this._destroy = true;
    this._destroy$.next(true);
  }

  private _fetchPanels() {
    this._panelService.getPanels().subscribe(panels => {
      this.panelData = panels.map(p => ({
        panel: p,
        data: {
          panelId: p.id,
          energyGeneratedKwh: 0,
          powerGenerated: 0,
          voltageGenerated: 0
        },
        auto: true
      }));

      this._simulateData();
    });
  }

  private _simulateData() {
    if (this._destroy) return;
    setTimeout(() => {
      this.panelData.forEach(record => {
        let newData: SolarPanelData;
        if (record.auto) {
          newData = {
            panelId: record.data.panelId,
            energyGeneratedKwh: Math.round(Math.random() * 1000),
            powerGenerated: Math.round(Math.random() * 600),
            voltageGenerated: Math.round(Math.random() * 300),
          };
          record.data = newData;
        } else {
          newData = { ...record.data };
          newData.energyGeneratedKwh = Math.round(newData.energyGeneratedKwh || Math.random() * 1000);
          newData.powerGenerated = Math.round(newData.powerGenerated || Math.random() * 600);
          newData.voltageGenerated = Math.round(newData.voltageGenerated || Math.random() * 300);
        }

        this._iotDataService.sendIotData(newData).subscribe();
      });

      this._simulateData();
    }, Math.random() * 1500 + 500);
  }

}
