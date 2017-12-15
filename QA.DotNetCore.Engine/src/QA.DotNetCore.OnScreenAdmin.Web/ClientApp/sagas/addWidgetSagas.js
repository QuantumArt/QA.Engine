import { select, call, put, takeEvery, all } from 'redux-saga/effects';
import { ADD_WIDGET_ACTIONS } from '../actions/actionTypes';
// import { qpFormCallback } from './qpFormSagas';

import { getAvailableWidgets as getAvailableWidgetsFromServer } from '../api';
// import { addWidget as addWidgetQpForm } from '../articleManagement';

const getAvailableWidgetsFromStore = state => state.componentTree.availableWidgetsInfo;

function* addWidget() {
  console.log('addWidget saga');
  yield put({ type: ADD_WIDGET_ACTIONS.GET_AVAILABLE_WIDGETS_REQUESTED });
}

function* getAvailableWidgets() {
  console.log('getAvailableWidgets');
  try {
    const cachedAvailableWidgets = yield select(getAvailableWidgetsFromStore);
    if (cachedAvailableWidgets) {
      yield put({ type: ADD_WIDGET_ACTIONS.GET_AVAILABLE_WIDGETS_SUCCESS, availableWidgets: cachedAvailableWidgets });
    } else {
      const availableWidgetsInfo = yield call(getAvailableWidgetsFromServer);
      yield put({
        type: ADD_WIDGET_ACTIONS.GET_AVAILABLE_WIDGETS_SUCCESS,
        availableWidgets: availableWidgetsInfo.data.data,
      });
    }
  } catch (e) {
    yield put({ type: ADD_WIDGET_ACTIONS.GET_AVAILABLE_WIDGETS_FAIL, error: e });
  }
}

function* showAvailableWidgets() {
  yield put({ type: ADD_WIDGET_ACTIONS.SHOW_AVAILABLE_WIDGETS });
}

function* watchAddWidget() {
  yield takeEvery(ADD_WIDGET_ACTIONS.ADD_WIDGET_TO_ZONE, addWidget);
}

function* watchGetAvailableWidgets() {
  yield takeEvery(ADD_WIDGET_ACTIONS.GET_AVAILABLE_WIDGETS_REQUESTED, getAvailableWidgets);
}

function* watchGetAvailableWidgetsSuccess() {
  yield takeEvery(ADD_WIDGET_ACTIONS.GET_AVAILABLE_WIDGETS_SUCCESS, showAvailableWidgets);
}

export default function* rootSaga() {
  yield all([
    watchAddWidget(),
    watchGetAvailableWidgets(),
    watchGetAvailableWidgetsSuccess(),
  ]);
}
