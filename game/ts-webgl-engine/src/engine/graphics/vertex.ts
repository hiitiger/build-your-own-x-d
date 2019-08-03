import { Vector3 } from "../math/vector3.js";
import { Vector2 } from "../math/vector2.js";

export class Vertex {
    public position: Vector3 = Vector3.zero;

    public texCoords: Vector2 = Vector2.zero;

    public constructor(x: number = 0, y: number = 0, z: number = 0, tu: number = 0, tv: number = 0) {
        this.position.x = x;
        this.position.y = y;
        this.position.z = z;

        this.texCoords.x = tu;
        this.texCoords.y = tv;
    }

    public toArray(): number[] {
        let array: number[] = [];

        array = array.concat(this.position.toArray());
        array = array.concat(this.texCoords.toArray());

        return array;
    }

    public toFloat32Array(): Float32Array {
        return new Float32Array(this.toArray());
    }
}
