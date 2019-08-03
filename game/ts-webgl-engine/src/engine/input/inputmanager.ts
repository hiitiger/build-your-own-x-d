export enum Keys {
    LEFT = 37,
    UP = 38,
    RIGHT = 39,
    DOWN = 40
}

export class InputManager {
    private static _keys: boolean[] = [];

    public static initialize() {
        InputManager._keys = Array(255).fill(false);

        window.addEventListener("keydown", InputManager.onKeyDown);
        window.addEventListener("keyup", InputManager.onKeyUp);
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
}
