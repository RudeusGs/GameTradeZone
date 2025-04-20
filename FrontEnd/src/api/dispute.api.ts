import baseApi from './base.api';

const dispute = {
    Add: async (disputeData: any) => {
        return await baseApi.post('Dispute/Add', disputeData);
    }
}

export default dispute;