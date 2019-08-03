import { IComponentBuilder, IComponent } from "./interface.js";
import { SpriteComponentBuilder } from "./spritecomponent.js";
import { AnimatedSpriteComponentBuilder } from "./animtedspritecomponent.js";

export class ComponentManager {
    private static _registeredBuilders: { [type: string]: IComponentBuilder } = {};

    public static registerBuilder(builder: IComponentBuilder): void {
        ComponentManager._registeredBuilders[builder.type] = builder;
    }

    public static extractComponent(data: any): IComponent {
        if ("type" in data) {
            if (ComponentManager._registeredBuilders[data.type]) {
                return ComponentManager._registeredBuilders[data.type].buildFromJson(data);
            }
        }
        throw new Error("extractComponent failed: type is missing or builder is not registered for this type.");
    }
}

ComponentManager.registerBuilder(new SpriteComponentBuilder());
ComponentManager.registerBuilder(new AnimatedSpriteComponentBuilder());
