import { Texture } from "./texture.js";

class TextureReferenceNode {
    public texutre: Texture;
    public referenceCount: number = 1;

    public constructor(texture: Texture) {
        this.texutre = texture;
    }
}

export class TextureManager {
    private static _textures: { [name: string]: TextureReferenceNode } = {};

    private constructor() {}

    public static getTexture(name: string): Texture {
        if (TextureManager._textures[name] === undefined) {
            const texture = new Texture(name);
            TextureManager._textures[name] = new TextureReferenceNode(texture);
        } else {
            TextureManager._textures[name].referenceCount += 1;
        }

        return TextureManager._textures[name].texutre;
    }

    public static releaseTexture(name: string): void {
        if (TextureManager._textures[name] === undefined) {
            console.warn(`texture ${name} does not exist.`);
        } else {
            TextureManager._textures[name].referenceCount -= 1;
            if (TextureManager._textures[name].referenceCount === 0) {
                TextureManager._textures[name].texutre.destroy();
                delete TextureManager._textures[name];
            }
        }
    }
}
