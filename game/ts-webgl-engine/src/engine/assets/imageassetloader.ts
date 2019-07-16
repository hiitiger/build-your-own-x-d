import { IAssetLoader, IAsset } from "./asset.js";
import { AssetManager } from "./assetmanager.js";

export class ImageAsset implements IAsset {
    public readonly name: string;
    public readonly data: HTMLImageElement;

    public constructor(name: string, data: HTMLImageElement) {
        this.name = name;
        this.data = data;
    }

    public get width(): number {
        return this.data.width;
    }

    public get height(): number {
        return this.data.height;
    }
}

export class ImageAssetLoader implements IAssetLoader {
    public get supportedExtensions(): string[] {
        return ["jpg", "png", "gif"];
    }

    loadAsset(assetName: string): void {
        const image = new Image();

        image.onload = this.onImageLoaded.bind(this, assetName, image);
        image.src = assetName;
    }

    private onImageLoaded(assetName: string, image: HTMLImageElement): void {
        console.log(`onImageLoaded: ${assetName}`, image);

        const asset = new ImageAsset(assetName, image);
        AssetManager.onAssetLoaded(asset);
    }
}
