import { Sprite } from "../graphics/sprite.js";
import { BaseComponent } from "./basecomponent.js";
import { Shader } from "../gl/shader.js";
import { IComponentData, IComponentBuilder, IComponent } from "./interface.js";

export class SpriteComponentData implements IComponentData {
    public name: string;
    public materialName: string;

    public setFromJson(data: any): void {
        if ("name" in data) {
            this.name = data.name;
        }

        if ("materialName" in data) {
            this.materialName = data.materialName;
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

        this._sprite = new Sprite(data.name, data.materialName);
    }

    public load(): void {
        this._sprite.load();
    }

    public render(shader: Shader): void {
        super.render(shader);
        this._sprite.draw(shader, this.owner.worldMatrix);
    }
}
