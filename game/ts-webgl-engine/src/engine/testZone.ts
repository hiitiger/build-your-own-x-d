import { Zone } from "./world/zone.js";
import { Shader } from "./gl/shader.js";
import { SimObject } from "./world/simobject.js";
import { SpriteComponent } from "./components/spritecomponent.js";

export class TestZone extends Zone {
    private _parentObject: SimObject;
    private _testObject: SimObject;
    private _testSprite: SpriteComponent;

    public constructor(id: number) {
        super(id, "testZone", "testZone");
    }

    public load(): void {
        this._parentObject = new SimObject(0, "parentObject", this.scene);
        this._parentObject.transform.position.x = 250;
        this._parentObject.transform.position.y = 250;
        this._parentObject.addComponent(new SpriteComponent("testP", "crate"));

        this._testObject = new SimObject(0, "testObject", this.scene);
        this._testSprite = new SpriteComponent("test", "crate");
        this._testObject.addComponent(this._testSprite);

        this._testObject.transform.position.x = 10;
        this._testObject.transform.position.y = 10;

        this._parentObject.addChild(this._testObject);
        this.scene.addObject(this._parentObject);
        super.load();
    }

    public update(time: number): void {
        this._parentObject.transform.rotation.z += 0.01;
        this._testObject.transform.position.x += 1;
        this._testObject.transform.position.x %= 150;
        super.update(time);
    }

    public render(shader: Shader): void {
        super.render(shader);
    }
}
