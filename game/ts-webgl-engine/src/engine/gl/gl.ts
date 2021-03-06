export var gl: WebGLRenderingContext;
export var gl2d: CanvasRenderingContext2D;

export class GLUtilities {
    public static Initialize(elementId?: string): HTMLCanvasElement {
        let canvas: HTMLCanvasElement;

        if (elementId !== undefined) {
            canvas = document.getElementById(elementId) as HTMLCanvasElement;
            if (canvas === null) {
                throw new Error(`Cannot find canvas element ${elementId}`);
            }
        } else {
            canvas = document.createElement("canvas") as HTMLCanvasElement;
            document.body.appendChild(canvas);
        }

        gl = canvas.getContext("webgl");
        if (gl === null) {
            gl = canvas.getContext("experimental-webgl");
            if (gl === null) {
                throw new Error("Unable to init WebGL");
            }
        }

        gl2d = (document.getElementById("layerCanvas") as HTMLCanvasElement).getContext("2d");
        return canvas;
    }
}
