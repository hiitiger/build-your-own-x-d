import { Texture } from "./texture.js";
import { Vertex, IVector3, IVector2} from "./vertex.js";

export interface Face {
    verts: [number, number, number];
    uv?: [number, number, number];
    norms?: [number, number, number];
}

export class Mesh {
    position: BABYLON.Vector3;
    roation: BABYLON.Vector3;
    vertices: Vertex[];
    norms: BABYLON.Vector3[];
    uvs: BABYLON.Vector2[];
    faces: Face[];
    tex?: Texture;

    constructor(public name: string) {
        this.position = BABYLON.Vector3.Zero();
        this.roation = BABYLON.Vector3.Zero();
    }

    updateVertices(vertices: Vertex[]) {
        this.vertices = vertices;
    }

    updateNormals(norms: IVector3[]) {
        this.norms = norms.map(([x, y, z]) => new BABYLON.Vector3(x, y, z));
    }

    updateUVs(uvs: IVector2[]) {
        this.uvs = uvs.map(([x, y]) => new BABYLON.Vector2(x, y));
    }

    updateFaces(faces: Face[]) {
        this.faces = faces;
    }
}
