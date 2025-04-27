import * as signalR from "@microsoft/signalr";

type ReceiveMessageCallback = (userId: number, message: string, sentAt: string) => void;
type PriceUpdateCallback = (auctionId: number, newPrice: number, userId: number) => void;

class SignalRService {
    private connection: signalR.HubConnection | null;
    private isConnected: boolean;

    constructor() {
        this.connection = null;
        this.isConnected = false;
    }

    async startConnection(): Promise<void> {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7232/auctionHub", {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets,
            })
            .withAutomaticReconnect()
            .build();

        try {
            await this.connection.start();
            console.log("Connected to AuctionHub!");
            this.isConnected = true;
        } catch (err) {
            console.error("SignalR Connection Error:", err);
            this.isConnected = false;
            throw err;
        }
    }

    private async waitForConnection(): Promise<void> {
        if (!this.connection) {
            throw new Error("SignalR connection not initialized!");
        }

        const maxAttempts = 10;
        const delay = 500; // 500ms
        let attempts = 0;

        while (this.connection.state !== signalR.HubConnectionState.Connected && attempts < maxAttempts) {
            console.log(`Waiting for SignalR to connect... (Attempt ${attempts + 1}/${maxAttempts})`);
            await new Promise(resolve => setTimeout(resolve, delay));
            attempts++;
        }

        if (this.connection.state !== signalR.HubConnectionState.Connected) {
            throw new Error("SignalR failed to reach Connected state after maximum attempts.");
        }
    }

    onReceiveMessage(callback: ReceiveMessageCallback): void {
        if (this.connection) {
            this.connection.on("ReceiveMessage", (userId: number, message: string, sentAt: string) => {
                console.log("Received message:", userId, message, sentAt);
                callback(userId, message, sentAt);
            });
        }
    }

    onPriceUpdate(callback: PriceUpdateCallback): void {
        if (this.connection) {
            this.connection.on("PriceUpdate", (auctionId: number, newPrice: number, userId: number) => {
                console.log("Received price update:", auctionId, newPrice, userId);
                callback(auctionId, newPrice, userId);
            });
        }
    }

    async joinGroup(auctionId: number): Promise<void> {
        if (!this.connection) {
            console.error("SignalR connection not initialized!");
            return;
        }

        if (!this.isConnected) {
            console.log("SignalR not connected yet, waiting for connection...");
            await this.startConnection();
        }

        try {
            await this.waitForConnection(); // Chờ cho đến khi trạng thái là Connected
            await this.connection.invoke("JoinAuctionDetailGroup", auctionId);
            console.log(`Joined group Auction_${auctionId}`);
        } catch (err) {
            console.error("Failed to join group:", err);
        }
    }

    async leaveGroup(auctionId: number): Promise<void> {
        if (this.isConnected && this.connection) {
            await this.connection.invoke("LeaveAuctionDetailGroup", auctionId);
            console.log(`Left group Auction_${auctionId}`);
        } else {
            console.error("SignalR not connected!");
        }
    }

    stopConnection(): void {
        if (this.isConnected && this.connection) {
            this.connection.stop();
            this.isConnected = false;
            console.log("Disconnected from AuctionHub!");
        }
    }
}

export default new SignalRService();