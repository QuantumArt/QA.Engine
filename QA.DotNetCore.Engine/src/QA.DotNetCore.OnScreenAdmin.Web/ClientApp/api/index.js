
import axios from 'axios';
import qs from 'qs';
import * as Cookies from 'js-cookie';

const siteId = window.siteId;
const baseUrl = window.onScreenAdminBaseUrl;
const onScreenTokenCookieName = window.onScreenTokenCookieName;

const axiosConfig = {
  baseURL: `${baseUrl}/api`,
  headers: { 'X-QP8-Access-Token': Cookies.get(onScreenTokenCookieName) },
};

const axiosInstance = () => axios.create(axiosConfig);

export const getMeta = contentNetName => axiosInstance().get('/meta', {
  params: {
    siteId,
    contentNetName,
  },
});

export const getAvailableWidgets = () => axiosInstance().get('/availableWidgets', {
  params: {
    siteId,
  },
});

export const moveWidget = (options) => {
  axiosInstance().post('/MoveWidget', qs.stringify({
    widgetId: options.widgetId,
    newParentId: options.newParentId,
    zoneName: options.zoneName,
  },
  ));
};
