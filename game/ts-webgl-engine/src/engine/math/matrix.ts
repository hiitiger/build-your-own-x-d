import { Vector3 } from "./vector3";

export class Matrix {
    private _data: number[] = [];

    private constructor() {
        // prettier-ignore
        this._data = [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1,
        ];
    }

    public get data(): number[] {
        return this._data;
    }

    public static identity(): Matrix {
        return new Matrix();
    }

    /**
     *
     * [
     *   2/(r-l)    0       0   -(r+l)/(r-l)
     *
     *      0    2/(t-b)    0   -(t+b)/(t-b)
     *
     *      0       0    -2/(f-n)   0
     *
     *      0       0       0       1        ]
     *
     */

    public static orthographic(
        left: number,
        right: number,
        bottom: number,
        top: number,
        near: number,
        far: number
    ): Matrix {
        const m = new Matrix();

        // prettier-ignore
        m._data =   [
               2/(right-left),                  0,                      0,              0,
                    0,                  2/(top-bottom),                 0,              0,
                    0,                          0,                  -2/(far-near),      0,
        -(right+left)/(right-left), -(top+bottom)/(top-bottom), -(far+near)/(far-near), 1 ];

        return m;
    }

    public static translation(position: Vector3): Matrix {
        const m = new Matrix();

        m._data[12] = position.x;
        m._data[13] = position.y;
        m._data[14] = position.z;

        return m;
    }

    /**
     *
     */
    public static perspective(fov: number, aspect: number, near: number, far: number): Matrix {
        const m = new Matrix();
        return m;
    }
}
