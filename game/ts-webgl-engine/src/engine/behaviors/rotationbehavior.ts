import { BaseBehavior } from "./basebehavior.js";
import { IBehaviorData, IBehaviorBuilder, IBehavior } from "./interface.js";
import { Vector3 } from "../math/vector3.js";

export class RotationBehaviorData implements IBehaviorData {
    public name: string;
    public rotation: Vector3 = Vector3.zero;
    setFromJson(data: any): void {
        if ("name" in data) {
            this.name = data.name;
        }

        if ("rotation" in data) {
            this.rotation.setFromJson(data.rotation);
        }
    }
}

export class RotationBehaviorBuilder implements IBehaviorBuilder {
    public get type(): string {
        return "rotation";
    }

    public buildFromJson(json: any): IBehavior {
        const data = new RotationBehaviorData();
        data.setFromJson(json);
        return new RotationBehavior(data);
    }
}

export class RotationBehavior extends BaseBehavior {
    private _rotation: Vector3;

    public constructor(data: RotationBehaviorData) {
        super(data);
        this._rotation = data.rotation;
    }

    public update(time: number): void {
        super.update(time);
        this._owner.transform.rotation.add(this._rotation);
    }
}
