import * as TSE from "./engine/engine.js";

const engine = new TSE.Engine();
engine.start();
engine.resize();

window.onresize = () => engine.resize();
