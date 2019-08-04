import { GLBuffer, AttributeInfo } from "../gl/glbuffer.js";
import { Shader } from "../gl/shader.js";
import { gl } from "../gl/gl.js";
import { Matrix } from "../math/matrix.js";
import { Material } from "./material.js";
import { MaterialManager } from "./materialmanager.js";
import { Vertex } from "./vertex.js";
import { Vector3 } from "../math/vector3.js";

export class Sprite {
    protected _name: string;
    protected _width: number;
    protected _height: number;
    protected _origin: Vector3 = Vector3.zero;

    protected _buffer: GLBuffer;
    protected _material: Material;
    protected _vertices: Vertex[] = [];

    public constructor(name: string, materialName: string, width: number = 100, height: number = 100) {
        this._name = name;
        this._width = width;
        this._height = height;
        this._material = MaterialManager.getMaterial(materialName);
    }

    public get name(): string {
        return this._name;
    }

    public get origin(): Vector3 {
        return this._origin;
    }

    public set origin(value: Vector3) {
        this._origin = value;
        this.recalculateVertices();
    }

    public destroy(): void {
        this._buffer.destroy();
        MaterialManager.releaseMaterial(this._material.name);
        this._material = undefined;
    }

    public load(): void {
        this._buffer = new GLBuffer();

        const positionAttribute = new AttributeInfo();
        positionAttribute.location = 0;
        positionAttribute.size = 3;
        this._buffer.addAttributeLocation(positionAttribute);

        const texCoordAttribute = new AttributeInfo();
        texCoordAttribute.location = 1;
        texCoordAttribute.size = 2;
        this._buffer.addAttributeLocation(texCoordAttribute);

        const minX = -(this._width * this._origin.x);
        const maxX = this._width * (1.0 - this._origin.x);

        const minY = -(this._height * this._origin.y);
        const maxY = this._height * (1.0 - this._origin.y);
        // prettier-ignore
        const vertices = [
            // x,y,z, u,v
            [minX,          minY,       0, 0, 0],
            [minX,          maxY,       0, 0, 1],
            [maxX,          maxY,       0, 1, 1],

            [maxX,          maxY,       0, 1, 1],
            [maxX,          minY,       0, 1, 0],
            [minX,          minY,       0, 0, 0],
        ];

        this._vertices = vertices.map(v => new Vertex(...v));
        this._buffer.setData([].concat.apply([], vertices));
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

    protected recalculateVertices(): void {
        if (this._vertices.length !== 0) {
            const minX = -(this._width * this._origin.x);
            const maxX = this._width * (1.0 - this._origin.x);

            const minY = -(this._height * this._origin.y);
            const maxY = this._height * (1.0 - this._origin.y);

            this._vertices[0].position.set(minX, minY);
            this._vertices[1].position.set(minX, maxY);
            this._vertices[2].position.set(maxX, maxY);

            this._vertices[3].position.set(maxX, maxY);
            this._vertices[4].position.set(maxX, minY);
            this._vertices[5].position.set(minX, minY);

            this._buffer.clearData();
            for (const v of this._vertices) {
                this._buffer.pushBackData(v.toArray());
            }

            this._buffer.upload();
            this._buffer.unbind();
        }
    }
}
