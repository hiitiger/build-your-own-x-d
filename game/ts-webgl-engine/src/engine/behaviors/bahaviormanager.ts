import { IBehaviorBuilder, IBehavior } from "./interface.js";
import { RotationBehaviorBuilder } from "./rotationbehavior.js";
import { KeyboardMovementBehaviorBuilder } from "./keyboardmovementbehavior.js";
import { PlayerBehaviorBuilder } from "./playerbehavior.js";
import { ScrollBehaviorBuilder } from "./scrollbahavior.js";

export class BehaviorManager {
    private static _registeredBuilders: { [type: string]: IBehaviorBuilder } = {};

    public static registerBuilder(builder: IBehaviorBuilder): void {
        BehaviorManager._registeredBuilders[builder.type] = builder;
    }

    public static extractBehavior(data: any): IBehavior {
        if ("type" in data) {
            if (BehaviorManager._registeredBuilders[data.type]) {
                return BehaviorManager._registeredBuilders[data.type].buildFromJson(data);
            }
        }
        throw new Error("extractBehavior failed: type is missing or builder is not registered for this type.");
    }
}

BehaviorManager.registerBuilder(new RotationBehaviorBuilder());
BehaviorManager.registerBuilder(new KeyboardMovementBehaviorBuilder());
BehaviorManager.registerBuilder(new PlayerBehaviorBuilder());
BehaviorManager.registerBuilder(new ScrollBehaviorBuilder());
