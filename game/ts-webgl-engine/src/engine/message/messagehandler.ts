import { Message } from "./message.js";

export interface IMessageHandler {
    onMessage(message: Message): void;
}
