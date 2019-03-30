export type IVector3 = [number, number, number];
export type IVector2 = [number, number];

export interface Vertex {
    pos: BABYLON.Vector3;
    normal?: BABYLON.Vector3;
    uv?: BABYLON.Vector2;
    color?: BABYLON.Color4;
    worldPos?: BABYLON.Vector3;
}
