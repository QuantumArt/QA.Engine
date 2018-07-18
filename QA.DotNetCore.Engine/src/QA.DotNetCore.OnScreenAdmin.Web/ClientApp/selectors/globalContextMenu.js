import { createSelector } from 'reselect';
import { ADD_WIDGET_TO_PAGE_KEY, TOGGLE_SHOW_ONLY_WIDGETS_KEY } from 'constants/globalContextMenu';
import { WIDGET_TAB } from 'constants/general';
import { getShowComponentTree } from './widgetsScreen';
import { getActiveTab } from './sidebar';


/* eslint-disable import/prefer-default-export */
export const getEnabledMenuKeys = createSelector(
  [getShowComponentTree, getActiveTab],
  (showComponentTree, activeTab) => {
    const result = [];
    if (showComponentTree && activeTab === WIDGET_TAB) {
      result.push(ADD_WIDGET_TO_PAGE_KEY);
      result.push(TOGGLE_SHOW_ONLY_WIDGETS_KEY);
    }
    return result;
  },
);

