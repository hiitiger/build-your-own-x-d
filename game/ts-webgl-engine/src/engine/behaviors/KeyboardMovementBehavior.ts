import { BaseBehavior } from "./basebehavior.js";
import { IBehaviorData, IBehaviorBuilder, IBehavior } from "./interface.js";
import { InputManager, Keys } from "../input/inputmanager.js";

export class KeyboardMovementBehaviorData implements IBehaviorData {
    public name: string;
    public speed: number = 0.1;
    public setFromJson(data: any): void {
        this.name = data.name;

        if ("speed" in data) {
            this.speed = data.speed;
        }
    }
}

export class KeyboardMovementBehaviorBuilder implements IBehaviorBuilder {
    public get type(): string {
        return "keyboardMovement";
    }

    public buildFromJson(json: any): IBehavior {
        const data = new KeyboardMovementBehaviorData();
        data.setFromJson(json);
        return new KeyboardMovementBehavior(json);
    }
}

export class KeyboardMovementBehavior extends BaseBehavior {
    public speed: number = 0.1;

    public constructor(data: KeyboardMovementBehaviorData) {
        super(data);
        this.speed = data.speed;
    }

    public update(time: number): void {
        if (InputManager.isKeyDown(Keys.LEFT)) {
            this._owner.transform.position.x -= this.speed;
        }
        if (InputManager.isKeyDown(Keys.RIGHT)) {
            this._owner.transform.position.x += this.speed;
        }
        if (InputManager.isKeyDown(Keys.UP)) {
            this._owner.transform.position.y -= this.speed;
        }
        if (InputManager.isKeyDown(Keys.DOWN)) {
            this._owner.transform.position.y += this.speed;
        }

        super.update(time);
    }
}
