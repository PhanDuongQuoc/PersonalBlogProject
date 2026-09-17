import * as signalR from "@microsoft/signalr";
import type { ContactMessage } from "@/types/admin-contact";

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private isConnecting = false;
  private newContactCallbacks: Array<(msg: ContactMessage) => void> = [];
  private statusUpdateCallbacks: Array<(data: { id: number; status: string }) => void> = [];

  private getHubUrl(): string {
    const rawApiUrl = (import.meta.env.QCLI_API_URL as string) || "http://localhost:5148/api";
    const baseUrl = rawApiUrl.replace(/\/api\/?$/, "");
    return `${baseUrl}/hub/notifications`;
  }

  public async start(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    if (this.isConnecting) {
      return;
    }

    try {
      this.isConnecting = true;
      const hubUrl = this.getHubUrl();

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
          skipNegotiation: false,
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Warning)
        .build();

      // Register Listeners
      this.connection.on("ReceiveNewContactMessage", (msg: ContactMessage) => {
        this.newContactCallbacks.forEach((cb) => {
          try {
            cb(msg);
          } catch (e) {
            console.error("Error executing ReceiveNewContactMessage callback:", e);
          }
        });
      });

      this.connection.on("ReceiveContactStatusUpdate", (data: { id: number; status: string }) => {
        this.statusUpdateCallbacks.forEach((cb) => {
          try {
            cb(data);
          } catch (e) {
            console.error("Error executing ReceiveContactStatusUpdate callback:", e);
          }
        });
      });

      this.connection.onreconnecting((error) => {
        console.warn("SignalR reconnecting due to:", error);
      });

      this.connection.onreconnected((connectionId) => {
        console.log("SignalR reconnected successfully. ID:", connectionId);
      });

      this.connection.onclose((error) => {
        console.log("SignalR connection closed:", error);
        this.isConnecting = false;
      });

      await this.connection.start();
      console.log("SignalR NotificationHub connected successfully to:", hubUrl);
    } catch (err) {
      console.warn("Could not connect to SignalR NotificationHub:", err);
    } finally {
      this.isConnecting = false;
    }
  }

  public async stop(): Promise<void> {
    if (this.connection) {
      try {
        await this.connection.stop();
      } catch (e) {
        console.error("Error stopping SignalR connection:", e);
      } finally {
        this.connection = null;
      }
    }
  }

  public onNewContactMessage(callback: (msg: ContactMessage) => void): void {
    if (!this.newContactCallbacks.includes(callback)) {
      this.newContactCallbacks.push(callback);
    }
  }

  public offNewContactMessage(callback: (msg: ContactMessage) => void): void {
    this.newContactCallbacks = this.newContactCallbacks.filter((cb) => cb !== callback);
  }

  public onContactStatusUpdate(callback: (data: { id: number; status: string }) => void): void {
    if (!this.statusUpdateCallbacks.includes(callback)) {
      this.statusUpdateCallbacks.push(callback);
    }
  }

  public offContactStatusUpdate(callback: (data: { id: number; status: string }) => void): void {
    this.statusUpdateCallbacks = this.statusUpdateCallbacks.filter((cb) => cb !== callback);
  }
}

export const signalRService = new SignalRService();
