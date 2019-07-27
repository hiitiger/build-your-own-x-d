import { Zone } from "./zone.js";
import { Shader } from "../gl/shader.js";
import { TestZone } from "../testZone.js";

export class ZoneManager {
    private static _globalZoneId: number = -1;
    private static _zones: { [id: number]: Zone } = {};
    private static _activeZone: Zone;

    private constructor() {}

    public static createTestZone(): number {
        const id = ++ZoneManager._globalZoneId;
        const zone = new TestZone(id);
        ZoneManager._zones[id] = zone;
        return id;
    }

    public static createZone(name: string, description: string): number {
        const id = ++ZoneManager._globalZoneId;
        const zone = new Zone(id, name, description);
        ZoneManager._zones[id] = zone;
        return id;
    }

    public static changeZone(id: number): void {
        if (ZoneManager._activeZone) {
            ZoneManager._activeZone.onDeactivated();
            ZoneManager._activeZone.unload();
        }
        if (id in this._zones) {
            ZoneManager._activeZone = this._zones[id];
            ZoneManager._activeZone.onActivated();
            ZoneManager._activeZone.load();
        }
    }

    public static update(time: number): void {
        if (ZoneManager._activeZone) {
            ZoneManager._activeZone.update(time);
        }
    }

    public static render(shader: Shader): void {
        if (ZoneManager._activeZone) {
            ZoneManager._activeZone.render(shader);
        }
    }
}
