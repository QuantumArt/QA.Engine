import {
  TOGGLE_OPEN_STATE,
  TOGGLE_LEFT_POSITION,
  TOGGLE_RIGHT_POSITION,
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
