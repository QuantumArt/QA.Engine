import _ from 'lodash';
import { select, call, put, take, takeEvery } from 'redux-saga/effects';
import { channel } from 'redux-saga';
import { EDIT_WIDGET_ACTIONS } from '../actions/actionTypes';

import { getMeta } from '../api';
import editWidgetQpForm from '../articleManagement';


const getAbstractItemMetaInfo = state => state.componentTree.abstractItemMetaInfo;
const getCurrentEditingWidgetId = state => state.componentTree.editingComponentId;
const getNeedReload = state => state.componentTree.needReload;
const qpFormChannel = channel();

const qpFormActionsNeedReload = [
  'update_article',
  'update_article_and_up',
  'move_to_archive_article',
  'remove_article',
];

function* editWidget(action) {
  console.log('editWidget saga');
  yield put({ type: EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_REQUESTED, id: action.id });
}

function* getAbstractItemInfo() {
  console.log('getAbstractItemInfo');

  try {
    const cachedAbstractItemInfo = yield select(getAbstractItemMetaInfo);
    console.log('cached:', cachedAbstractItemInfo);
    if (cachedAbstractItemInfo) {
      yield put({ type: EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_SUCCESS, info: cachedAbstractItemInfo });
    } else {
      const abstractItemInfo = yield call(getMeta, 'QPAbstractItem');
      yield put({ type: EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_SUCCESS, info: abstractItemInfo.data.data });
    }
  } catch (e) {
    yield put({ type: EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_FAIL, error: e });
  }
}

function qpFormCallback(eventType, details) {
  console.log('qpFormCallback', eventType, details);
  if (eventType === 1) { // host unbinded
    if (details.reason === 'closed') { // closed without saving
      qpFormChannel.put({ type: EDIT_WIDGET_ACTIONS.CLOSE_QP_FORM, eventType, details });
    }
  }
  if (eventType === 2) { // action executed
    if (_.includes(qpFormActionsNeedReload, details.actionCode)) {
      qpFormChannel.put({ type: EDIT_WIDGET_ACTIONS.NEED_RELOAD });
    }
  }
}

function* showQpForm() {
  console.log('showQpForm');
  const widgetId = yield select(getCurrentEditingWidgetId);
  const abstractItemInfo = yield select(getAbstractItemMetaInfo);
  editWidgetQpForm(widgetId, (x, y) => qpFormCallback(x, y), abstractItemInfo);
  yield put({ type: EDIT_WIDGET_ACTIONS.SHOW_QP_FORM });
}

function* qpFormClosed(action) {
  console.log('qpFormClosed', action);
  const isReloadNeeded = yield select(getNeedReload);
  if (isReloadNeeded) {
    location.reload();
  }
}

export function* watchEditWidget() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.EDIT_WIDGET, editWidget);
}

export function* watchGetAbstractItemInfo() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_REQUESTED, getAbstractItemInfo);
}

export function* watchGetAbstractItemInfoSuccess() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_SUCCESS, showQpForm);
}

export function* watchQpFormClosed() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.CLOSE_QP_FORM, qpFormClosed);
}

export function* watchQpFormChannel() {
  while (true) {
    const action = yield take(qpFormChannel);
    yield put(action);
  }
}

