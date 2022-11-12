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
        const energyRand = Math.random() > 0.98 ? 500 : 40;
        const powerRand = Math.random() < 0.01 ? 800 : 40;
        const voltageRand = Math.random() > 0.99 ? 600 : 40;
        if (record.auto) {
          newData = {
            panelId: record.data.panelId,
            energyGeneratedKwh: Math.round(Math.random() * energyRand + 70),
            powerGenerated: Math.round(Math.random() * powerRand + 50),
            voltageGenerated: Math.round(Math.random() * voltageRand + 30),
          };
          record.data = newData;
        } else {
          newData = { ...record.data };
          newData.energyGeneratedKwh = Math.round(newData.energyGeneratedKwh || Math.random() * energyRand);
          newData.powerGenerated = Math.round(newData.powerGenerated || Math.random() * powerRand);
          newData.voltageGenerated = Math.round(newData.voltageGenerated || Math.random() * voltageRand);
        }

        this._iotDataService.sendIotData(newData).subscribe();
      });

      this._simulateData();
    }, Math.random() * 500 + 250);
  }

}
