import { GLUtilities, gl } from "./gl/gl.js";
import { Sprite } from "./graphics/sprite.js";
import { Matrix } from "./math/matrix.js";
import { AssetManager } from "./assets/assetmanager.js";
import { MessageBus } from "./message/messagebus.js";
import { BasicShader } from "./gl/shaders/basicshader.js";
import { MaterialManager } from "./graphics/materialmanager.js";
import { Material } from "./graphics/material.js";
import { Color } from "./graphics/color.js";

export class Engine {
    private _canvas: HTMLCanvasElement;
    private _basicShader: BasicShader;
    private _projection: Matrix;

    private _sprite: Sprite;

    public constructor() {
        console.log("init...");
    }

    public start(): void {
        this._canvas = GLUtilities.Initialize("renderCanvas");
        gl.clearColor(0, 0, 0, 1);

        AssetManager.initialize();

        this._basicShader = new BasicShader();
        this._basicShader.use();

        MaterialManager.registerMaterial(new Material("crate", "assets/textures/crate.jpg", Color.white()));

        this._projection = Matrix.orthographic(0, this._canvas.width, 0, this._canvas.height, -100, 100);

        this._sprite = new Sprite("square", "crate");
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
        MessageBus.update(0);

        gl.clear(gl.COLOR_BUFFER_BIT);

        gl.uniformMatrix4fv(
            this._basicShader.getUniformLocation("u_projection"),
            false,
            new Float32Array(this._projection.data)
        );

        this._sprite.postion.x += 10;
        this._sprite.postion.x %= this._canvas.width;
        this._sprite.draw(this._basicShader);

        requestAnimationFrame(this.loop.bind(this));
    }
}
