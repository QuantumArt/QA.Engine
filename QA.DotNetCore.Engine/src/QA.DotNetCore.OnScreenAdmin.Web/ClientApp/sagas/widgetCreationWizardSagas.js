// import _ from 'lodash';
import { select, put, takeEvery, all } from 'redux-saga/effects';
import { WIDGETS_SCREEN_MODE_ACTIONS } from 'actions/actionTypes';
import { BEGIN_WIDGET_CREATION, GO_TO_PREV_STEP } from 'actions/widgetCreation/actionTypes';
import { getIsActive } from 'selectors/widgetCreation';


function* beginWizard() {
  yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.SHOW_WIDGET_CREATION_WIZARD });
}

function* prevStep() {
  const isActive = yield select(getIsActive);
  if (!isActive) { yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.HIDE_WIDGET_CREATION_WIZARD }); }
}

function* watchPrevStep() {
  yield takeEvery(GO_TO_PREV_STEP, prevStep);
}

function* watchBeginWizard() {
  yield takeEvery(BEGIN_WIDGET_CREATION, beginWizard);
}

export default function* rootSaga() {
  yield all([
    watchBeginWizard(),
    watchPrevStep(),
  ]);
}

