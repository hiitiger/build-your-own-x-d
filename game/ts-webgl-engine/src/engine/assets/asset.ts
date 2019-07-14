export interface IAsset {
    readonly name: string;

    readonly data: any;
}

export interface IAssetLoader {
    readonly supportedExtensions: string[];

    loadAsset(assetName: string): void;
}
