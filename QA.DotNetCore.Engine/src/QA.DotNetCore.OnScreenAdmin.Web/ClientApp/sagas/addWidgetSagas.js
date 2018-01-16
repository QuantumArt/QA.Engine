import _ from 'lodash';
import { select, put, takeEvery, all } from 'redux-saga/effects';
import { ADD_WIDGET_ACTIONS, CONTENT_META_INFO_ACTION, WIDGETS_SCREEN_MODE_ACTIONS } from '../actions/actionTypes';
import { qpFormCallback } from './qpFormSagas';

import { addWidget as addWidgetQpForm } from '../articleManagement';

const selfSource = 'meta_info_add_widget';

const abstractItemMetaInfoSelector = state => state.metaInfo.abstractItemMetaInfo;
const widgetToAddSelector = state =>
  _.find(
    state.metaInfo.availableWidgets,
    { id: state.articleManagement.addWidget.selectedWidgetId });
const zoneToAddSelector = state =>
  _.find(
    state.componentTree.components,
    { onScreenId: state.articleManagement.addWidget.zoneOnScreenId });


function* addWidget() {
  console.log('addWidget saga');
  // см metaInfoSagas.js
  yield put({ type: CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_REQUESTED, source: selfSource });
  yield put({ type: CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_REQUESTED, source: selfSource });
}

function* widgetSelected() {
  console.log('widget selected saga');
  const zoneToAdd = yield select(zoneToAddSelector);
  const widgetToAdd = yield select(widgetToAddSelector);
  const abstractItemInfo = yield select(abstractItemMetaInfoSelector);
  yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.HIDE_AVAILABLE_WIDGETS });
  addWidgetQpForm(widgetToAdd, zoneToAdd, abstractItemInfo, qpFormCallback);
  yield put({ type: ADD_WIDGET_ACTIONS.SHOW_QP_FORM });
  console.log(zoneToAdd, widgetToAdd, abstractItemInfo);
}

function* showAvailableWidgets(action) {
  if (action.source !== selfSource) { return; }
  yield put({ type: WIDGETS_SCREEN_MODE_ACTIONS.SHOW_AVAILABLE_WIDGETS });
}

function* watchAddWidget() {
  yield takeEvery(ADD_WIDGET_ACTIONS.ADD_WIDGET_TO_ZONE, addWidget);
}

function* watchGetAvailableWidgetsSuccess() {
  yield takeEvery(CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_SUCCESS, showAvailableWidgets);
}

function* watchSelectWidgetToAdd() {
  yield takeEvery(ADD_WIDGET_ACTIONS.SELECT_WIDGET_TO_ADD, widgetSelected);
}

export default function* rootSaga() {
  yield all([
    watchAddWidget(),
    watchGetAvailableWidgetsSuccess(),
    watchSelectWidgetToAdd(),
  ]);
}
