export class GameTimer {

    frameId: number;
    lastTime: number;
    renderCallback: (delta: number) => void;

    constructor(renderCallback: (delta: number) => void) {
        this.frameId = 0;
        this.renderCallback = renderCallback;
    }

    onFrame(time: number) {
        const delta = time - this.lastTime;
        this.lastTime = time;
        this.renderCallback(delta);
        this.frameId = requestAnimationFrame(this.onFrame.bind(this));
    }

    start() {
        this.lastTime = performance.now();
        this.frameId = requestAnimationFrame(this.onFrame.bind(this));
    }

    stop() {
        cancelAnimationFrame(this.frameId);
    }

}
