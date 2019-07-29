import { Zone } from "./zone.js";
import { Shader } from "../gl/shader.js";
import { AssetManager, assetLoadEventName } from "../assets/assetmanager.js";
import { JsonAsset } from "../assets/jsonassetloader.js";
import { IMessageHandler } from "../message/messagehandler.js";
import { Message } from "../message/message.js";

export class ZoneManager implements IMessageHandler {
    private static _globalZoneId: number = -1;
    private static _registeredZones: { [id: number]: string } = {};
    private static _activeZone: Zone;
    private static _instance: ZoneManager;

    private constructor() {}

    public static initialize(): void {
        ZoneManager._instance = new ZoneManager();
        ZoneManager._registeredZones[0] = "assets/zones/testZone.json";
    }

    public static changeZone(id: number): void {
        if (ZoneManager._activeZone) {
            ZoneManager._activeZone.onDeactivated();
            ZoneManager._activeZone.unload();
            ZoneManager._activeZone = null;
        }
        if (id in ZoneManager._registeredZones) {
            const assetName = ZoneManager._registeredZones[id];
            if (AssetManager.isAssetLoader(assetName)) {
                const asset = AssetManager.getAsset(assetName);
                ZoneManager.loadZone(asset);
            } else {
                Message.subscribe(assetLoadEventName(assetName), ZoneManager._instance);
                AssetManager.loadAsset(assetName);
            }
        } else {
            throw new Error(`Zone id ${id} is not registered.`);
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

    private static loadZone(asset: JsonAsset): void {
        const data = asset.data;
        ["id", "name", "description"].forEach(key => {
            if (!(key in data)) {
                throw new Error(`Zone asset file unexpected error ${key} not exist.`);
            }
        });
        const zone = new Zone(data.id, data.name, data.description);
        zone.initialize(data);
        ZoneManager._activeZone = zone;
        ZoneManager._activeZone.onActivated();
        ZoneManager._activeZone.load();
    }

    public onMessage(message: Message): void {
        const asset = message.context as JsonAsset;
        ZoneManager.loadZone(asset);
    }
}
