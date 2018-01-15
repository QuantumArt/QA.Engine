
import axios from 'axios';
import qs from 'qs';

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
});

export const moveWidget = (options) => {
  axios.post(`${baseUrl}/api/MoveWidget`, qs.stringify({
    widgetId: options.widgetId,
    newParentId: options.newParentId,
    zoneName: options.zoneName,
  },
  ));
};
