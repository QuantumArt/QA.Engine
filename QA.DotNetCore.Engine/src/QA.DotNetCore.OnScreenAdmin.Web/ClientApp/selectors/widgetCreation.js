import { createSelector } from 'reselect';
import _ from 'lodash';
import { WIDGET_CREATION_MODE } from 'constants/widgetCreation';

const getIsActiveSelector = state => state.widgetCreation.isActive;
const getIsTargetZoneDefinedSelector = state => _.isString(state.widgetCreation.targetZoneName)
                                        && !_.isEmpty(state.widgetCreation.targetZoneName);

const getIsCustomZoneSelector = state => state.widgetCreation.isCustomTargetZone;
const getAvailableWidgetsLoadedSelector = state => state.widgetCreation.availableWidgetsLoaded;

const getFlatComponentsSelector = state => state.componentTree.components;
const getCreationModeSelector = state => state.widgetCreation.creationMode;
const getParentOnScreenIdSelector = state => state.widgetCreation.parentOnScreenId;
const getZonesListSearchTextSelector = state => state.widgetCreation.zonesListSearchText;


export const getIsActive = createSelector(
  [getIsActiveSelector],
  isActive => isActive,
);

export const getZonesListSearchText = createSelector(
  [getZonesListSearchTextSelector],
  searchText => searchText,
);

export const getShowZonesList = createSelector(
  [getIsActiveSelector, getIsTargetZoneDefinedSelector, getIsCustomZoneSelector],
  (isActive, targetZoneDefined, isCustomZone) => isActive && !targetZoneDefined && !isCustomZone,
);

export const getShowEnterCustomZoneName = createSelector(
  [getIsActiveSelector, getIsTargetZoneDefinedSelector, getIsCustomZoneSelector],
  (isActive, targetZoneDefined, isCustomZone) => isActive && !targetZoneDefined && isCustomZone,
);

export const getShowAvailableWidgets = createSelector(
  [getIsActiveSelector, getIsTargetZoneDefinedSelector, getAvailableWidgetsLoadedSelector],
  (isActive, targetZoneDefined, availableWidgetsLoaded) => isActive && targetZoneDefined && availableWidgetsLoaded,
);

export const getZonesList = createSelector(
  [getCreationModeSelector, getFlatComponentsSelector, getParentOnScreenIdSelector, getZonesListSearchTextSelector],
  (creationMode, flatComponents, parentOnScreenId, searchText) => {
    switch (creationMode) {
      case WIDGET_CREATION_MODE.WIDGET_CHILD:
        return _.filter(flatComponents, component =>
          component.type === 'zone'
          && component.parentOnScreenId === parentOnScreenId
          && _.includes(_.toLower(component.properties.zoneName), _.toLower(searchText)));
      case WIDGET_CREATION_MODE.PAGE_CHILD:
        return _.filter(flatComponents, component =>
          component.type === 'zone'
          && component.parentOnScreenId === 'page'
          && _.includes(_.toLower(component.properties.zoneName), _.toLower(searchText)));
      default:
        return [];
    }
  },
);
