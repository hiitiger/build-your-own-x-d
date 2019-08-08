import { Scene } from "./scene.js";
import { Shader } from "../gl/shader.js";
import { SimObject } from "./simobject.js";
import { ComponentManager } from "../components/componentmanager.js";
import { BehaviorManager } from "../behaviors/bahaviormanager.js";

export enum ZoneState {
    UNINITIALIZED,
    LOADING,
    UPDATING
}

export class Zone {
    private _id: number;
    private _name: string;
    private _description: string;
    private _scene: Scene;
    private _objectId: number = 0;
    private _state: ZoneState;

    public constructor(id: number, name: string, description: string) {
        this._id = id;
        this._name = name;
        this._description = description;

        this._scene = new Scene();
        this._state = ZoneState.UNINITIALIZED;
    }

    public initialize(data: any): void {
        if (!("objects" in data)) {
            throw new Error("Zone initialization error: objects not present.");
        }
        data.objects.forEach(obj => {
            this.loadSimObject(obj, this._scene.root);
        });
    }

    public get id(): number {
        return this._id;
    }

    public get name(): string {
        return this._name;
    }

    public get description(): string {
        return this._description;
    }

    public get scene(): Scene {
        return this._scene;
    }

    public load(): void {
        this._state = ZoneState.LOADING;

        this._scene.load();
        this._scene.root.updateReady();

        this._state = ZoneState.UPDATING;
    }

    public unload(): void {}

    public update(time: number): void {
        if (this._state === ZoneState.UPDATING) {
            this._scene.update(time);
        }
    }

    public render(shader: Shader): void {
        if (this._state === ZoneState.UPDATING) {
            this._scene.render(shader);
        }
    }

    public onActivated(): void {}
    public onDeactivated(): void {}

    private loadSimObject(data: any, parent?: SimObject): void {
        if (!("name" in data)) {
            throw new Error(`loadSimObject data unexpected error name not exist`);
        }

        ++this._objectId;
        const simObject = new SimObject(this._objectId, data.name, this._scene);

        if ("transform" in data) {
            simObject.transform.setFromJson(data.transform);
        }

        if (data.components && Array.isArray(data.components)) {
            data.components.forEach(c => {
                const component = ComponentManager.extractComponent(c);
                simObject.addComponent(component);
            });
        }

        if (data.behaviors && Array.isArray(data.behaviors)) {
            data.behaviors.forEach(c => {
                const behavior = BehaviorManager.extractBehavior(c);
                simObject.addBehavior(behavior);
            });
        }

        if (data.children && Array.isArray(data.children)) {
            data.children.forEach(child => {
                this.loadSimObject(child, simObject);
            });
        }

        if (parent) {
            parent.addChild(simObject);
        }
    }
}
