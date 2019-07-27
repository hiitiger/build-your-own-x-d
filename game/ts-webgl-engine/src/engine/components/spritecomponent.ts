import { Sprite } from "../graphics/sprite.js";
import { BaseComponent } from "./basecomponent.js";
import { Shader } from "../gl/shader.js";

export class SpriteComponent extends BaseComponent {
    private _sprite: Sprite;

    public constructor(name: string, materialName: string) {
        super(name);

        this._sprite = new Sprite(name, materialName);
    }

    public load(): void {
        this._sprite.load();
    }

    public render(shader: Shader): void {
        super.render(shader);
        this._sprite.draw(shader, this.owner.worldMatrix);
    }
}
