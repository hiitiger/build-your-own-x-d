import { SimObject } from "../world/simobject.js";
import { Shader } from "../gl/shader.js";
import { IComponent, IComponentData } from "./interface.js";

export abstract class BaseComponent implements IComponent {
    protected _owner: SimObject;
    protected _data: IComponentData;
    public name: string;

    public constructor(data: IComponentData) {
        this._data = data;
        this.name = data.name;
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
