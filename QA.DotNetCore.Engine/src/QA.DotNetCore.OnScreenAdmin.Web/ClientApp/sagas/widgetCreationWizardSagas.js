import _ from 'lodash';
import { select, put, takeEvery, all, call } from 'redux-saga/effects';
import { WIDGETS_SCREEN_MODE_ACTIONS, CONTENT_META_INFO_ACTION, ADD_WIDGET_ACTIONS } from 'actions/actionTypes';
import {
  BEGIN_WIDGET_CREATION,
  GO_TO_PREV_STEP,
  SELECT_TARGET_ZONE,
  AVAILABLE_WIDGETS_LOADED,
  SELECT_WIDGET_TYPE,
  FINISH_WIDGET_CREATION,
} from 'actions/widgetCreation/actionTypes';
import { WIDGET_CREATION_MODE } from 'constants/widgetCreation';
import { getIsActive, getCreationMode, getTargetZoneName, getParentAbstractItemId } from 'selectors/widgetCreation';

import { qpFormCallback } from './qpFormSagas';

import { addWidget as addWidgetQpForm } from '../articleManagement';

const selfSource = 'meta_info_add_widget';


const abstractItemMetaInfoSelector = state => state.metaInfo.abstractItemMetaInfo;
const widgetToAddSelector = state =>
  _.find(
    state.metaInfo.availableWidgets,
    { id: state.widgetCreation.selectedWidgetId });

function* prevStep() {
  const isActive = yield select(getIsActive);
  if (!isActive) { yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.HIDE_WIDGET_CREATION_WIZARD }); }
}

function* requestMetaInfo() {
  yield put({ type: CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_REQUESTED, source: selfSource });
  yield put({ type: CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_REQUESTED, source: selfSource });
}

function* beginWizard() {
  yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.SHOW_WIDGET_CREATION_WIZARD });
  const creationMode = yield select(getCreationMode);
  if (creationMode === WIDGET_CREATION_MODE.SPECIFIC_ZONE) {
    yield call(requestMetaInfo);
  }
}

function* finishWizard() {
  yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.HIDE_WIDGET_CREATION_WIZARD });
}

function* showAvailableWidgets(action) {
  if (action.source !== selfSource) { return; }
  yield put({ type: AVAILABLE_WIDGETS_LOADED });
}

function* selectWidgetType() {
  const widgetToAdd = yield select(widgetToAddSelector);
  const abstractItemInfo = yield select(abstractItemMetaInfoSelector);
  const zoneName = yield select(getTargetZoneName);
  const parentAbstractItemId = yield select(getParentAbstractItemId);
  // yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.HIDE_WIDGET_CREATION_WIZARD });
  addWidgetQpForm(widgetToAdd, zoneName, parentAbstractItemId, abstractItemInfo, qpFormCallback);
  yield put({ type: ADD_WIDGET_ACTIONS.SHOW_QP_FORM });
  yield put({ type: FINISH_WIDGET_CREATION });
}

function* watchPrevStep() {
  yield takeEvery(GO_TO_PREV_STEP, prevStep);
}

function* watchBeginWizard() {
  yield takeEvery(BEGIN_WIDGET_CREATION, beginWizard);
}

function* watchFinishWizard() {
  yield takeEvery(FINISH_WIDGET_CREATION, finishWizard);
}

function* watchSelectTargetZone() {
  yield takeEvery(SELECT_TARGET_ZONE, requestMetaInfo);
}

function* watchGetAvailableWidgetsSuccess() {
  yield takeEvery(CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_SUCCESS, showAvailableWidgets);
}

function* watchSelectWidgetType() {
  yield takeEvery(SELECT_WIDGET_TYPE, selectWidgetType);
}

export default function* rootSaga() {
  yield all([
    watchBeginWizard(),
    watchPrevStep(),
    watchSelectTargetZone(),
    watchGetAvailableWidgetsSuccess(),
    watchSelectWidgetType(),
    watchFinishWizard(),
  ]);
}

