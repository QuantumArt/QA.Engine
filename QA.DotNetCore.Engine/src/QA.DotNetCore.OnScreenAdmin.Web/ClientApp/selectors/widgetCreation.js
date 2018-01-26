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
const getCustomZoneNameSelector = state => state.widgetCreation.customZoneName;
const getSelectedWidgetIdSelector = state => state.widgetCreation.selectedWidgetId;

const getTargetZoneNameSelector = state => state.widgetCreation.targetZoneName;

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

export const getShowZonesList = createSelector(
  [getIsActiveSelector, getIsTargetZoneDefinedSelector, getIsCustomZoneSelector],
  (isActive, targetZoneDefined, isCustomZone) => isActive && !targetZoneDefined && !isCustomZone,
);

export const getShowEnterCustomZoneName = createSelector(
  [getIsActiveSelector, getIsTargetZoneDefinedSelector, getIsCustomZoneSelector],
  (isActive, targetZoneDefined, isCustomZone) => isActive && !targetZoneDefined && isCustomZone,
);

export const getShowAvailableWidgets = createSelector(
  [getIsActiveSelector, getIsTargetZoneDefinedSelector, getAvailableWidgetsLoadedSelector, getSelectedWidgetIdSelector],
  (isActive, targetZoneDefined, availableWidgetsLoaded, selectedWidgetId) =>
    isActive && targetZoneDefined && availableWidgetsLoaded && (selectedWidgetId === null),
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
