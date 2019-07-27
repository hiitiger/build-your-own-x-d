import { SimObject } from "../world/simobject.js";
import { Shader } from "../gl/shader.js";

export abstract class BaseComponent {
    protected _owner: SimObject;
    public name: string;

    public constructor(name: string) {
        this.name = name;
    }

    public set owner(owner: SimObject) {
        this._owner = owner;
    }

    public get owner(): SimObject {
        return this._owner;
    }

    public load(): void {}

    public update(time: number) {}
    public render(shader: Shader) {}
}
