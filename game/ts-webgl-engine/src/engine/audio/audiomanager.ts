export class SoundEffect {
    public readonly assetPath: string;

    private _player: HTMLAudioElement;

    public constructor(assetPath: string, loop: boolean) {
        this.assetPath = assetPath;
        this._player = new Audio(assetPath);
        this.loop = loop;
    }

    public get loop(): boolean {
        return this._player.loop;
    }

    public set loop(v: boolean) {
        this._player.loop = v;
    }

    public destroy(): void {
        this._player = null;
    }

    public play(): void {
        if (!this._player.paused) {
            this.stop();
        }
        this._player.play();
    }

    public pause(): void {
        this._player.pause();
    }

    public stop(): void {
        this._player.pause();
        this._player.currentTime = 0;
    }
}

export class AudioManager {
    private static _soundEffects: { [name: string]: SoundEffect } = {};

    public static loadSoundFile(name: string, assetPath: string, loop: boolean): void {
        AudioManager._soundEffects[name] = new SoundEffect(assetPath, loop);
    }

    public static playSound(name: string): void {
        if (AudioManager._soundEffects[name]) {
            AudioManager._soundEffects[name].play();
        }
    }

    public static pauseSound(name: string): void {
        if (AudioManager._soundEffects[name]) {
            AudioManager._soundEffects[name].pause();
        }
    }

    public static stopSound(name: string): void {
        if (AudioManager._soundEffects[name]) {
            AudioManager._soundEffects[name].stop();
        }
    }

    public static pauseAll(): void {
        for (const name in AudioManager._soundEffects) {
            AudioManager._soundEffects[name].pause();
        }
    }

    public static stopAll(): void {
        for (const name in AudioManager._soundEffects) {
            AudioManager._soundEffects[name].stop();
        }
    }
}
