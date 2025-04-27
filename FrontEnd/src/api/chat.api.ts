import baseApi from './base.api';

interface ChatMessage {
    userId: number;
    messageText: string;
    sentAt: string;
}

interface SendChatMessageModel {
    auctionId: number;
    userId: number;
    message: string;
}

const chatApi = {
    sendChatMessage: async (data: SendChatMessageModel) => {
        return await baseApi.post('Chat/SendChatMessage', data);
    },
    getChatMessages: async (auctionId: number) => {
        return await baseApi.get(`Chat/GetChatMessages/${auctionId}`);
    },
};

export default chatApi;