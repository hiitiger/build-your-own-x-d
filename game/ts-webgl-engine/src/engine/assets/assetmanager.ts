import { IAssetLoader, IAsset } from "./asset.js";
import { Message } from "../message/message.js";
import { ImageAssetLoader } from "./imageassetloader.js";
import { JsonAssetLoader } from "./jsonassetloader.js";

export const MESSAGE_ASSET_LOADED = "MESSAGE_ASSET_LOADED";

export function assetLoadEventName(assetName: string): string {
    return MESSAGE_ASSET_LOADED + "." + assetName;
}

export class AssetManager {
    private static _loaders: IAssetLoader[] = [];
    private static _loadedAssets: { [name: string]: IAsset } = {};

    private constructor() {}

    public static initialize(): void {
        AssetManager.registerLoader(new ImageAssetLoader());
        AssetManager.registerLoader(new JsonAssetLoader());
    }

    public static registerLoader(loader: IAssetLoader): void {
        AssetManager._loaders.push(loader);
    }

    public static onAssetLoaded(asset: IAsset): void {
        AssetManager._loadedAssets[asset.name] = asset;
        Message.send(assetLoadEventName(asset.name), this, asset);
    }

    public static loadAsset(assetName: string): void {
        const extension = assetName
            .split(".")
            .pop()
            .toLocaleLowerCase();

        for (const loader of AssetManager._loaders) {
            if (loader.supportedExtensions.indexOf(extension) !== -1) {
                loader.loadAsset(assetName);
                return;
            }
        }

        console.warn(`Cannot load asset with extension: ${extension}`);
    }

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
