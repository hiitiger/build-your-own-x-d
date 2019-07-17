import { gl } from "../gl/gl.js";
import { Message } from "../message/message.js";
import { assetLoadEventName, AssetManager } from "../assets/assetmanager.js";
import { IMessageHandler } from "../message/messagehandler.js";
import { ImageAsset } from "../assets/imageassetloader.js";

const LEVEL: number = 0;
const BORDER: number = 0;
const TEMP_IMAGE_DATA: Uint8Array = new Uint8Array([255, 255, 255, 255]);

export class Texture implements IMessageHandler {
    private _name: string;
    private _handler: WebGLTexture;
    private _isloaded: boolean = false;
    private _width: number;
    private _height: number;

    public constructor(name: string, width: number = 1, height: number = 1) {
        this._name = name;
        this._width = width;
        this._height = height;
        this._handler = gl.createTexture();

        this.bind();
        gl.texImage2D(gl.TEXTURE_2D, LEVEL, gl.RGBA, 1, 1, BORDER, gl.RGBA, gl.UNSIGNED_BYTE, TEMP_IMAGE_DATA);

        const asset = AssetManager.getAsset(this.name) as ImageAsset;
        if (asset !== null) {
            this.loadTextureFromAsset(asset);
        } else {
            Message.subscribe(assetLoadEventName(name), this);
        }
    }

    public destroy(): void {
        gl.deleteTexture(this._handler);
    }

    public get name(): string {
        return this._name;
    }

    public get isloaded(): boolean {
        return this._isloaded;
    }

    public get width(): number {
        return this._width;
    }

    public get height(): number {
        return this._height;
    }

    public activateAndBind(textureUnit: number = 0): void {
        gl.activeTexture(gl.TEXTURE0 + textureUnit);

        this.bind();
    }

    public bind(): void {
        gl.bindTexture(gl.TEXTURE_2D, this._handler);
    }

    public unbind(): void {
        gl.bindTexture(gl.TEXTURE_2D, null);
    }

    public onMessage(message: Message): void {
        if (message.code === assetLoadEventName(this.name)) {
            this.loadTextureFromAsset(message.context as ImageAsset);
        }
    }

    private loadTextureFromAsset(asset: ImageAsset): void {
        this._width = asset.width;
        this._height = asset.height;

        this.bind();

        gl.texImage2D(gl.TEXTURE_2D, LEVEL, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, asset.data);

        if (this.isPowerOf2()) {
            gl.generateMipmap(gl.TEXTURE_2D);
        } else {
            //clamp wrapping to edge
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        }

        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);

        this._isloaded = true;
    }

    private isPowerOf2(): boolean {
        return this.isValuePowerOf2(this._width) && this.isValuePowerOf2(this._height);
    }

    private isValuePowerOf2(value: number): boolean {
        return (value & (value - 1)) === 0;
    }
}
