import { select, call, put, takeEvery } from 'redux-saga/effects';
import {
  EDIT_WIDGET,
  GET_ABSTRACT_ITEM_INFO_REQUESTED,
  GET_ABSTRACT_ITEM_INFO_SUCCESS,
  GET_ABSTRACT_ITEM_INFO_FAIL,
  EDIT_WIDGET_SHOW_QP_FORM,
  // EDIT_WIDGET_SHOW_QP_FORM,
  // EDIT_WIDGET_CLOSE_QP_FORM,
} from '../actions/actionTypes';

import { getMeta } from '../api';
import editWidgetQpForm from '../articleManagement';


const getAbstractItemMetaInfo = state => state.componentTree.abstractItemMetaInfo;
const getCurrentEditingWidgetId = state => state.componentTree.editingComponentId;


function* editWidget(action) {
  console.log('editWidget saga');
  yield put({ type: GET_ABSTRACT_ITEM_INFO_REQUESTED, id: action.id });
}

function* getAbstractItemInfo() {
  console.log('getAbstractItemInfo');

  try {
    const cachedAbstractItemInfo = yield select(getAbstractItemMetaInfo);
    console.log('cached:', cachedAbstractItemInfo);
    if (cachedAbstractItemInfo) {
      yield put({ type: GET_ABSTRACT_ITEM_INFO_SUCCESS, info: cachedAbstractItemInfo });
    } else {
      const abstractItemInfo = yield call(getMeta, 'QPAbstractItem');
      yield put({ type: GET_ABSTRACT_ITEM_INFO_SUCCESS, info: abstractItemInfo.data.data });
    }
  } catch (e) {
    yield put({ type: GET_ABSTRACT_ITEM_INFO_FAIL, error: e });
  }
}

function* showQpForm() {
  console.log('showQpForm');
  const widgetId = yield select(getCurrentEditingWidgetId);
  const abstractItemInfo = yield select(getAbstractItemMetaInfo);
  editWidgetQpForm(widgetId, (x, y) => console.log(x, y), abstractItemInfo);
  yield put({ type: EDIT_WIDGET_SHOW_QP_FORM });
}

export function* watchEditWidget() {
  yield takeEvery(EDIT_WIDGET, editWidget);
}


export function* watchGetAbstractItemInfo() {
  yield takeEvery(GET_ABSTRACT_ITEM_INFO_REQUESTED, getAbstractItemInfo);
}

export function* watchGetAbstractItemInfoSuccess() {
  yield takeEvery(GET_ABSTRACT_ITEM_INFO_SUCCESS, showQpForm);
}

