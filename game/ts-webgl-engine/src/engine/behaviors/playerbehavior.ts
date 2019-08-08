import { IBehaviorData, IBehaviorBuilder, IBehavior } from "./interface.js";
import { Vector2 } from "../math/vector2.js";
import { BaseBehavior } from "./basebehavior.js";
import { IMessageHandler } from "../message/messagehandler.js";
import { AnimatedSpriteComponent } from "../components/animtedspritecomponent.js";
import { Message } from "../message/message.js";
import { MESSAGE_MOUSE_DOWN } from "../input/inputmanager.js";
import { MESSAGE_COLLISION_ENTER, CollisionData } from "../collision/collisionmanager.js";
import { AudioManager } from "../audio/audiomanager.js";
import "../math/mathextension.js";

export class PlayerBehaviorData implements IBehaviorData {
    public name: string;

    public acceleration: Vector2 = new Vector2(0, 920);
    public animatedSpriteName: string;
    public playerCollisionComponent: string;
    public groundCollisionComponent: string;

    setFromJson(data: any): void {
        this.name = data.name;
        this.animatedSpriteName = data.animatedSpriteName;
        this.playerCollisionComponent = data.playerCollisionComponent;
        this.groundCollisionComponent = data.groundCollisionComponent;

        if (data.acceleration) {
            this.acceleration.setFromJson(data.acceleration);
        }
    }
}

export class PlayerBehaviorBuilder implements IBehaviorBuilder {
    public get type(): string {
        return "player";
    }

    public buildFromJson(json: any): IBehavior {
        const data = new PlayerBehaviorData();
        data.setFromJson(json);
        return new PlayerBehavior(data);
    }
}

export class PlayerBehavior extends BaseBehavior implements IMessageHandler {
    private _acceleration: Vector2;
    private _velocity: Vector2 = Vector2.zero;
    private _isAlive: boolean = true;

    private _animatedSpriteName: string;
    private _playerCollisionComponent: string;
    private _groundCollisionComponent: string;

    private _sprite: AnimatedSpriteComponent;

    public constructor(data: PlayerBehaviorData) {
        super(data);

        this._acceleration = data.acceleration;
        this._animatedSpriteName = data.animatedSpriteName;
        this._playerCollisionComponent = data.playerCollisionComponent;
        this._groundCollisionComponent = data.groundCollisionComponent;

        Message.subscribe(MESSAGE_MOUSE_DOWN, this);
        Message.subscribe(MESSAGE_COLLISION_ENTER, this);
    }

    public updateReady(): void {
        super.updateReady();
        this._sprite = this._owner.getComponentByName(this._animatedSpriteName) as AnimatedSpriteComponent;
        if (!this._sprite) {
            throw new Error(`AnimatedSpriteComponent ${this._animatedSpriteName} does not exist on the owner`);
        }
    }

    public update(time: number): void {
        if (!this._isAlive) {
            return;
        }

        const seconds = time / 1000;
        this._velocity.add(this._acceleration.clone().scale(seconds));

        this._velocity.y = Math.min(this._velocity.y, 400);

        if (this._owner.transform.position.y < -13) {
            this._owner.transform.position.y = -13;
            this._velocity.y = 0;
        }

        this._owner.transform.position.add(
            this._velocity
                .clone()
                .scale(seconds)
                .toVector3()
        );

        if (this._velocity.y < 0) {
            this._owner.transform.rotation.z -= Math.degToRad(600.0) * seconds;
            if (this._owner.transform.rotation.z < Math.degToRad(-20)) {
                this._owner.transform.rotation.z = Math.degToRad(-20);
            }
        }

        if (this.isFalling() || !this._isAlive) {
            this._owner.transform.rotation.z += Math.degToRad(480) * seconds;
            if (this._owner.transform.rotation.z > Math.degToRad(90)) {
                this._owner.transform.rotation.z = Math.degToRad(90);
            }
        }

        if (this.shouldNotFlap()) {
            this._sprite.stop();
        } else {
            if (!this._sprite.isPlaying) {
                this._sprite.play();
            }
        }

        super.update(time);
    }

    public onMessage(message: Message): void {
        if (message.code === MESSAGE_MOUSE_DOWN) {
            this.onFlap();
        } else if (message.code === MESSAGE_COLLISION_ENTER) {
            const data = message.context as CollisionData;
            if (data.a.name !== this._playerCollisionComponent && data.b.name !== this._playerCollisionComponent) {
                return;
            }
            if (data.a.name === this._groundCollisionComponent || data.b.name === this._groundCollisionComponent) {
                this.die();
                this.decelerate();
                Message.send("PLAYER_DEAD", this);
            }
        }
    }

    private isFalling(): boolean {
        return this._velocity.y > 220;
    }

    private shouldNotFlap(): boolean {
        return this._velocity.y > 220 || !this._isAlive;
    }

    private die(): void {
        this._isAlive = false;

        AudioManager.playSound("dead");
    }

    private decelerate(): void {
        this._acceleration.y = 0;
        this._velocity.y = 0;
    }

    private onFlap(): void {
        if (this._isAlive) {
            this._velocity.y = -280;
            AudioManager.playSound("flap");
        }
    }

    private onRestart(y: number): void {
        this._owner.transform.rotation.z = 0;
        this._owner.transform.position.set(32, y);
        this._velocity.set(0, 0);
        this._acceleration.set(0, 920);
        this._isAlive = true;
        this._sprite.play();
    }
}
