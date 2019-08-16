import { CollisionComponent } from "../components/collisioncomponent.js";
import { Message } from "../message/message.js";

export const MESSAGE_COLLISION_ENTER = "COLLISION_ENTER";
export const MESSAGE_COLLISION_EXIT = "COLLISION_EXIT";

export class CollisionData {
    public a: CollisionComponent;
    public b: CollisionComponent;
    public time: number;

    public constructor(time: number, a: CollisionComponent, b: CollisionComponent) {
        this.time = time;
        this.a = a;
        this.b = b;
    }
}

export class CollisionManager {
    private static _totalTime: number = 0;
    private static _components: CollisionComponent[] = [];

    private static _collisionData: CollisionData[] = [];

    public static registerCollisionComponent(component: CollisionComponent): void {
        CollisionManager._components.push(component);
    }

    public static unRegisterCollisionComponent(component: CollisionComponent): void {
        const index = CollisionManager._components.indexOf(component);
        if (index !== -1) {
            CollisionManager._components.slice(index, 1);
        }
    }

    public static clear(): void {
        CollisionManager._components = [];
    }

    public static update(time: number): void {
        CollisionManager._totalTime += time;

        for (const comp of CollisionManager._components) {
            for (const other of CollisionManager._components) {
                if (comp === other) {
                    continue;
                }

                if (comp.static && other.static) {
                    continue;
                }

                if (comp.shape.intersects(other.shape)) {
                    let exists: boolean = false;
                    for (const data of CollisionManager._collisionData) {
                        if ((data.a === comp && data.b === other) || (data.a === other && data.b === comp)) {
                            comp.onCollisionUpdate(other);
                            other.onCollisionUpdate(comp);
                            data.time = CollisionManager._totalTime;
                            exists = true;
                            break;
                        }
                    }

                    if (!exists) {
                        const collision = new CollisionData(CollisionManager._totalTime, comp, other);
                        comp.onCollisionEnter(other);
                        other.onCollisionEnter(comp);
                        CollisionManager._collisionData.push(collision);

                        Message.sendPriority(MESSAGE_COLLISION_ENTER, undefined, collision);
                    }
                }
            }
        }

        const removeData: CollisionData[] = [];
        for (const data of CollisionManager._collisionData) {
            if (data.time !== CollisionManager._totalTime) {
                removeData.push(data);
            }
        }

        while (removeData.length !== 0) {
            const data = removeData.shift();
            const index = CollisionManager._collisionData.indexOf(data);
            CollisionManager._collisionData.splice(index, 1);

            data.a.onCollisionExit(data.b);
            data.b.onCollisionExit(data.a);
            Message.sendPriority(MESSAGE_COLLISION_EXIT, undefined, data);
        }
    }
}
