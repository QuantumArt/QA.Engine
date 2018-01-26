import {
  TOGGLE_ALL_ZONES,
  TOGGLE_ALL_WIDGETS,
} from './actionTypes';

export function toggleAllZones() {
  return { type: TOGGLE_ALL_ZONES };
}

export function toggleAllWidgets() {
  return { type: TOGGLE_ALL_WIDGETS };
}
