import { Transform } from "../math/transform.js";
import { Matrix } from "../math/matrix.js";
import { Shader } from "../gl/shader.js";
import { Scene } from "./scene.js";
import { IComponent } from "../components/interface.js";

export class SimObject {
    private _id: number;
    private _children: SimObject[] = [];
    private _parent: SimObject;
    private _isLoaded: boolean = false;
    private _scene: Scene;
    private _components: IComponent[] = [];

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

    public load(): void {
        this._components.forEach(c => c.load());
        this._children.forEach(c => c.load());

        this._isLoaded = true;
    }

    public update(time: number): void {
        this._localMatrix = this.transform.getTransformationMatrix();
        this.updateWorldMatrix(this._parent && this._parent.worldMatrix);

        this._components.forEach(c => c.update(time));
        this._children.forEach(c => c.update(time));
    }

    public render(shader: Shader): void {
        this._components.forEach(c => c.render(shader));
        this._children.forEach(c => c.render(shader));
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
