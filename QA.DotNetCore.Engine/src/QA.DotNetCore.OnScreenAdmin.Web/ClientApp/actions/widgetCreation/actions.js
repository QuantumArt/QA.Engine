import { BEGIN_WIDGET_CREATION, SELECT_TARGET_ZONE, SELECT_CUSTOM_ZONE, GO_TO_PREV_STEP, CHANGE_ZONES_LIST_SEARCH_TEXT } from './actionTypes';

export function selectTargetZone(targetZoneName) {
  return {
    type: SELECT_TARGET_ZONE,
    payload: {
      targetZoneName,
    },
  };
}

export function selectCustomZone() {
  return {
    type: SELECT_CUSTOM_ZONE,
  };
}

export function beginWidgetCreation({ creationMode, parentOnScreenId, targetZoneName }) {
  return {
    type: BEGIN_WIDGET_CREATION,
    payload: {
      creationMode,
      parentOnScreenId,
      targetZoneName,
    },
  };
}

export function changeZonesListSearchText(newValue) {
  return {
    type: CHANGE_ZONES_LIST_SEARCH_TEXT,
    payload: {
      newValue,
    },
  };
}

export function goToPrevStep() {
  return {
    type: GO_TO_PREV_STEP,
  };
}
