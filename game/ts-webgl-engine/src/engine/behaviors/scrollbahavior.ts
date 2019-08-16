import { IBehaviorBuilder, IBehavior, IBehaviorData } from "./interface.js";
import { Vector2 } from "../math/vector2.js";
import { IMessageHandler } from "../message/messagehandler.js";
import { BaseBehavior } from "./basebehavior.js";
import { Message } from "../message/message.js";

export class ScrollBehaviorData implements IBehaviorData {
    public name: string;
    public velocity: Vector2 = Vector2.zero;
    public minPosition: Vector2 = Vector2.zero;
    public resetPosition: Vector2 = Vector2.zero;
    public minResetY: number;
    public maxResetY: number;
    public startMessage: string;
    public stopMessage: string;
    public resetMessage: string;

    public setFromJson(data: any): void {
        this.name = data.name;
        this.velocity.setFromJson(data.velocity);
        this.minPosition.setFromJson(data.minPosition);
        this.resetPosition.setFromJson(data.resetPosition);

        this.startMessage = data.startMessage;
        this.stopMessage = data.stopMessage;
        this.resetMessage = data.resetMessage;
    }
}

export class ScrollBehaviorBuilder implements IBehaviorBuilder {
    public get type(): string {
        return "scroll";
    }

    public buildFromJson(json: any): IBehavior {
        const data = new ScrollBehaviorData();
        data.setFromJson(json);
        return new ScrollBehavior(data);
    }
}

export class ScrollBehavior extends BaseBehavior implements IMessageHandler {
    private _velocity: Vector2 = Vector2.zero;
    private _minPosition: Vector2 = Vector2.zero;
    private _resetPosition: Vector2 = Vector2.zero;

    private _startMessage: string;
    private _stopMessage: string;
    private _resetMessage: string;

    private _isScrolling: boolean = false;
    private _initialPosition: Vector2 = Vector2.zero;

    constructor(data: ScrollBehaviorData) {
        super(data);

        this._velocity.copyFrom(data.velocity);
        this._minPosition.copyFrom(data.minPosition);
        this._resetPosition.copyFrom(data.resetPosition);

        this._startMessage = data.startMessage;
        this._stopMessage = data.stopMessage;
        this._resetMessage = data.resetMessage;
    }

    public updateReady(): void {
        super.updateReady();

        if (this._startMessage) {
            Message.subscribe(this._startMessage, this);
        }

        if (this._stopMessage) {
            Message.subscribe(this._stopMessage, this);
        }

        if (this._resetMessage) {
            Message.subscribe(this._resetMessage, this);
        }

        this._initialPosition.copyFrom(this._owner.transform.position.toVector2());
    }

    public update(time: number): void {
        if (this._isScrolling) {
            this._owner.transform.position.add(
                this._velocity
                    .clone()
                    .scale(time / 1000)
                    .toVector3()
            );

            if (
                this._owner.transform.position.x <= this._minPosition.x &&
                this._owner.transform.position.y <= this._minPosition.y
            ) {
                this.reset();
            }
        }
    }

    public onMessage(message: Message): void {
        if (message.code === this._startMessage) {
            this._isScrolling = true;
        } else if (message.code === this._stopMessage) {
            this._isScrolling = false;
        } else if (message.code === this._resetMessage) {
            this.init();
        }
    }

    private reset(): void {
        this._owner.transform.position.copyFrom(this._resetPosition.toVector3());
    }

    private init(): void {
        this._owner.transform.position.copyFrom(this._initialPosition.toVector3());
    }
}
