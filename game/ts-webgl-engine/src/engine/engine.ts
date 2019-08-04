import { GLUtilities, gl, gl2d } from "./gl/gl.js";
import { Matrix } from "./math/matrix.js";
import { AssetManager } from "./assets/assetmanager.js";
import { MessageBus } from "./message/messagebus.js";
import { BasicShader } from "./gl/shaders/basicshader.js";
import { MaterialManager } from "./graphics/materialmanager.js";
import { Material } from "./graphics/material.js";
import { Color } from "./graphics/color.js";
import { ZoneManager } from "./world/zonemanager.js";
import { InputManager } from "./input/inputmanager.js";
import { AudioManager } from "./audio/audiomanager.js";
import { CollisionManager } from "./collision/collisionmanager.js";

export class Engine {
    private _canvas: HTMLCanvasElement;
    private _layerCanvas: HTMLCanvasElement;
    private _basicShader: BasicShader;
    private _projection: Matrix;
    private _time: number = 0;

    public constructor() {
        console.log("init...");
    }

    public start(): void {
        this._layerCanvas = document.getElementById("layerCanvas") as HTMLCanvasElement;
        this._canvas = GLUtilities.Initialize("renderCanvas");
        gl.clearColor(0, 0, 0, 1);
        gl.enable(gl.BLEND);
        gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);

        AssetManager.initialize();
        InputManager.initialize(this._canvas);
        ZoneManager.initialize();

        this._basicShader = new BasicShader();
        this._basicShader.use();

        MaterialManager.registerMaterial(new Material("crate", "assets/textures/crate.jpg", Color.white()));
        MaterialManager.registerMaterial(new Material("flybird", "assets/textures/flybird.png", Color.white()));

        AudioManager.loadSoundFile("flap", "assets/sounds/flap.mp3", true);
        // AudioManager.playSound("flap");

        this.resize();
        ZoneManager.changeZone(0);

        this.loop();
    }

    public resize(): void {
        if (this._canvas !== null) {
            this._canvas.width = window.innerWidth;
            this._canvas.height = window.innerHeight;
            this._layerCanvas.width = window.innerWidth;
            this._layerCanvas.height = window.innerHeight;
            this._projection = Matrix.orthographic(0, this._canvas.width, this._canvas.height, 0, -100, 100);

            gl.viewport(0, 0, this._canvas.width, this._canvas.height);
        }
    }

    public loop(time: number = 0): void {
        this.update(time);
        this.render();

        requestAnimationFrame(this.loop.bind(this));
    }

    private update(time: number): void {
        const delta = time - this._time;

        MessageBus.update(delta);
        ZoneManager.update(delta);
        CollisionManager.update(delta);
        this._time = time;
    }

    private render(): void {
        gl2d.fillStyle = "#00000000";
        gl2d.clearRect(0, 0, this._layerCanvas.width, this._layerCanvas.height);

        gl.clear(gl.COLOR_BUFFER_BIT);

        ZoneManager.render(this._basicShader);

        gl.uniformMatrix4fv(
            this._basicShader.getUniformLocation("u_projection"),
            false,
            new Float32Array(this._projection.data)
        );
    }
}
