import {
  ONSCREEN_TOGGLE_COMPONENT,
} from './actionTypes';

export default function onScreenToggleComponent(onScreenId) {
  return { type: ONSCREEN_TOGGLE_COMPONENT, onScreenId };
}
