import { createSelector } from 'reselect';
import _ from 'lodash';
import { WIDGET_CREATION_MODE, WIDGET_CREATION_STEPS } from 'constants/widgetCreation';

const getIsActiveSelector = state => state.widgetCreation.isActive;
const getCurrentStepSelector = state => state.widgetCreation.currentStep;
const getAvailableWidgetsLoadedSelector = state => state.widgetCreation.availableWidgetsLoaded;
const getFlatComponentsSelector = state => state.componentTree.components;
const getCreationModeSelector = state => state.widgetCreation.creationMode;
const getParentOnScreenIdSelector = state => state.widgetCreation.parentOnScreenId;
const getZonesListSearchTextSelector = state => state.widgetCreation.zonesListSearchText;
const getCustomZoneNameSelector = state => state.widgetCreation.customZoneName;
const getTargetZoneNameSelector = state => state.widgetCreation.targetZoneName;
const getShowZonesListSearchBoxSelector = state => state.widgetCreation.showZonesListSearchBox;

const getParentAbstractItemIdSelector = (state) => {
  const targetZoneName = state.widgetCreation.targetZoneName;
  const isCustomZone = state.widgetCreation.isCustomTargetZone;
  const creationMode = state.widgetCreation.creationMode;
  const parentOnScreenId = state.widgetCreation.parentOnScreenId;

  if (!targetZoneName) { return null; }
  if (creationMode === WIDGET_CREATION_MODE.PAGE_CHILD) {
    if (isCustomZone) { return window.currentPageId; }
    const targetZone = _.find(getFlatComponentsSelector(state), c => c.parentOnScreenId === 'page' && c.properties.zoneName === targetZoneName);
    return targetZone.properties.parentAbstractItemId;
  }
  const component = _.find(getFlatComponentsSelector(state), { onScreenId: parentOnScreenId });
  switch (creationMode) {
    case WIDGET_CREATION_MODE.SPECIFIC_ZONE:
      return component.properties.parentAbstractItemId;
    case WIDGET_CREATION_MODE.WIDGET_CHILD:
      return component.properties.widgetId;
    default:
      return null;
  }
};


export const getIsActive = createSelector(
  [getIsActiveSelector],
  isActive => isActive,
);

export const getZonesListSearchText = createSelector(
  [getZonesListSearchTextSelector],
  searchText => searchText,
);

export const getShowZonesListSearchBox = createSelector(
  [getShowZonesListSearchBoxSelector],
  show => show,
);

export const getCustomZoneName = createSelector(
  [getCustomZoneNameSelector],
  customZoneName => customZoneName,
);

export const getCreationMode = createSelector(
  [getCreationModeSelector],
  creationMode => creationMode,
);

export const getTargetZoneName = createSelector(
  [getTargetZoneNameSelector],
  targetZoneName => targetZoneName,
);

export const getParentAbstractItemId = createSelector(
  [getParentAbstractItemIdSelector],
  parentAbstractItemId => parentAbstractItemId,
);

export const getShowZoneTypeSelect = createSelector(
  [getIsActiveSelector, getCurrentStepSelector],
  (isActive, currentStep) => isActive && currentStep === WIDGET_CREATION_STEPS.ZONE_TYPE_SELECT,
);

export const getShowZonesList = createSelector(
  [getIsActiveSelector, getCurrentStepSelector],
  (isActive, currentStep) => isActive && currentStep === WIDGET_CREATION_STEPS.ZONES_LIST,
);

export const getShowEnterCustomZoneName = createSelector(
  [getIsActiveSelector, getCurrentStepSelector],
  (isActive, currentStep) => isActive && currentStep === WIDGET_CREATION_STEPS.CUSTOM_ZONE_NAME_ENTER,
);

export const getShowAvailableWidgets = createSelector(
  [getIsActiveSelector, getCurrentStepSelector, getAvailableWidgetsLoadedSelector],
  (isActive, currentStep, availableWidgetsLoaded) =>
    isActive && currentStep === WIDGET_CREATION_STEPS.SHOW_AVAILABLE_WIDGETS && availableWidgetsLoaded,
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
