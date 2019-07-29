import { IAssetLoader, IAsset } from "./asset.js";
import { AssetManager } from "./assetmanager.js";

export class JsonAsset implements IAsset {
    public readonly name: string;
    public readonly data: any;

    public constructor(name: string, data: any) {
        this.name = name;
        this.data = data;
    }
}

export class JsonAssetLoader implements IAssetLoader {
    public get supportedExtensions(): string[] {
        return ["json"];
    }

    loadAsset(assetName: string): void {
        fetch(assetName)
            .then(response => response.json())
            .then(this.onJsonLoaded.bind(this, assetName))
            .catch(err => console.error(`load jons asset  ${assetName} failed`, err));
    }

    private onJsonLoaded(assetName: string, json: any): void {
        console.log(`onJsonLoaded: ${assetName}`);

        const asset = new JsonAsset(assetName, json);
        AssetManager.onAssetLoaded(asset);
    }
}
