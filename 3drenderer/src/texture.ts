export class Texture {
    constructor(
        public buffer: Uint8ClampedArray,
        public width: number,
        public height: number
    ) {}

    public map(u: number, v: number) {
        if (this.buffer) {
            const x = Math.abs((u * this.width) % this.width) | 0;
            const y = this.height - Math.abs((v * this.height) % this.height) | 0;

            var pos = (x + y * this.width) * 4;

            var r = this.buffer[pos];
            var g = this.buffer[pos + 1];
            var b = this.buffer[pos + 2];
            var a = this.buffer[pos + 3];

            return new BABYLON.Color4(
                r / 255.0,
                g / 255.0,
                b / 255.0,
                a / 255.0
            );
        }
        else {
            return new BABYLON.Color4(1, 1, 1, 1);
        }
    }
}
