import { Sprite } from "./sprite.js";
import { Vector2 } from "../math/vector2.js";
import { Message } from "../message/message.js";
import { assetLoadEventName, AssetManager } from "../assets/assetmanager.js";
import { IMessageHandler } from "../message/messagehandler.js";
import { ImageAsset } from "../assets/imageassetloader.js";
import { MaterialManager } from "./materialmanager.js";

export class AnimatedSpriteInfo {
    public name: string;

    public materialName: string;

    public width: number = 100;

    public height: number = 100;

    public frameWidth: number = 10;

    public frameHeight: number = 10;

    public frameCount: number = 1;

    public frameSequence: number[] = [];

    public frameTime: number = 60;
}

class UVInfo {
    public min: Vector2;
    public max: Vector2;

    public constructor(min: Vector2, max: Vector2) {
        this.min = min;
        this.max = max;
    }
}

export class AnimatedSprite extends Sprite implements IMessageHandler {
    private _frameHeight: number;
    private _frameWidth: number;
    private _frameCount: number;
    private _frameSequence: number[];

    private _frameTime: number = 33;

    private _currentFrame: number = 0;
    private _currentTime: number = 0;
    private _frameUVs: UVInfo[] = [];

    private _assetWidth: number;
    private _assetHeight: number;
    private _assetLoaded: boolean = false;
    private _isPlaying: boolean = true;

    public constructor(info: AnimatedSpriteInfo) {
        super(info.name, info.materialName, info.width, info.height);

        this._frameWidth = info.frameWidth;
        this._frameHeight = info.frameHeight;
        this._frameCount = info.frameCount;
        this._frameSequence = info.frameSequence;
        this._frameTime = info.frameTime;

        Message.subscribe(assetLoadEventName(this._material.diffuseTextureName), this);
    }

    public get isPlaying(): boolean {
        return this._isPlaying;
    }

    public destroy(): void {
        super.destroy();
    }

    public update(time: number): void {
        if (!this._assetLoaded) {
            if (!this._assetLoaded) {
                this.setupFromMaterial();
            }
            return;
        }

        this._currentTime += time;
        if (this._currentTime > this._frameTime) {
            this._currentFrame++;
            this._currentTime = 0;

            if (this._currentFrame >= this._frameSequence.length) {
                this._currentFrame = 0;
            }

            const frameUVs = this._frameSequence[this._currentFrame];
            this._vertices[0].texCoords.copyFrom(this._frameUVs[frameUVs].min);
            this._vertices[1].texCoords = new Vector2(this._frameUVs[frameUVs].min.x, this._frameUVs[frameUVs].max.y);
            this._vertices[2].texCoords.copyFrom(this._frameUVs[frameUVs].max);
            this._vertices[3].texCoords.copyFrom(this._frameUVs[frameUVs].max);
            this._vertices[4].texCoords = new Vector2(this._frameUVs[frameUVs].max.x, this._frameUVs[frameUVs].min.y);
            this._vertices[5].texCoords.copyFrom(this._frameUVs[frameUVs].min);

            this._buffer.clearData();
            for (const v of this._vertices) {
                this._buffer.pushBackData(v.toArray());
            }

            this._buffer.upload();
            this._buffer.unbind();
        }

        super.update(time);
    }

    public load(): void {
        super.load();
        if (!this._assetLoaded) {
            this.setupFromMaterial();
        }
    }

    private calculateUVs(): void {
        let totalWidth = 0;
        let yValue = 0;

        for (let i = 0; i < this._frameCount; ++i) {
            totalWidth += i * this._frameWidth;
            if (totalWidth > this._assetWidth) {
                yValue++;
                totalWidth = 0;
            }

            const u = (i * this._frameWidth) / this._assetWidth;
            const v = (yValue * this._frameHeight) / this._assetHeight;
            const min: Vector2 = new Vector2(u, v);

            const uMax = (i * this._frameWidth + this._frameWidth) / this._assetWidth;
            const vMax = (yValue * this._frameHeight + this._frameHeight) / this._assetHeight;
            const max: Vector2 = new Vector2(uMax, vMax);

            this._frameUVs.push(new UVInfo(min, max));
        }
    }

    public onMessage(message: Message): void {
        if (message.code === assetLoadEventName(this._material.diffuseTextureName)) {
            this._assetLoaded = true;
            const asset = message.context as ImageAsset;
            this._assetHeight = asset.height;
            this._assetWidth = asset.width;
            this.calculateUVs();
        }
    }

    private setupFromMaterial(): void {
        if (!this._assetLoaded) {
            const material = MaterialManager.getMaterial(this._material.name);
            if (material.diffuseTexture.isLoaded) {
                if (AssetManager.isAssetLoaded(material.diffuseTextureName)) {
                    this._assetHeight = material.diffuseTexture.height;
                    this._assetWidth = material.diffuseTexture.width;
                    this._assetLoaded = true;
                    this.calculateUVs();
                }
            }
        }
    }
}
