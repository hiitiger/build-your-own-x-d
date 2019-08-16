import { IComponentData, IComponentBuilder, IComponent } from "./interface.js";
import { BaseComponent } from "./basecomponent.js";
import { IShape2D } from "../graphics/shapes2d/ishape2d.js";
import { Rectangle2D } from "../graphics/shapes2d/rectangle2d.js";
import { Circle2D } from "../graphics/shapes2d/circle2d.js";
import { Shader } from "../gl/shader.js";
import { CollisionManager } from "../collision/collisionmanager.js";

export class CollisionComponentData implements IComponentData {
    public name: string;
    public shape: IShape2D;
    public static: boolean = true;

    public setFromJson(data: any): void {
        if ("name" in data) {
            this.name = data.name;
        }

        if ("static" in data) {
            this.static = data.static;
        }

        if (!("shape" in data)) {
            throw new Error();
        }

        switch (data.shape.type) {
            case "rectangle":
                {
                    this.shape = new Rectangle2D();
                    this.shape.setFromJson(data.shape);
                }
                break;
            case "circle":
                {
                    this.shape = new Circle2D();
                    this.shape.setFromJson(data.shape);
                }
                break;
            default:
                throw new Error(`Unsupported shape ${data.shape.type}`);
                break;
        }
    }
}

export class CollisionComponentBuilder implements IComponentBuilder {
    public get type(): string {
        return "collision";
    }

    public buildFromJson(json: any): IComponent {
        const data = new CollisionComponentData();
        data.setFromJson(json);
        return new CollisionComponent(data);
    }
}

export class CollisionComponent extends BaseComponent {
    private _shape: IShape2D;
    private _static: boolean = true;

    public constructor(data: CollisionComponentData) {
        super(data);
        this._shape = data.shape;
        this._static = data.static;
    }

    public get shape(): IShape2D {
        return this._shape;
    }

    public get static(): boolean {
        return this._static;
    }

    public load(): void {
        super.load();

        this._shape.position.copyFrom(
            this.owner
                .getWorldPosition()
                .toVector2()
                .add(this._shape.offset)
        );

        CollisionManager.registerCollisionComponent(this);
    }

    public update(time: number): void {
        super.update(time);

        this._shape.position.copyFrom(
            this.owner
                .getWorldPosition()
                .toVector2()
                .add(this._shape.offset)
        );
    }

    public render(shader: Shader): void {
        super.render(shader);

        this._shape.render();
    }

    public onCollisionExit(other: CollisionComponent): void {
        // console.log(`onCollisionExit`);
    }

    public onCollisionEnter(other: CollisionComponent): void {
        // console.log(`onCollisionEnter`);
    }

    public onCollisionUpdate(other: CollisionComponent): void {
        // console.log(`onCollisionUpdate`);
    }
}
