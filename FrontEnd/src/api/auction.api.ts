import baseApi from './base.api';

const auction = {
    getAllAuction: async () => {
        return await baseApi.get('Auction/Get-All_Auction');
    },
    getAllAuctionDetail: async () => {
        return await baseApi.get('Auction/Get-All_Auction_Detail');
    },
    getAllAuctionPrize: async () => {
        return await baseApi.get('Auction/Get-All_AuctionPrize');
    },
    AddAuction: async (formData: FormData) => {
        return await baseApi.post('Auction/Add_Auction', formData);
    },
    // AddAuctionDetail: async (formData: FormData) => {
    //     return await baseApi.post('Auction/Add-Auction-Detail', formData);
    // },
    AddAuctionDetail: async (data: { AuctionId: number; RaisePrice: string }) => {
        return await baseApi.post('Auction/Add-Auction-Detail', data);
    },
    GetAuctionById: async (id: number) => {
        return await baseApi.get(`Auction/Get-Auction-By-Id?id=${id}`);
    },
    GetAuctionDetailById: async (id: number) => {
        return await baseApi.get(`Auction/Get-Auction-Detail-By-Id?id=${id}`);
    },
    GetAuctionPrizeById: async (id: number) => {
        return await baseApi.get(`Auction/Get-Auction-Prize-By-Id?id=${id}`);   
    },
};

export default auction;