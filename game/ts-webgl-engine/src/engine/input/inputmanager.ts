import { Vector2 } from "../math/vector2.js";
import { Message } from "../message/message.js";

export enum Keys {
    LEFT = 37,
    UP = 38,
    RIGHT = 39,
    DOWN = 40
}

export const MESSAGE_MOUSE_DOWN: string = "MOUSE_DOWN";
export const MESSAGE_MOUSE_UP: string = "MOUSE_UP";

export class MouseContext {
    public leftMouseDown: boolean;
    public rightMouseDown: boolean;
    public position: Vector2;

    public constructor(leftDown: boolean = false, rightDown: boolean = false, position: Vector2) {
        this.leftMouseDown = leftDown;
        this.rightMouseDown = rightDown;
        this.position = position;
    }
}

export class InputManager {
    private static _keys: boolean[] = [];
    private static _previousMouseX: number = 0;
    private static _previousMouseY: number = 0;
    private static _mouseX: number = 0;
    private static _mouseY: number = 0;
    private static _leftMouseDown: boolean = false;
    private static _rightMouseDown: boolean = false;

    public static initialize(canvas: HTMLCanvasElement): void {
        InputManager._keys = Array(255).fill(false);

        window.addEventListener("keydown", InputManager.onKeyDown);
        window.addEventListener("keyup", InputManager.onKeyUp);

        canvas.addEventListener("mousemove", InputManager.onMouseMove);
        canvas.addEventListener("mousedown", InputManager.onMouseDown);
        canvas.addEventListener("mouseup", InputManager.onMouseUp);
    }

    public static isKeyDown(key: Keys): boolean {
        return InputManager._keys[key];
    }

    private static onKeyDown(event: KeyboardEvent): boolean {
        InputManager._keys[event.keyCode] = true;
        return true;
    }

    private static onKeyUp(event: KeyboardEvent): boolean {
        InputManager._keys[event.keyCode] = false;
        return true;
    }

    public static getMousePosition(): Vector2 {
        return new Vector2(this._mouseX, this._mouseY);
    }

    public static getPreviousMousePosition(): Vector2 {
        return new Vector2(this._previousMouseX, this._previousMouseY);
    }

    private static getMouseContext(): MouseContext {
        return new MouseContext(
            InputManager._leftMouseDown,
            InputManager._rightMouseDown,
            InputManager.getMousePosition()
        );
    }

    private static onMouseMove(event: MouseEvent): void {
        InputManager._previousMouseX = InputManager._mouseX;
        InputManager._previousMouseY = InputManager._mouseY;

        const rect = (event.target as HTMLElement).getBoundingClientRect();
        InputManager._mouseX = event.clientX - rect.left;
        InputManager._mouseY = event.clientY - rect.top;
    }

    private static onMouseDown(event: MouseEvent): void {
        if (event.button === 0) {
            this._leftMouseDown = true;
        } else if (event.button === 2) {
            this._rightMouseDown = true;
        }

        Message.send(MESSAGE_MOUSE_DOWN, this, InputManager.getMouseContext());
    }

    private static onMouseUp(event: MouseEvent): void {
        if (event.button === 0) {
            this._leftMouseDown = false;
        } else if (event.button === 2) {
            this._rightMouseDown = false;
        }

        Message.send(MESSAGE_MOUSE_UP, this, InputManager.getMouseContext());
    }
}
