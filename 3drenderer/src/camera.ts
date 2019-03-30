export class Camera {
    position: BABYLON.Vector3;
    target: BABYLON.Vector3;

    constructor() {
        this.position = BABYLON.Vector3.Zero();
        this.target = BABYLON.Vector3.Zero();
    }
}
