
import axios from 'axios';

const siteId = window.siteId;
const baseUrl = window.onScreenAdminBaseUrl;

export const getMeta = contentNetName => axios.get(`${baseUrl}/api/meta`, {
  params: {
    siteId,
    contentNetName,
  },
});

export const getAvailableWidgets = () => axios.get(`${baseUrl}/api/availableWidgets`, {
  params: {
    siteId,
  },
})
;
