import { MessageBus } from "./messagebus";
import { IMessageHandler } from "./messagehandler";

export enum MessagePriority {
    NORMAL,
    HIGH
}

export class Message {
    public code: string;
    public context: any;
    public sender: any;
    public priority: MessagePriority;

    /**
     *
     */
    public constructor(code: string, sender: any, context?: any, priority: MessagePriority = MessagePriority.NORMAL) {
        this.code = code;
        this.sender = sender;
        this.context = context;
        this.priority = priority;
    }

    public static send(code: string, sender: any, context?: any): void {
        MessageBus.post(new Message(code, sender, context, MessagePriority.NORMAL));
    }

    public static sendPriority(code: string, sender: any, context?: any): void {
        MessageBus.post(new Message(code, sender, context, MessagePriority.HIGH));
    }

    public static subscibe(code: string, handler: IMessageHandler): void {
        MessageBus.addSubScription(code, handler);
    }

    public static unsubscibe(code: string, handler: IMessageHandler): void {
        MessageBus.removeSubScription(code, handler);
    }
}
