import { takeEvery, all, put, select, call } from 'redux-saga/effects';
import { EDIT_WIDGET_ACTIONS } from 'actions/actionTypes';
import { getMovingWidgetTargetZoneSelector, movingWidgetSelector } from 'selectors/componentTree';
import { moveWidget as apiMoveWidget } from '../api';


function* selectTargetZone() {
  const targetZone = yield select(getMovingWidgetTargetZoneSelector);
  const movingWidget = yield select(movingWidgetSelector);
  console.log('select target zone', movingWidget);
  yield put({ type: EDIT_WIDGET_ACTIONS.MOVING_WIDGET_REQUESTED,
    options: {
      widgetId: movingWidget.properties.widgetId,
      newParentId: targetZone.properties.parentAbstractItemId,
      zoneName: targetZone.properties.zoneName,
    } });
}

function* moveWidgetRequested(action) {
  try {
    console.log('move request', action);
    const result = yield call(apiMoveWidget, action.options);
    yield put({ type: EDIT_WIDGET_ACTIONS.MOVING_WIDGET_SUCCEEDED, data: result });
  } catch (e) {
    yield put({
      type: EDIT_WIDGET_ACTIONS.MOVING_WIDGET_FAILED,
      error: e,
    });
  }
}

function* moveWidgetSucceeded() {
  yield call(location.reload());
}

function* moveWidgetFailed(action) {
  console.log('error: ', action.error);
  yield put({ type: EDIT_WIDGET_ACTIONS.FINISH_MOVING_WIDGET });
}


function* watchSelectTargetZone() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.MOVING_WIDGET_SELECT_TARGET_ZONE, selectTargetZone);
}

function* watchMoveWidgetRequested() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.MOVING_WIDGET_REQUESTED, moveWidgetRequested);
}

function* watchMoveWidgetSucceeded() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.MOVING_WIDGET_SUCCEEDED, moveWidgetSucceeded);
}

function* watchMoveWidgetFailed() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.MOVING_WIDGET_FAILED, moveWidgetFailed);
}


export default function* rootSaga() {
  yield all([
    watchSelectTargetZone(),
    watchMoveWidgetRequested(),
    watchMoveWidgetSucceeded(),
    watchMoveWidgetFailed(),
  ]);
}

