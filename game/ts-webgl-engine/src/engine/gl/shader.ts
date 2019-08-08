import { gl } from "./gl.js";

export class Shader {
    private _name: string;
    private _program: WebGLProgram;
    private _attribute: { [name: string]: number } = {};
    private _uniforms: { [name: string]: WebGLUniformLocation } = {};

    public constructor(name: string) {
        this._name = name;
    }

    public get name(): string {
        return this._name;
    }

    public getAttributeLocation(name: string): number {
        if (this._attribute[name] === undefined) {
            throw new Error(`Unable to find attribute ${name} in shader: ${this.name}`);
        }

        return this._attribute[name];
    }

    public getUniformLocation(name: string): WebGLUniformLocation {
        if (this._uniforms[name] === undefined) {
            throw new Error(`Unable to find uniform ${name} in shader: ${this.name}`);
        }

        return this._uniforms[name];
    }

    public use(): void {
        gl.useProgram(this._program);
    }

    protected load(vertexSource: string, fragmentSource: string): void {
        const vertexShader = this.loadShader(vertexSource, gl.VERTEX_SHADER);
        const fragmentShader = this.loadShader(fragmentSource, gl.FRAGMENT_SHADER);

        this.createProgram(vertexShader, fragmentShader);

        this.detectAttributes();
        this.detectUniforms();
    }

    private loadShader(source: string, shaderType: GLenum): WebGLShader {
        const shader: WebGLBuffer = gl.createShader(shaderType);

        gl.shaderSource(shader, source);
        gl.compileShader(shader);

        const error = gl.getShaderInfoLog(shader).trim();
        if (error) {
            throw new Error(`Error compiling shader: ${error}`);
        }
        return shader;
    }

    private createProgram(vertexShader: WebGLShader, fragmentShader: WebGLShader): void {
        this._program = gl.createProgram();

        gl.attachShader(this._program, vertexShader);
        gl.attachShader(this._program, fragmentShader);

        gl.linkProgram(this._program);

        const error = gl.getProgramInfoLog(this._program).trim();
        if (error) {
            throw new Error(`Error linking shader ${this._name} : ${error}`);
        }
    }

    private detectAttributes(): void {
        const attributeCount = gl.getProgramParameter(this._program, gl.ACTIVE_ATTRIBUTES);
        for (let i = 0; i < attributeCount; i++) {
            const attributeInfo: WebGLActiveInfo = gl.getActiveAttrib(this._program, i);
            if (!attributeInfo) {
                break;
            }

            this._attribute[attributeInfo.name] = gl.getAttribLocation(this._program, attributeInfo.name);
        }
    }

    private detectUniforms(): void {
        const uniformCount = gl.getProgramParameter(this._program, gl.ACTIVE_UNIFORMS);
        for (let i = 0; i < uniformCount; i++) {
            const info: WebGLActiveInfo = gl.getActiveUniform(this._program, i);
            if (!info) {
                break;
            }

            this._uniforms[info.name] = gl.getUniformLocation(this._program, info.name);
        }
    }
}
