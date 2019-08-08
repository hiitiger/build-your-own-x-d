import { Vector2 } from "./vector2.js";

export class Vector3 {
    private _x: number;
    private _y: number;
    private _z: number;

    public constructor(x: number = 0, y: number = 0, z: number = 0) {
        this._x = x;
        this._y = y;
        this._z = z;
    }

    public get x(): number {
        return this._x;
    }

    public set x(value: number) {
        this._x = value;
    }

    public get y(): number {
        return this._y;
    }

    public set y(value: number) {
        this._y = value;
    }

    public get z(): number {
        return this._z;
    }

    public set z(value: number) {
        this._z = value;
    }

    public set(x?: number, y?: number, z?: number): void {
        if (x !== undefined) {
            this._x = x;
        }

        if (y !== undefined) {
            this._y = y;
        }

        if (z !== undefined) {
            this._z = z;
        }
    }

    public toArray(): number[] {
        return [this._x, this._y, this._z];
    }

    public toFloat32Array(): Float32Array {
        return new Float32Array(this.toArray());
    }

    public copyFrom(other: Vector3): void {
        this._x = other._x;
        this._y = other._y;
        this._z = other._z;
    }

    public setFromJson(data: any): void {
        if ("x" in data) {
            this._x = data.x;
        }
        if ("y" in data) {
            this._y = data.y;
        }
        if ("z" in data) {
            this._z = data.z;
        }
    }

    public static get zero(): Vector3 {
        return new Vector3();
    }

    public static get one(): Vector3 {
        return new Vector3(1, 1, 1);
    }

    public add(v: Vector3): Vector3 {
        this._x += v._x;
        this._y += v._y;
        this._z += v._z;
        return this;
    }

    public subtract(v: Vector3): Vector3 {
        this._x -= v._x;
        this._y -= v._y;
        this._z -= v._z;
        return this;
    }

    public multiply(v: Vector3): Vector3 {
        this._x *= v._x;
        this._y *= v._y;
        this._z *= v._z;
        return this;
    }

    public devide(v: Vector3): Vector3 {
        this._x /= v._x;
        this._y /= v._y;
        this._z /= v._z;
        return this;
    }

    public equals(other: Vector3): boolean {
        return this._x === other._x && this._y === other._y && this._z === other._z;
    }

    public clone(): Vector3 {
        return new Vector3(this._x, this._y, this._z);
    }

    public toVector2(): Vector2 {
        return new Vector2(this._x, this._y);
    }

    public static distance(a: Vector3, b: Vector3): number {
        const diff = a.subtract(b);
        return Math.sqrt(diff.x * diff.x + diff.y * diff.y + diff.z * diff.z);
    }
}
