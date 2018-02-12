import { APP_STARTED, CONTENT_META_INFO_ACTION, WIDGETS_SCREEN_MODE_ACTIONS } from 'actions/actionTypes';
import { put, takeEvery, all } from 'redux-saga/effects';

const selfSource = 'app_start';

function* appStart() {
  yield put({ type: CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_REQUESTED, source: selfSource });
}

function* availableWidgetsSuccess(action) {
  if (action.source !== selfSource) { return; }
  yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.AVAILABLE_WIDGETS_INFO_LOADED });
}

function* availableWidgetsFail(action) {
  if (action.source !== selfSource) { return; }
  yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.AVAILABLE_WIDGETS_INFO_LOADED });
}

function* watchAppStart() {
  yield takeEvery(APP_STARTED, appStart);
}

function* watchGetAvailableWidgetsSuccess() {
  yield takeEvery(CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_SUCCESS, availableWidgetsSuccess);
}

function* watchGetAvailableWidgetsFail() {
  yield takeEvery(CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_FAIL, availableWidgetsFail);
}

export default function* rootSaga() {
  yield all([
    watchAppStart(),
    watchGetAvailableWidgetsSuccess(),
    watchGetAvailableWidgetsFail(),
  ]);
}
