import { call, select, takeLatest } from 'redux-saga/effects';
import { TOGGLE_SUBTREE, TOGGLE_FULL_SUBTREE } from 'actions/actionTypes';
import { setSubtreeState } from 'utils/componentTreeStateStorage';

export const getComponents = state => state.componentTree.components;

function* saveSubtreeOpenState() {
  try {
    const components = yield select(getComponents);
    yield call(setSubtreeState, components);
  } catch (error) {
    // do nothing
  }
}

export function* watchSubtreeToggle() {
  yield takeLatest([TOGGLE_SUBTREE, TOGGLE_FULL_SUBTREE], saveSubtreeOpenState);
}
