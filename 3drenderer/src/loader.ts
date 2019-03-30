import { Mesh, Face } from "./mesh.js";
import { IVector3 } from "./vertex.js";
import { Texture } from "./texture.js";

export function loadTexture(name: string): Promise<Texture> {
    return new Promise((resolve, reject) => {
        const imageTexture = new Image();
        imageTexture.onload = () => {
            const width = imageTexture.width;
            const height = imageTexture.height;
            const internalCanvas: HTMLCanvasElement = document.createElement(
                "canvas"
            );
            internalCanvas.width = width;
            internalCanvas.height = height;
            const internalContext: CanvasRenderingContext2D = internalCanvas.getContext(
                "2d"
            );
            internalContext.drawImage(imageTexture, 0, 0);
            const internalBuffer = internalContext.getImageData(
                0,
                0,
                width,
                height
            );

            const tex = new Texture(internalBuffer.data, width, height);
            resolve(tex);
        };
        imageTexture.src = `./mesh/${name}.jpg`;
    });
}

export function loadMesh(name: string) {
    return fetch(`./mesh/${name}.obj`)
        .then(r => r.text())
        .then(objectData => {
            const lines = objectData.split(/[\n\r]+/);
            const verts: IVector3[] = [];
            const uvs = [];
            const norms = [];
            const faces: Face[] = [];
            for (let line of lines) {
                line = line.trim();
                if (!line || line.startsWith("#")) {
                    continue;
                }
                const lineData = line.split(/\s+/);
                lineData.shift();
                if (line.startsWith("v ")) {
                    verts.push(lineData.map(v => Number(v)) as IVector3);
                } else if (line.startsWith("vn ")) {
                    norms.push(lineData.map(v => Number(v)));
                } else if (line.startsWith("vt ")) {
                    uvs.push([Number(lineData[0]), Number(lineData[1])]);
                } else if (line.startsWith("f ")) {
                    let vertex0 = lineData[0].split("/").map(v => Number(v));
                    let vertex1 = lineData[1].split("/").map(v => Number(v));
                    let vertex2 = lineData[2].split("/").map(v => Number(v));

                    faces.push({
                        verts: [vertex0[0] - 1, vertex1[0] - 1, vertex2[0] - 1],
                        uv: [vertex0[1] - 1, vertex1[1] - 1, vertex2[1] - 1],
                        norms: [vertex0[2] - 1, vertex1[2] - 1, vertex2[2] - 1]
                    });

                    for (let i = 3; i < lineData.length; ++i) {
                        let vertex0 = lineData[i - 3]
                            .split("/")
                            .map(v => Number(v));
                        let vertex1 = lineData[i - 1]
                            .split("/")
                            .map(v => Number(v));
                        let vertex2 = lineData[i]
                            .split("/")
                            .map(v => Number(v));

                        faces.push({
                            verts: [
                                vertex0[0] - 1,
                                vertex1[0] - 1,
                                vertex2[0] - 1
                            ],
                            uv: [
                                vertex0[1] - 1,
                                vertex1[1] - 1,
                                vertex2[1] - 1
                            ],

                            norms: [
                                vertex0[2] - 1,
                                vertex1[2] - 1,
                                vertex2[2] - 1
                            ]
                        });
                    }
                }
            }

            const mesh = new Mesh(name);

            mesh.updateVertices(
                verts.map(([x, y, z]) => {
                    return {
                        pos: new BABYLON.Vector3(x, y, z)
                    };
                })
            );
            mesh.updateNormals(norms);
            mesh.updateUVs(uvs);
            mesh.updateFaces(faces);
            return mesh;
        });
}
