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
    AddAuction: async (FormData: FormData) => {
        return await baseApi.get('Auction/Add_Auction');
    },
    AddAuctionDetail: async (FormData: FormData) => {
        return await baseApi.get('Auction/Add-Auction_Detail');
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