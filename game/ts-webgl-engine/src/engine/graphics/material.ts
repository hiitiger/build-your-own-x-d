import { Texture } from "./texture.js";
import { Color } from "./color.js";
import { TextureManager } from "./texturemanager.js";

export class Material {
    private _name: string;
    private _diffuseTextureName: string;
    private _diffuseTexture: Texture;

    private _tint: Color;

    public constructor(name: string, diffuseTextureName: string, tint: Color) {
        this._name = name;
        this._diffuseTextureName = diffuseTextureName;
        this._tint = tint;

        this._diffuseTexture = TextureManager.getTexture(this._diffuseTextureName);
    }

    public get name(): string {
        return this._name;
    }

    public get tint(): Color {
        return this._tint;
    }

    public get diffuseTexture(): Texture {
        return this._diffuseTexture;
    }

    public get diffuseTextureName(): string {
        return this._diffuseTextureName;
    }

    public set diffuseTextureName(value: string) {
        if (this._diffuseTexture) {
            TextureManager.releaseTexture(this._diffuseTextureName);
        }

        this._diffuseTextureName = value;
        this._diffuseTexture = TextureManager.getTexture(this._diffuseTextureName);
    }

    public destroy(): void {
        TextureManager.releaseTexture(this._diffuseTextureName);
        this._diffuseTexture = undefined;
        this._diffuseTextureName = undefined;
    }
}
