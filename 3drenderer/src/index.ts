import { Mesh } from "./mesh.js";
import { Camera } from "./camera.js";

import { Device, RenderState } from "./device.js";
import { loadMesh, loadTexture } from "./loader.js";

let canvas: HTMLCanvasElement;
let device: Device;
let camera: Camera;
let meshes: Mesh[] = [];

let running = true;

const button = document.getElementById("btn") as HTMLButtonElement;
button.addEventListener("click", () => {
    running = !running;
    button.innerText = running ? "stop" : "start";
});

function testDawLine() {
    const points = [
        [[10, 10], [10, 200]],
        [[10, 10], [100, 10]],
        [[10, 10], [100, 200]],
        [[10, 10], [110, 110]],
        [[10, 10], [300, 100]],
        [[300, 300], [200, 200]],
        [[300, 200], [200, 300]]
    ];
    for (const p of points) {
        const p0 = new BABYLON.Vector3(p[0][0], p[0][1], 0);
        const p1 = new BABYLON.Vector3(p[1][0], p[1][1], 0);
        device.drawLine(p0, p1);
    }
}

const renderLoop = () => {
    if (running) {
        device.clear();

        for (const mesh of meshes) {
            // mesh.roation.x += 0.01;
            mesh.roation.y += 0.02;
        }

        device.render(camera, meshes);
        device.present();
    }

    requestAnimationFrame(renderLoop);
};

const randomColor = () => {
    return new BABYLON.Color4(Math.random(), Math.random(), Math.random());
};

const init = async () => {
    canvas = document.getElementById("renderCanvas") as HTMLCanvasElement;
    console.log(`${canvas}`);
    const cubeMesh = new Mesh("cube");
    cubeMesh.updateVertices([
        { pos: new BABYLON.Vector3(-1, -1, -1), color: new BABYLON.Color4(1) },
        { pos: new BABYLON.Vector3(-1, 1, -1), color: new BABYLON.Color4(0, 1, 0) },
        { pos: new BABYLON.Vector3(1, 1, -1), color: new BABYLON.Color4(0, 0, 1) },
        { pos: new BABYLON.Vector3(1, -1, -1), color: new BABYLON.Color4(1, 1, 1)  },
        { pos: new BABYLON.Vector3(-1, -1, 1), color: randomColor() },
        { pos: new BABYLON.Vector3(-1, 1, 1), color: randomColor() },
        { pos: new BABYLON.Vector3(1, 1, 1), color: randomColor() },
        { pos: new BABYLON.Vector3(1, -1, 1), color: randomColor() }
    ]);

    cubeMesh.updateFaces([
        { verts: [0, 1, 2] },
        { verts: [0, 2, 3] },
        { verts: [7, 6, 5] },
        { verts: [7, 5, 4] },
        { verts: [1, 5, 6] },
        { verts: [1, 6, 2] },
        { verts: [4, 0, 3] },
        { verts: [4, 3, 7] },
        { verts: [0, 4, 5] },
        { verts: [0, 5, 1] },
        { verts: [3, 2, 6] },
        { verts: [3, 6, 7] }
    ]);
    meshes.push(cubeMesh);

    {
        const model = await loadMesh("cat");
        model.tex = await loadTexture("cat_d");
        model.position = new BABYLON.Vector3(0, 1, -4)
        meshes.push(model);
    }

    camera = new Camera();
    camera.position = new BABYLON.Vector3(0, 0, -12);
    camera.target = new BABYLON.Vector3(0, 0, 0);

    device = new Device(canvas);
    device.light = new BABYLON.Vector3(0, 1, -5.5);
    device.renderState = RenderState.Texture | RenderState.Color
    requestAnimationFrame(renderLoop);

    button.innerText = "stop";
};

document.addEventListener("DOMContentLoaded", init, false);

/*
  
 y    z
/|\ /
 | /
 |/___x
 ______
|\      \
| \      \
|  \______\
|   |      |
 \  |      |
  \ |      |
   \|______|
   
*/
