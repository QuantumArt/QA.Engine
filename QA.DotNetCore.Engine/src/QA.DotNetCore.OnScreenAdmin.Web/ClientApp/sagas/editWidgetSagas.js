import { select, put, takeEvery, all } from 'redux-saga/effects';
import _ from 'lodash';
import { EDIT_WIDGET_ACTIONS, CONTENT_META_INFO_ACTION } from '../actions/actionTypes';
import { qpFormCallback } from './qpFormSagas';

import { editWidget as editWidgetQpForm } from '../articleManagement';


const abstractItemMetaInfoSelector = state => state.metaInfo.abstractItemMetaInfo;
const currentEditingWidgetSelector = state =>
  _.find(state.componentTree.components, { onScreenId: state.articleManagement.editWidget.onScreenId });
const selfSource = 'meta_info_edit_widget';


function* editWidget(action) {
  console.log('editWidget saga');
  // см metaInfoSagas.js
  yield put({
    type: CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_REQUESTED,
    onScreenId: action.onScreenId,
    source: selfSource,
  });
}

function* showQpForm(action) {
  console.log('showQpForm');
  if (action.source !== selfSource) { return; }
  const widget = yield select(currentEditingWidgetSelector);
  console.log('widget', widget);
  const widgetId = widget.properties.widgetId;
  const abstractItemInfo = yield select(abstractItemMetaInfoSelector);
  editWidgetQpForm(widgetId, qpFormCallback, abstractItemInfo);
  yield put({ type: EDIT_WIDGET_ACTIONS.SHOW_QP_FORM });
}


function* watchEditWidget() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.EDIT_WIDGET, editWidget);
}

function* watchGetAbstractItemInfoSuccess() {
  yield takeEvery(CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_SUCCESS, showQpForm);
}


export default function* rootSaga() {
  yield all([
    watchEditWidget(),
    watchGetAbstractItemInfoSuccess(),
  ]);
}

