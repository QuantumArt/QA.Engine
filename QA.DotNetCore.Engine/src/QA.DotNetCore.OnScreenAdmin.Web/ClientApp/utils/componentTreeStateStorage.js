import store from 'store';
import _ from 'lodash/core';

export const setSubtreeState = (components) => {
  const currentPageId = window.currentPageId;
  const key = `subtrees_state_${currentPageId}`;
  const filtered = _.filter(components, { isOpened: true });
  const mapped = _.map(filtered, 'onScreenId');
  const currentData = {
    openedNodes: mapped,
  };
  store.set(key, currentData);
};

export const getSubtreeState = () => {
  const currentPageId = window.currentPageId;
  const key = `subtrees_state_${currentPageId}`;
  return store.get(key);
};
