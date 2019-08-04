import { Vector2 } from "../../math/vector2.js";

export interface IShape2D {
    position: Vector2;

    setFromJson(data: any): void;
    intersects(other: IShape2D): boolean;
    pointInShape(point: Vector2): boolean;

    render(): void;
}
