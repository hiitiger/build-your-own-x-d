import * as TSE from "./engine/engine.js";

const engine = new TSE.Engine(320, 480);
engine.start();
engine.resize();

window.onresize = () => engine.resize();
