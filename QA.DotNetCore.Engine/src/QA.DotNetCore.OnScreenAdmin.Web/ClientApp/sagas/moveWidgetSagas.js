import { takeEvery, all, put, select, call } from 'redux-saga/effects';
import { EDIT_WIDGET_ACTIONS, WIDGETS_SCREEN_MODE_ACTIONS } from 'actions/actionTypes';
import { getMovingWidgetTargetZoneSelector, movingWidgetSelector } from 'selectors/componentTree';
import { moveWidget as apiMoveWidget } from '../api';

function* selectTargetZone() {
  const targetZone = yield select(getMovingWidgetTargetZoneSelector);
  const movingWidget = yield select(movingWidgetSelector);
  yield put({ type: EDIT_WIDGET_ACTIONS.MOVING_WIDGET_REQUESTED,
    options: {
      widgetId: movingWidget.properties.widgetId,
      newParentId: targetZone.properties.parentAbstractItemId,
      zoneName: targetZone.properties.zoneName,
    },
  });
}

function* moveWidgetRequested(action) {
  try {
    const result = yield call(apiMoveWidget, action.options);
    yield put({ type: EDIT_WIDGET_ACTIONS.MOVING_WIDGET_SUCCEEDED, data: result });
  } catch (error) {
    yield put({ type: EDIT_WIDGET_ACTIONS.MOVING_WIDGET_FAILED, error });
  }
}

function moveWidgetSucceeded() {
  location.reload();
}

function* moveWidgetFailed() {
  yield put({ type: EDIT_WIDGET_ACTIONS.FINISH_MOVING_WIDGET });
}

function* showMoveWidget() {
  yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.SHOW_MOVE_WIDGET });
}

function* cancelMoveWidget() {
  yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.HIDE_MOVE_WIDGET });
  yield put({ type: EDIT_WIDGET_ACTIONS.FINISH_MOVING_WIDGET });
}


function* watchMoveWidget() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.MOVE_WIDGET, showMoveWidget);
}

function* watchCancelMoveWidget() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.CANCEL_MOVING_WIDGET, cancelMoveWidget);
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
    watchMoveWidget(),
    watchCancelMoveWidget(),
  ]);
}

