import { IAssetLoader, IAsset } from "./asset";

export class AssetManager {
    private static _loaders: IAssetLoader[] = [];
    private static _loadedAssets: { [name: string]: IAsset } = {};

    private constructor() {}

    public static initialize(): void {}

    public static registerLoader(loader: IAssetLoader): void {
        AssetManager._loaders.push(loader);
    }

    public static loadAsset(assetName: string): void {}

    public static isAssetLoader(assetName: string): boolean {
        return AssetManager._loadedAssets[assetName] !== undefined;
    }

    public static getAsset(assetName: string): IAsset {
        if (AssetManager._loadedAssets[assetName] !== undefined) {
            return AssetManager._loadedAssets[assetName];
        } else {
            AssetManager.loadAsset(assetName);
            return null;
        }
    }
}
