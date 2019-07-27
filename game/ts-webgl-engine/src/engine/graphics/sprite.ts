import { GLBuffer, AttributeInfo } from "../gl/glbuffer.js";
import { Vector3 } from "../math/vector3.js";
import { TextureManager } from "./texturemanager.js";
import { Shader } from "../gl/shader.js";
import { gl } from "../gl/gl.js";
import { Matrix } from "../math/matrix.js";
import { Material } from "./material.js";
import { MaterialManager } from "./materialmanager.js";

export class Sprite {
    private _name: string;
    private _width: number;
    private _height: number;

    private _buffer: GLBuffer;
    private _material: Material;

    public constructor(name: string, materialName: string, width: number = 100, height: number = 100) {
        this._name = name;
        this._width = width;
        this._height = height;
        this._material = MaterialManager.getMaterial(materialName);
    }

    public get name(): string {
        return this._name;
    }

    public destroy(): void {
        this._buffer.destroy();
        MaterialManager.releaseMaterial(this._material.name);
        this._material = undefined;
    }

    public load(): void {
        this._buffer = new GLBuffer(5);

        const positionAttribute = new AttributeInfo();
        positionAttribute.location = 0;
        positionAttribute.offset = 0;
        positionAttribute.size = 3;
        this._buffer.addAttributeLocation(positionAttribute);

        const texCoordAttribute = new AttributeInfo();
        texCoordAttribute.location = 1;
        texCoordAttribute.offset = 3;
        texCoordAttribute.size = 2;
        this._buffer.addAttributeLocation(texCoordAttribute);

        // prettier-ignore
        const vertices = [
            // x,y,z, u,v
            [0,                0,       0, 0, 0],
            [0,           this._height, 0, 0, 1],
            [this._width, this._height, 0, 1, 1],

            [this._width, this._height, 0, 1, 1],
            [this._width,      0,       0, 1, 0],
            [0,                0,       0, 0, 0]
        ];
        this._buffer.pushBackData([].concat.apply([], vertices));
        this._buffer.upload();
        this._buffer.unbind();
    }

    public update(time: number): void {}

    public draw(shader: Shader, model: Matrix): void {
        gl.uniformMatrix4fv(shader.getUniformLocation("u_model"), false, model.toFloat32Array());

        gl.uniform4fv(shader.getUniformLocation("u_tint"), this._material.tint.toFloatArray());

        if (this._material.diffuseTexture) {
            this._material.diffuseTexture.activateAndBind(0);
            gl.uniform1i(shader.getUniformLocation("u_diffuse"), 0);
        }

        this._buffer.bind();
        this._buffer.draw();
    }
}
