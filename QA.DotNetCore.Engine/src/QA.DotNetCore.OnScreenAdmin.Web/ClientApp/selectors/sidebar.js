import { createSelector } from 'reselect';
import _ from 'lodash';
import { getAvailableFeatures } from 'utils/features';
import { ONSCREEN_FEATURES } from 'constants/features';
import { ABTESTS_TAB, WIDGET_TAB } from 'constants/general';

export const availableFeatures = getAvailableFeatures();

const getShowTabsSelector = () => availableFeatures && availableFeatures.length > 1;
const getWidgetsTabAvailableSelector = () => availableFeatures &&
  _.indexOf(availableFeatures, ONSCREEN_FEATURES.WIDGETS_MANAGEMENT) !== -1;
const getAbTestsTabAvailableSelector = () => availableFeatures &&
  _.indexOf(availableFeatures, ONSCREEN_FEATURES.ABTESTS) !== -1;

const getSidebarSideSelector = state => state.sidebar.side;
const getOpenedSelector = state => state.sidebar.opened;
const getActiveTabIndexSelector = state => state.sidebar.activeTab;
const getCordsSelector = state => state.sidebar.cords;

export const getSidebarSide = createSelector(getSidebarSideSelector, _.identity);
export const getOpened = createSelector(getOpenedSelector, _.identity);
export const getShowTabs = createSelector(getShowTabsSelector, _.identity);
export const getWidgetsTabAvailable = createSelector(getWidgetsTabAvailableSelector, _.identity);
export const getAbTestsTabAvailable = createSelector(getAbTestsTabAvailableSelector, _.identity);
export const getActiveTabIndex = createSelector(getActiveTabIndexSelector, _.identity);
export const getCords = createSelector(getCordsSelector, _.identity);

// адский костыль
export const getActiveTab = createSelector(
  getActiveTabIndexSelector,
  (tabIndex) => {
    const activeTabFeature = availableFeatures[tabIndex];
    switch (activeTabFeature) {
      case ONSCREEN_FEATURES.WIDGETS_MANAGEMENT:
        return WIDGET_TAB;
      case ONSCREEN_FEATURES.ABTESTS:
        return ABTESTS_TAB;
      default:
        return null;
    }
  },
);
