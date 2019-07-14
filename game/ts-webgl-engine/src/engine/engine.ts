import { GLUtilities, gl } from "./gl/gl.js";
import { Shader } from "./gl/shader.js";
import { Sprite } from "./graphics/sprite.js";
import { Matrix } from "./math/matrix.js";

export class Engine {
    private _canvas: HTMLCanvasElement;
    private _shader: Shader;
    private _projection: Matrix;

    private _sprite: Sprite;

    public constructor() {
        console.log("init...");
    }

    public start(): void {
        this._canvas = GLUtilities.Initialize("renderCanvas");
        gl.clearColor(0, 0, 0, 1);

        this.loadShaders();
        this._shader.use();

        this._projection = Matrix.orthographic(0, this._canvas.width, 0, this._canvas.height, -100, 100);

        this._sprite = new Sprite("square");
        this._sprite.load();

        this.loop();
    }

    public resize(): void {
        if (this._canvas !== null) {
            this._canvas.width = window.innerWidth * 0.8;
            this._canvas.height = window.innerHeight * 0.8;
            this._projection = Matrix.orthographic(0, this._canvas.width, 0, this._canvas.height, -100, 100);

            gl.viewport(0, 0, this._canvas.width, this._canvas.height);
        }
    }

    public loop(): void {
        gl.clear(gl.COLOR_BUFFER_BIT);

        gl.uniform4fv(this._shader.getUniformLocation("u_color"), [1.0, 0.5, 0.5, 1]);
        gl.uniformMatrix4fv(this._shader.getUniformLocation("u_projection"), false, new Float32Array(this._projection.data));
        gl.uniformMatrix4fv(
            this._shader.getUniformLocation("u_model"),
            false,
            new Float32Array(Matrix.translation(this._sprite.postion).data)
        );

        this._sprite.postion.x += 10;
        this._sprite.postion.x %= this._canvas.width;
        this._sprite.draw();

        requestAnimationFrame(this.loop.bind(this));
    }

    private loadShaders(): void {
        const vertexShaderSource = `
attribute vec3 a_position;
uniform mat4 u_projection;
uniform mat4 u_model;

void main() {
    gl_Position = u_projection * u_model * vec4(a_position, 1.0);
}`;

        const fragmentShaderSource = `
precision mediump float;
uniform vec4 u_color;
void main() {
    gl_FragColor = u_color;
}`;

        this._shader = new Shader("basic", vertexShaderSource, fragmentShaderSource);
    }
}
