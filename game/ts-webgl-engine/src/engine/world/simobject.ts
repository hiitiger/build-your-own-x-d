import { Transform } from "../math/transform.js";
import { Matrix } from "../math/matrix.js";
import { Shader } from "../gl/shader.js";
import { Scene } from "./scene.js";
import { IComponent } from "../components/interface.js";
import { IBehavior } from "../behaviors/interface.js";
import { Vector2 } from "../math/vector2.js";
import { Vector3 } from "../math/vector3.js";

export class SimObject {
    private _id: number;
    private _children: SimObject[] = [];
    private _parent: SimObject;
    private _isLoaded: boolean = false;
    private _scene: Scene;
    private _components: IComponent[] = [];
    private _behaviors: IBehavior[] = [];

    private _localMatrix: Matrix = Matrix.identity();
    private _worldMatrix: Matrix = Matrix.identity();

    public name: string;
    public transform: Transform = new Transform();

    public constructor(id: number, name: string, scene?: Scene) {
        this._id = id;
        this.name = name;
        this._scene = scene;
    }

    public get id(): number {
        return this._id;
    }

    public get parent(): SimObject {
        return this._parent;
    }

    public get worldMatrix(): Matrix {
        return this._worldMatrix;
    }

    public get isLoaded(): boolean {
        return this._isLoaded;
    }

    public addChild(child: SimObject): void {
        child._parent = this;
        this._children.push(child);
        child.onAdded(this._scene);
    }

    public removeChild(child: SimObject): void {
        const index = this._children.indexOf(child);
        if (index !== -1) {
            child._parent = null;
            this._children.splice(index, 1);
        }
    }

    public getComponentByName(name: string): IComponent {
        for (const component of this._components) {
            if (component.name === name) {
                return component;
            }
        }
        for (const child of this._children) {
            const component = child.getComponentByName(name);
            if (component) {
                return component;
            }
        }

        return null;
    }

    public getBehaviorByName(name: string): IBehavior {
        for (const behavior of this._behaviors) {
            if (behavior.name === name) {
                return behavior;
            }
        }
        for (const child of this._children) {
            const behavior = child.getBehaviorByName(name);
            if (behavior) {
                return behavior;
            }
        }

        return null;
    }

    public getObjectByName(name: string): SimObject {
        if (this.name === name) {
            return this;
        } else {
            for (const child of this._children) {
                const res = child.getObjectByName(name);
                if (res !== null) {
                    return res;
                }
            }
        }
        return null;
    }

    public addComponent(component: IComponent): void {
        this._components.push(component);
        component.owner = this;
    }

    public addBehavior(behavior: IBehavior): void {
        this._behaviors.push(behavior);
        behavior.setOwner(this);
    }

    public load(): void {
        this._components.forEach(c => c.load());
        this._children.forEach(c => c.load());

        this._isLoaded = true;
    }

    public updateReady(): void {
        this._components.forEach(c => c.updateReady());
        this._behaviors.forEach(c => c.updateReady());
        this._children.forEach(c => c.updateReady());
    }

    public update(time: number): void {
        this._localMatrix = this.transform.getTransformationMatrix();
        this.updateWorldMatrix(this._parent && this._parent.worldMatrix);

        this._components.forEach(c => c.update(time));
        this._behaviors.forEach(c => c.update(time));
        this._children.forEach(c => c.update(time));
    }

    public render(shader: Shader): void {
        this._components.forEach(c => c.render(shader));
        this._children.forEach(c => c.render(shader));
    }

    public getWorldPosition(): Vector3 {
        return new Vector3(this._worldMatrix.data[12], this._worldMatrix.data[13], this._worldMatrix.data[14]);
    }

    protected onAdded(scene: Scene): void {
        this._scene = scene;
    }

    private updateWorldMatrix(parentWorldMatrix?: Matrix): void {
        if (parentWorldMatrix) {
            this._worldMatrix = Matrix.multiply(parentWorldMatrix, this._localMatrix);
        } else {
            this._worldMatrix.copyFrom(this._localMatrix);
        }
    }
}
