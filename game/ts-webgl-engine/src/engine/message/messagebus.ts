import { IMessageHandler } from "./messagehandler.js";
import { MessageSubscriptionNode } from "./messagesubscriptionnode.js";
import { Message, MessagePriority } from "./message.js";

export class MessageBus {
    private static _subscriptions: { [code: string]: IMessageHandler[] } = {};

    private static _normalQueueMessagePerUpdate: number = 10;
    private static _normalQueue: MessageSubscriptionNode[] = [];

    private constructor() {}

    public static addSubScription(code: string, handler: IMessageHandler): void {
        if (MessageBus._subscriptions[code] === undefined) {
            MessageBus._subscriptions[code] = [];
        }

        if (MessageBus._subscriptions[code].indexOf(handler) !== -1) {
            console.warn(`Attempting to add duplicated handle to code: ${code}`);
        } else {
            MessageBus._subscriptions[code].push(handler);
        }
    }

    public static removeSubScription(code: string, handler: IMessageHandler): void {
        if (MessageBus._subscriptions[code] !== undefined) {
            console.warn(`Cannot remove handle from code: ${code} since it's not subscribed.`);
            return;
        }

        const index = MessageBus._subscriptions[code].indexOf(handler);
        if (index !== -1) {
            MessageBus._subscriptions[code].splice(index, 1);
        }
    }

    public static post(message: Message): void {
        console.log(`Post message:`, message);

        const handlers = MessageBus._subscriptions[message.code];
        if (handlers !== undefined) {
            for (const handler of handlers) {
                if (message.priority === MessagePriority.HIGH) {
                    handler.onMessage(message);
                } else {
                    MessageBus._normalQueue.push(new MessageSubscriptionNode(message, handler));
                }
            }
        }
    }

    public static update(time: number): void {
        if (MessageBus._normalQueue.length === 0) {
            return;
        }

        const messageLimit = Math.min(MessageBus._normalQueueMessagePerUpdate, MessageBus._normalQueue.length);

        for (let i = 0; i < messageLimit; ++i) {
            const node = MessageBus._normalQueue.shift();
            node.handler.onMessage(node.message);
        }
    }
}
