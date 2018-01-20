import { createSelector } from 'reselect';
import _ from 'lodash';

const getIsActiveSelector = state => state.widgetCreation.isActive;
const getIsTargetZoneDefinedSelector = state => _.isString(state.widgetCreation.targetZoneName)
                                        && !_.isEmpty(state.widgetCreation.targetZoneName);

const getIsCustomZoneSelector = state => state.widgetCreation.isCustomTargetZone;
const getAvailableWidgetsLoadedSelector = state => state.widgetCreation.availableWidgetsLoaded;


export const getIsActive = createSelector(
  [getIsActiveSelector],
  isActive => isActive,
);

export const showZonesList = createSelector(
  [getIsActiveSelector, getIsTargetZoneDefinedSelector, getIsCustomZoneSelector],
  (isActive, targetZoneDefined, isCustomZone) => isActive && !targetZoneDefined && !isCustomZone,
);

export const showEnterCustomZoneName = createSelector(
  [getIsActiveSelector, getIsTargetZoneDefinedSelector, getIsCustomZoneSelector],
  (isActive, targetZoneDefined, isCustomZone) => isActive && !targetZoneDefined && isCustomZone,
);

export const showAvailableWidgets = createSelector(
  [getIsActiveSelector, getIsTargetZoneDefinedSelector, getAvailableWidgetsLoadedSelector],
  (isActive, targetZoneDefined, availableWidgetsLoaded) => isActive && targetZoneDefined && availableWidgetsLoaded,
);
