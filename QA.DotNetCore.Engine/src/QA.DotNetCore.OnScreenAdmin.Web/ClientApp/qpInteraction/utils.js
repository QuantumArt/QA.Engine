const guidRegexp = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
// const HOSTID_SESSION_STORAGE_KEY = '7C46280A-A276-4F1A-8FA3-053B38959E09';

export const isGuid = guid => (guid ? guidRegexp.test(guid) : false);

export const getNewUid = () => `uid${(new Date()).getTime()}_${Math.floor(Math.random() * 10000)}`;

const getParameterByName = (name, url) => {
  if (!url) url = window.location.href;
  name = name.replace(/[[\]]/g, '\\$&');
  const regex = new RegExp(`[?&]${name}(=([^&#]*)|&|#|$)`);
  const results = regex.exec(url);
  if (!results) return null;
  if (!results[2]) return '';
  return decodeURIComponent(results[2].replace(/\+/g, ' '));
};

export const getHostId = () => {
  if (isGuid(window.name) === false) {
    console.log('window.name is not a guid. taking from url and setting window.name');
    const hostId = getParameterByName('hostUID');
    window.name = hostId;
    return hostId;
  }
  return window.name;
};

export const initHostId = () => {
  if (isGuid(window.name) === false) {
    const hostId = getParameterByName('hostUID');
    window.name = hostId;
  }
};

export const getParent = () => window.top;
