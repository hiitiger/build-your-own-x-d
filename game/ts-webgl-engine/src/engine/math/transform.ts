import { Vector3 } from "./vector3.js";
import { Matrix } from "./matrix.js";

export class Transform {
    public position: Vector3 = Vector3.zero;
    public rotation: Vector3 = Vector3.zero;
    public scale: Vector3 = Vector3.one;

    public copyFrom(other: Transform): void {
        this.position.copyFrom(other.position);
        this.rotation.copyFrom(other.rotation);
        this.scale.copyFrom(other.scale);
    }

    public setFromJson(data: any): void {
        if (data.position) {
            this.position.setFromJson(data.position);
        }
        if (data.rotation) {
            this.rotation.setFromJson(data.rotation);
        }
        if (data.scale) {
            this.scale.setFromJson(data.scale);
        }
    }

    public getTransformationMatrix(): Matrix {
        const translation = Matrix.translation(this.position);
        const rotation = Matrix.rotationXYZ(this.rotation.x, this.rotation.y, this.rotation.z);
        const scale = Matrix.scale(this.scale);

        return Matrix.multiply(Matrix.multiply(translation, rotation), scale);
    }
}
