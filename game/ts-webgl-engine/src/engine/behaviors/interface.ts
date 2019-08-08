import { SimObject } from "../world/simobject";

export interface IBehavior {
    name: string;

    setOwner(owner: SimObject): void;
    update(time: number): void;
    apply(userData: any): void;

    updateReady(): void;
}

export interface IBehaviorData {
    name: string;

    setFromJson(json: any): void;
}

export interface IBehaviorBuilder {
    readonly type: string;

    buildFromJson(data: any): IBehavior;
}
