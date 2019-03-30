import { Mesh } from "./mesh.js";
import { Vertex } from "./vertex.js";
import { Camera } from "./camera.js";
import { Texture } from "./texture.js";

function isCcw(v0: BABYLON.Vector3, v1: BABYLON.Vector3, v2: BABYLON.Vector3) {
  return (v1.x - v0.x) * (v2.y - v0.y) - (v1.y - v0.y) * (v2.x - v0.x) >= 0;
}

export enum RenderState {
  None = 0,
  WireFrame = 1,
  Texture = 1 << 1,
  Color = 1 << 2
}

export class Device {
  private backbuffer: ImageData;
  private canvas: HTMLCanvasElement;
  private context: CanvasRenderingContext2D;
  private width: number;
  private height: number;
  private backbufferData: Uint8ClampedArray;
  private zbuffer: Array<number>;

  public light: BABYLON.Vector3;
  public lighting: boolean;
  public renderState: RenderState;

  constructor(canvas: HTMLCanvasElement) {
    this.canvas = canvas;
    this.context = canvas.getContext("2d");
    this.width = canvas.width;
    this.height = canvas.height;
    this.zbuffer = new Array(this.width * this.height);
    this.light = new BABYLON.Vector3(0, 0, 1.2);
    this.lighting = false;
    this.renderState = RenderState.WireFrame;
  }

  clear() {
    this.context.clearRect(0, 0, this.width, this.height);
    this.backbuffer = this.context.getImageData(0, 0, this.width, this.height);
    for (var i = 0; i < this.zbuffer.length; i++) {
      this.zbuffer[i] = 2147483647;
    }
  }

  present() {
    this.context.putImageData(this.backbuffer, 0, 0);
  }

  clamp(v: number, min: number = 0, max: number = 1) {
    return Math.max(min, Math.min(v, max));
  }

  interpolate<T>(x: T, y: T, factor: number): T {
    if (typeof x === "number") {
      return ((x + ((y as any) - x) * this.clamp(factor)) as any) as T;
    } else {
      return (x as any).add((y as any).subtract(x).scale(this.clamp(factor)));
    }
  }

  putPixel(x: number, y: number, z: number, color: BABYLON.Color4) {
    this.backbufferData = this.backbuffer.data;
    const pixelIndex = Math.round(x) + Math.round(y) * this.width;
    if (this.zbuffer[pixelIndex] < z) {
      return;
    }
    this.zbuffer[pixelIndex] = z;
    const index = pixelIndex * 4;
    this.backbufferData[index] = color.r * 255;
    this.backbufferData[index + 1] = color.g * 255;
    this.backbufferData[index + 2] = color.b * 255;
    this.backbufferData[index + 3] = color.a * 255;
  }

  drawPoint(point: BABYLON.Vector3, color?: BABYLON.Color4) {
    if (
      point.x >= 0 &&
      point.y >= 0 &&
      point.x < this.width &&
      point.y < this.height
    ) {
      this.putPixel(
        point.x,
        point.y,
        point.z,
        color || new BABYLON.Color4(1, 1, 1, 1)
      );
    }
  }

  drawLineXBresenham(
    point0: BABYLON.Vector3,
    point1: BABYLON.Vector3,
    color?: BABYLON.Color4
  ) {
    /*
            a =  (y1-y0)/(x1-x0)
            y = a * (x - x0) + y0
            b = y0 - a * x0
        */
    let [x0, y0, x1, y1] = [point0.x, point0.y, point1.x, point1.y];
    const steep = Math.abs(y1 - y0) > Math.abs(x1 - x0);

    //transpose x -> y
    if (steep) {
      [x0, y0] = [y0, x0];
      [x1, y1] = [y1, x1];
    }

    //if line is right -> left, make it left -< right
    if (x0 > x1) {
      [x0, x1] = [x1, x0];
      [y0, y1] = [y1, y0];
    }
    const dy = y1 - y0;
    const dx = x1 - x0;
    const derror = Math.abs(dy) / dx;
    const derror2 = Math.abs(dy) * 2;
    let error = 0;
    let y = y0;
    let ystep = 1;
    if (y0 > y1) {
      ystep = -1;
    }
    for (let x = x0; x < x1; ++x) {
      if (steep) {
        this.drawPoint(new BABYLON.Vector3(y, x, 0), color);
      } else {
        this.drawPoint(new BABYLON.Vector3(x, y, 0), color);
      }
      // error += derror
      // if (error > 0.5) {
      //     y += ystep;
      //     error -= 1;
      // }
      error += derror2;
      if (error > dx) {
        y += ystep;
        error -= dx * 2;
      }
    }
  }

  drawLine(
    point0: BABYLON.Vector3,
    point1: BABYLON.Vector3,
    color?: BABYLON.Color4
  ) {
    // const dist = point1.subtract(point0).length();
    // if (dist <= 1)
    //     return;

    // const middlePoint = point0.add((point1.subtract(point0)).scale(0.5));
    // this.drawPoint(middlePoint);
    // this.drawLine(point0, middlePoint);
    // this.drawLine(middlePoint, point1);
    this.drawLineXBresenham(point0, point1, color);
  }

  drawScanline(
    v0: Vertex,
    v1: Vertex,
    y: number,
    diffuse: number,
    color: BABYLON.Color4,
    tex?: Texture
  ) {
    if (v0.pos.x > v1.pos.x) {
      [v0, v1] = [v1, v0];
    }

    const x0 = v0.pos.x;
    const x1 = v1.pos.x;
    const z0 = v0.pos.z;
    const z1 = v1.pos.z;

    for (let x = x0; x <= x1; ++x) {
      let factor = 1;
      if (x1 !== x0) {
        factor = (x - x0) / (x1 - x0);
      }
      const z = this.interpolate(z0, z1, factor);

      if (this.lighting) {
        const normal = this.interpolate(v0.normal, v1.normal, factor);
        const worldPos = this.interpolate(v0.worldPos, v1.worldPos, factor);
        diffuse = this.computeDiffuse(worldPos, normal, this.light);
      }

      let textureColor: BABYLON.Color4;
      if (tex && this.renderState & RenderState.Texture) {
        const u = this.interpolate(v0.uv.x, v1.uv.x, factor);
        const v = this.interpolate(v0.uv.y, v1.uv.y, factor);
        textureColor = tex.map(u, v);
      } else {
        textureColor = new BABYLON.Color4(1, 1, 1, 1);
      }

      let vertColor: BABYLON.Color4;
      if (v0.color && this.renderState & RenderState.Color) {
        vertColor = this.interpolate(v0.color, v1.color, factor);
      } else {
        vertColor = new BABYLON.Color4(1, 1, 1, 1);
      }

      this.drawPoint(
        new BABYLON.Vector3(x, y, z),
        new BABYLON.Color4(
          color.r * diffuse * textureColor.r * vertColor.r,
          color.g * diffuse * textureColor.g * vertColor.g,
          color.b * diffuse * textureColor.b * vertColor.b,
          1
        )
      );
    }
  }

  drawTriangle(
    v0: Vertex,
    v1: Vertex,
    v2: Vertex,
    color: BABYLON.Color4,
    tex?: Texture
  ) {
    if (!isCcw(v0.pos, v1.pos, v2.pos)) {
      return;
    }

    if (v0.pos.y > v1.pos.y) {
      [v0, v1] = [v1, v0];
    }
    if (v0.pos.y > v2.pos.y) {
      [v0, v2] = [v2, v0];
    }
    if (v1.pos.y > v2.pos.y) {
      [v1, v2] = [v2, v1];
    }
    const point0 = v0.pos;
    const point1 = v1.pos;
    const point2 = v2.pos;

    let diffuse = 1;
    // if (this.lighting) {
    //     const vn = v0.normal
    //         .add(v1.normal)
    //         .add(v2.normal)
    //         .scale(1 / 3);

    //     const center = v0.worldPos
    //         .add(v1.worldPos)
    //         .add(v2.worldPos)
    //         .scale(1 / 3);
    //     diffuse = this.computeDiffuse(center, vn, this.light);
    // }

    if (this.renderState & (RenderState.Texture | RenderState.Color)) {
      const middelFactor = (point1.y - point0.y) / (point2.y - point0.y);
      const middleVert: Vertex = {
        pos: this.interpolate(point0, point2, middelFactor),
        worldPos: this.interpolate(v0.worldPos, v2.worldPos, middelFactor)
      };
      if (this.lighting) {
        middleVert.normal = this.interpolate(
          v0.normal,
          v2.normal,
          middelFactor
        );
      }
      if (v0.color) {
        middleVert.color = this.interpolate(v0.color, v2.color, middelFactor);
      }
      if (tex) {
        middleVert.uv = this.interpolate(v0.uv, v2.uv, middelFactor);
      }

      for (let y = point0.y; y < point2.y; ++y) {
        const isUpperHalf = y < point1.y;
        if (isUpperHalf) {
          const factor = (y - point0.y) / (point1.y - point0.y);

          const vx01: Vertex = {
            pos: this.interpolate(point0, point1, factor)
          };
          const vx02: Vertex = {
            pos: this.interpolate(point0, middleVert.pos, factor)
          };
          if (tex) {
            vx01.uv = this.interpolate(v0.uv, v1.uv, factor);
            vx02.uv = this.interpolate(v0.uv, middleVert.uv, factor);
          }

          if (this.lighting) {
            vx01.worldPos = this.interpolate(v0.worldPos, v1.worldPos, factor);
            vx01.normal = this.interpolate(v0.normal, v1.normal, factor);

            vx02.worldPos = this.interpolate(
              v0.worldPos,
              middleVert.worldPos,
              factor
            );
            vx02.normal = this.interpolate(
              v0.normal,
              middleVert.normal,
              factor
            );
          }

          if (middleVert.color) {
            vx01.color = this.interpolate(v0.color, v1.color, factor);
            vx02.color = this.interpolate(v0.color, middleVert.color, factor);
          }

          this.drawScanline(vx01, vx02, y, diffuse, color, tex);
        } else {
          const factor = (y - point1.y) / (point2.y - point1.y);

          const vx12: Vertex = {
            pos: this.interpolate(point1, point2, factor)
          };
          const vx02: Vertex = {
            pos: this.interpolate(middleVert.pos, point2, factor)
          };
          if (tex) {
            vx12.uv = this.interpolate(v1.uv, v2.uv, factor);
            vx02.uv = this.interpolate(middleVert.uv, v2.uv, factor);
          }

          if (this.lighting) {
            vx12.worldPos = this.interpolate(v1.worldPos, v2.worldPos, factor);
            vx12.normal = this.interpolate(v1.normal, v2.normal, factor);

            vx02.worldPos = this.interpolate(
              middleVert.worldPos,
              v2.worldPos,
              factor
            );
            vx02.normal = this.interpolate(
              middleVert.normal,
              v2.normal,
              factor
            );
          }
          if (middleVert.color) {
            vx12.color = this.interpolate(v1.color, v2.color, factor);
            vx02.color = this.interpolate(middleVert.color, v2.color, factor);
          }
          this.drawScanline(vx12, vx02, y, diffuse, color, tex);
        }
      }
    }

    if (this.renderState & RenderState.WireFrame) {
      this.drawLine(v0.pos, v1.pos);
      this.drawLine(v1.pos, v2.pos);
      this.drawLine(v2.pos, v0.pos);
    }
  }

  computeDiffuse(
    vertex: BABYLON.Vector3,
    normal: BABYLON.Vector3,
    light: BABYLON.Vector3
  ) {
    const lightDir = light.subtract(vertex);
    const distance = lightDir.length();
    let diffuse = this.clamp(
      BABYLON.Vector3.Dot(normal.normalize(), lightDir.normalize())
    );

    let att =
      1.0 /
      BABYLON.Vector3.Dot(
        new BABYLON.Vector3(0, 0, 1),
        new BABYLON.Vector3(1, distance, distance * distance)
      );
    att = this.clamp(att);
    diffuse *= att;
    return diffuse;
  }

  transformToSceen(coord: BABYLON.Vector3) {
    const x = Math.trunc(coord.x * this.height + this.width / 2.0);
    const y = Math.trunc(-coord.y * this.width + this.width / 2.0);
    return new BABYLON.Vector3(x, y, coord.z);
  }

  project(
    vert: Vertex,
    transformMatrix: BABYLON.Matrix,
    worldMatrix: BABYLON.Matrix
  ): Vertex {
    if (!vert.pos) {
      debugger;
    }
    const worldPos = BABYLON.Vector3.TransformCoordinates(
      vert.pos,
      worldMatrix
    );

    const screenPos = this.transformToSceen(
      BABYLON.Vector3.TransformCoordinates(vert.pos, transformMatrix)
    );

    const r: Vertex = {
      pos: screenPos,
      worldPos,
      uv: vert.uv,
      color: vert.color
    };

    if (vert.normal) {
      const normal = BABYLON.Vector3.TransformCoordinates(
        vert.normal,
        worldMatrix
      );
      r.normal = normal;
    }

    return r;
  }

  render(camera: Camera, meshes: Mesh[]) {
    const viewMatrix = BABYLON.Matrix.LookAtLH(
      camera.position,
      camera.target,
      BABYLON.Vector3.Up()
    );
    const projectionMatrix = BABYLON.Matrix.PerspectiveFovLH(
      0.9,
      this.width / this.height,
      0.01,
      1.0
    );
    for (const mesh of meshes) {
      if (mesh.norms) {
        this.lighting = true;
      } else {
        this.lighting = false;
      }
      const worldMatrix = BABYLON.Matrix.RotationYawPitchRoll(
        mesh.roation.y,
        mesh.roation.x,
        mesh.roation.z
      ).multiply(
        BABYLON.Matrix.Translation(
          mesh.position.x,
          mesh.position.y,
          mesh.position.z
        )
      );

      const transformMatrix = worldMatrix
        .multiply(viewMatrix)
        .multiply(projectionMatrix);

      for (let i = 0; i < mesh.faces.length; i += 1) {
        const face = mesh.faces[i];
        const toVertext = (vertIndex: number) => {
          const v: Vertex = {
            pos: mesh.vertices[face.verts[vertIndex]].pos,
            color: mesh.vertices[face.verts[vertIndex]].color
          };
          if (face.norms) {
            v.normal = mesh.norms[face.norms[vertIndex]];
          }
          if (mesh.tex) {
            v.uv = mesh.uvs[face.uv[vertIndex]];
          }
          return v;
        };

        const v0 = this.project(toVertext(0), transformMatrix, worldMatrix);
        const v1 = this.project(toVertext(1), transformMatrix, worldMatrix);
        const v2 = this.project(toVertext(2), transformMatrix, worldMatrix);

        this.drawTriangle(v0, v1, v2, new BABYLON.Color4(1, 1, 1, 1), mesh.tex);
      }
    }
  }
}
