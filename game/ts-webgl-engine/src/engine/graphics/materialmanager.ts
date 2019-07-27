import { Material } from "./material.js";

class MaterialReferenceNode {
    public material: Material;
    public referenceCount: number = 1;

    public constructor(material: Material) {
        this.material = material;
    }
}

export class MaterialManager {
    private static _materials: { [name: string]: MaterialReferenceNode } = {};

    private constructor() {}

    public static registerMaterial(material: Material): void {
        if (MaterialManager._materials[material.name] === undefined) {
            MaterialManager._materials[material.name] = new MaterialReferenceNode(material);
        }
    }

    public static getMaterial(name: string): Material {
        if (name in MaterialManager._materials) {
            MaterialManager._materials[name].referenceCount += 1;
            return MaterialManager._materials[name].material;
        } else {
            return null;
        }
    }

    public static releaseMaterial(name: string): void {
        if (MaterialManager._materials[name] === undefined) {
            console.warn(`Material ${name} is not registerd`);
        } else {
            MaterialManager._materials[name].referenceCount -= 1;
            if (MaterialManager._materials[name].referenceCount === 0) {
                MaterialManager._materials[name].material.destroy();
                MaterialManager._materials[name] = undefined;
            }
        }
    }
}
(window as any).MaterialManager = MaterialManager;
