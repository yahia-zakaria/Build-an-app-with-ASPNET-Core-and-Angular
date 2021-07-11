    export interface Message {
        id: number;
        senderId: number;
        recipientId: number;
        senderUsername: string;
        recipientUsername: string;
        senderPhotoUrl: string;
        recipientPhotoUrl: string;
        content: string;
        dateRead?: Date;
        messageSent: Date;
    }
