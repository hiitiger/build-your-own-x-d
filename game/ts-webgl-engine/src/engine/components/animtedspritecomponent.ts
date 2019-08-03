import { IComponentData, IComponentBuilder, IComponent } from "./interface.js";
import { SpriteComponentData } from "./spritecomponent.js";
import { BaseComponent } from "./basecomponent.js";
import { AnimatedSprite, AnimatedSpriteInfo } from "../graphics/animatedsprite.js";
import { Vector3 } from "../math/vector3.js";
import { Shader } from "../gl/shader.js";

export class AnimatedSpriteComponentData extends SpriteComponentData implements IComponentData {
    public frameWidth: number;
    public frameHeight: number;
    public frameCount: number;
    public frameSequence: number[] = [];
    public autoPlay: boolean = true;
    public frameTime: number = 33;

    public setFromJson(data: any): void {
        super.setFromJson(data);

        if ("autoPlay" in data) {
            this.autoPlay = data.autoPlay;
        }

        ["frameWidth", "frameHeight", "frameCount", "frameSequence"].forEach(key => {
            if (!(key in data)) {
                throw new Error(`AnimatedSpriteComponentData parse error: ${key} not exist`);
            }
        });

        this.frameWidth = data.frameWidth;
        this.frameHeight = data.frameHeight;
        this.frameCount = data.frameCount;
        this.frameSequence = data.frameSequence;
        if ("frameTime" in data) {
            this.frameTime = data.frameTime;
        }
    }
}

export class AnimatedSpriteComponentBuilder implements IComponentBuilder {
    public get type(): string {
        return "animatedSprite";
    }

    public buildFromJson(json: any): IComponent {
        const data = new AnimatedSpriteComponentData();
        data.setFromJson(json);
        return new AnimatedSpriteComponent(data);
    }
}

export class AnimatedSpriteComponent extends BaseComponent {
    private _sprite: AnimatedSprite;

    public constructor(data: AnimatedSpriteComponentData) {
        super(data);

        const spriteInfo = new AnimatedSpriteInfo();
        spriteInfo.name = name;
        spriteInfo.materialName = data.materialName;
        spriteInfo.frameWidth = data.frameWidth;
        spriteInfo.frameHeight = data.frameHeight;
        spriteInfo.width = data.frameWidth;
        spriteInfo.height = data.frameHeight;
        spriteInfo.frameCount = data.frameCount;
        spriteInfo.frameSequence = data.frameSequence;
        spriteInfo.frameTime = data.frameTime;

        this._sprite = new AnimatedSprite(spriteInfo);
        if (!data.origin.equals(Vector3.zero)) {
            this._sprite.origin.copyFrom(data.origin);
        }
    }

    public get isPlaying(): boolean {
        return this._sprite.isPlaying;
    }

    public load(): void {
        this._sprite.load();
    }

    public update(time: number): void {
        this._sprite.update(time);

        super.update(time);
    }

    public render(shader: Shader): void {
        super.render(shader);
        this._sprite.draw(shader, this.owner.worldMatrix);
    }
}
