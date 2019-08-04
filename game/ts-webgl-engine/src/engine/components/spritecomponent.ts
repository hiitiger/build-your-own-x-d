import { Sprite } from "../graphics/sprite.js";
import { BaseComponent } from "./basecomponent.js";
import { Shader } from "../gl/shader.js";
import { IComponentData, IComponentBuilder, IComponent } from "./interface.js";
import { Vector3 } from "../math/vector3.js";

export class SpriteComponentData implements IComponentData {
    public name: string;
    public materialName: string;
    public origin: Vector3 = Vector3.zero;
    public width: number = 100;
    public height: number = 100;

    public setFromJson(data: any): void {
        if ("name" in data) {
            this.name = data.name;
        }

        if ("materialName" in data) {
            this.materialName = data.materialName;
        }

        if ("origin" in data) {
            this.origin.setFromJson(data.origin);
        }

        if ("width" in data) {
            this.width = data.width;
        }

        if ("height" in data) {
            this.height = data.height;
        }
    }
}

export class SpriteComponentBuilder implements IComponentBuilder {
    public get type(): string {
        return "sprite";
    }

    buildFromJson(data: any): IComponent {
        const spriteData = new SpriteComponentData();
        spriteData.setFromJson(data);

        return new SpriteComponent(spriteData);
    }
}

export class SpriteComponent extends BaseComponent {
    private _sprite: Sprite;

    public constructor(data: SpriteComponentData) {
        super(data);

        this._sprite = new Sprite(data.name, data.materialName, data.width, data.height);
        if (!data.origin.equals(Vector3.zero)) {
            this._sprite.origin = data.origin;
        }
    }

    public load(): void {
        this._sprite.load();
    }

    public render(shader: Shader): void {
        super.render(shader);
        this._sprite.draw(shader, this.owner.worldMatrix);
    }
}
