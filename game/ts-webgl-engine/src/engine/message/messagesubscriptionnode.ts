import { Message } from "./message.js";
import { IMessageHandler } from "./messagehandler.js";

export class MessageSubscriptionNode {
    public constructor(public message: Message, public handler: IMessageHandler) {}
}
