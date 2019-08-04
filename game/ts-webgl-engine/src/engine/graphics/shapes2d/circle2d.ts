import { IShape2D } from "./ishape2d.js";
import { Vector2 } from "../../math/vector2.js";
import { Rectangle2D } from "./rectangle2d.js";
import { gl2d } from "../../gl/gl.js";

export class Circle2D implements IShape2D {
    public position: Vector2 = Vector2.zero;
    public radius: number;

    public setFromJson(data: any): void {
        if ("position" in data) {
            this.position.setFromJson(data.position);
        }

        if (!("radius" in data)) {
            throw new Error(`Circle2D parse error: radius not exist`);
        }

        this.radius = data.radius;
    }

    public intersects(other: IShape2D): boolean {
        if (other instanceof Circle2D) {
            const distance = Math.abs(Vector2.distance(this.position, other.position));
            return distance <= this.radius + other.radius;
        } else if (other instanceof Rectangle2D) {
            return other.intersects(this);
        }

        return false;
    }

    public pointInShape(point: Vector2): boolean {
        const distance = Math.abs(Vector2.distance(this.position, point));
        return distance <= this.radius;
    }

    public render(): void {
        gl2d.strokeStyle = "rgba(255, 50, 255, 0.5)";
        gl2d.beginPath();
        gl2d.arc(this.position.x, this.position.y, this.radius, 0, 2 * Math.PI);
        gl2d.stroke();
    }
}
