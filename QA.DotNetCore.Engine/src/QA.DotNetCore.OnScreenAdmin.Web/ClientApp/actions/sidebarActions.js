import {
  TOGGLE_OPEN_STATE,
  TOGGLE_LEFT_POSITION,
  TOGGLE_RIGHT_POSITION,
  TOGGLE_TAB,
  SAVE_CORDS,
} from './actionTypes';

export function toggleState() {
  return { type: TOGGLE_OPEN_STATE };
}

export function toggleLeftPosition() {
  return { type: TOGGLE_LEFT_POSITION };
}

export function toggleRightPosition() {
  return { type: TOGGLE_RIGHT_POSITION };
}

export function toggleTab(value) {
  return { type: TOGGLE_TAB, value };
}

export function saveCords(cx, cy, nx, ny) {
  return {
    type: SAVE_CORDS,
    payload: {
      componentX: cx,
      componentY: cy,
      nodeX: nx,
      nodeY: ny,
    },
  };
}
