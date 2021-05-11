import { Photo } from "./photo";

    export interface Member {
        id: number;
        userName: string;
        age: number;
        photoUrl: string;
        knownAs: string;
        createdAt: Date;
        lastActive: Date;
        gender: string;
        introduction: string;
        lookingFor: string;
        interests: string;
        city: string;
        country: string;
        photos: Photo[];
    }
