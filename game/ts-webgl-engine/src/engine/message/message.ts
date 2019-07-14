export enum MessagePriority {
    NORMAL,
    HIGH
}

export class Message {
    public code: string;
    public context: any;
    public sender: any;
    public priority: MessagePriority;
}
