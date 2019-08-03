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
        if (data.x) {
            this._x = data.x;
        }
        if (data.y) {
            this._y = data.y;
        }
        if (data.z) {
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

    public device(v: Vector3): Vector3 {
        this._x /= v._x;
        this._y /= v._y;
        this._z /= v._z;
        return this;
    }

    public equals(other: Vector3): boolean {
        return this._x === other._x && this._y === other._y && this._z === other._z;
    }
}
