import { SimObject } from "../world/simobject.js";
import { Shader } from "../gl/shader.js";

export interface IComponent {
    name: string;
    owner: SimObject;

    load(): void;

    update(time: number): void;
    render(shader: Shader): void;
    updateReady(): void;
}

export interface IComponentData {
    name: string;
    setFromJson(dat: any): void;
}

export interface IComponentBuilder {
    readonly type: string;

    buildFromJson(data: any): IComponent;
}
