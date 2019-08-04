import { IShape2D } from "./ishape2d.js";
import { Vector2 } from "../../math/vector2.js";
import { Circle2D } from "./circle2d.js";
import { gl2d } from "../../gl/gl.js";

export class Rectangle2D implements IShape2D {
    public position: Vector2 = Vector2.zero;

    public width: number;
    public height: number;

    public setFromJson(data: any): void {
        if ("position" in data) {
            this.position.setFromJson(data.position);
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
            if (
                this.pointInShape(other.position) ||
                this.pointInShape(new Vector2(other.position.x + other.width, other.position.y)) ||
                this.pointInShape(new Vector2(other.position.x + other.width, other.position.y + other.height)) ||
                this.pointInShape(new Vector2(other.position.x, other.position.y + other.height))
            ) {
                return true;
            }
        } else if (other instanceof Circle2D) {
            if (
                other.pointInShape(this.position) ||
                other.pointInShape(new Vector2(this.position.x + this.width, this.position.y)) ||
                other.pointInShape(new Vector2(this.position.x + this.width, this.position.y + this.height)) ||
                other.pointInShape(new Vector2(this.position.x, this.position.y + this.height))
            ) {
                return true;
            }
        }

        return false;
    }

    public pointInShape(point: Vector2): boolean {
        if (point.x >= this.position.x && point.x < this.position.x + this.width) {
            if (point.y >= this.position.y && point.y < this.position.y + this.height) {
                return true;
            }
        }

        return false;
    }

    public render(): void {
        gl2d.strokeStyle = "rgba(255, 50, 255, 0.5)";
        gl2d.strokeRect(this.position.x, this.position.y, this.width, this.height);
    }
}
