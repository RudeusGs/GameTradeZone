// profile.api.js
import baseApi from './base.api';

const profile = {
    getAllProfile: async () => {
        return await baseApi.get('WebsiteAccount/Get-All');
    },
    getByIdProfile: async (id: number) => {
        return await baseApi.get(`WebsiteAccount/Get-By-Id?id=${id}`);
    },
};

export default profile;
