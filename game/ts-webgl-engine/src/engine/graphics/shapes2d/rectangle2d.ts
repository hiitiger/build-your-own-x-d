import { IShape2D } from "./ishape2d.js";
import { Vector2 } from "../../math/vector2.js";
import { Circle2D } from "./circle2d.js";
import { gl2d } from "../../gl/gl.js";

export class Rectangle2D implements IShape2D {
    public position: Vector2 = Vector2.zero;
    public origin: Vector2 = Vector2.zero;

    public width: number;
    public height: number;

    public get offset(): Vector2 {
        return new Vector2(-this.width * this.origin.x, -this.height * this.origin.y);
    }

    public setFromJson(data: any): void {
        if ("position" in data) {
            this.position.setFromJson(data.position);
        }

        if ("origin" in data) {
            this.origin.setFromJson(data.origin);
        }

        ["width", "height"]
            .filter(v => !(v in data))
            .forEach(key => {
                throw new Error(`Rectangle2D parse error: ${key} not exist`);
            });

        this.width = data.width;
        this.height = data.height;
    }

    public intersects(other: IShape2D): boolean {
        if (other instanceof Rectangle2D) {
            return (
                this.pointInShape(other.position) ||
                this.pointInShape(new Vector2(other.position.x + other.width, other.position.y)) ||
                this.pointInShape(new Vector2(other.position.x + other.width, other.position.y + other.height)) ||
                this.pointInShape(new Vector2(other.position.x, other.position.y + other.height))
            );
        } else if (other instanceof Circle2D) {
            const deltaX =
                other.position.x - Math.max(this.position.x, Math.min(other.position.x, this.position.x + this.width));
            const deltaY =
                other.position.y - Math.max(this.position.y, Math.min(other.position.y, this.position.y + this.height));
            return deltaX * deltaX + deltaY * deltaY < other.radius * other.radius;
        }

        return false;
    }

    public pointInShape(point: Vector2): boolean {
        const x = this.width < 0 ? this.position.x - this.width : this.position.x;
        const y = this.height < 0 ? this.position.y - this.height : this.position.y;

        const extentX = this.width < 0 ? this.position.x : this.position.x + this.width;
        const extentY = this.height < 0 ? this.position.y : this.position.y + this.height;

        if (point.x >= x && point.x < extentX && point.y >= y && point.y < extentY) {
            return true;
        }

        return false;
    }
    y;
    public render(): void {
        gl2d.strokeStyle = "rgba(255, 50, 255, 1.0)";
        gl2d.strokeRect(this.position.x, this.position.y, this.width, this.height);
    }
}
