import { select, call, put, takeEvery, all } from 'redux-saga/effects';
import { EDIT_WIDGET_ACTIONS } from '../actions/actionTypes';
import { qpFormCallback } from './qpFormSagas';

import { getMeta } from '../api';
import editWidgetQpForm from '../articleManagement';


const getAbstractItemMetaInfo = state => state.componentTree.abstractItemMetaInfo;
const getCurrentEditingWidgetId = state => state.componentTree.editingComponentId;


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

function* showQpForm() {
  console.log('showQpForm');
  const widgetId = yield select(getCurrentEditingWidgetId);
  const abstractItemInfo = yield select(getAbstractItemMetaInfo);
  editWidgetQpForm(widgetId, (x, y) => qpFormCallback(x, y), abstractItemInfo);
  yield put({ type: EDIT_WIDGET_ACTIONS.SHOW_QP_FORM });
}


function* watchEditWidget() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.EDIT_WIDGET, editWidget);
}

function* watchGetAbstractItemInfo() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_REQUESTED, getAbstractItemInfo);
}

function* watchGetAbstractItemInfoSuccess() {
  yield takeEvery(EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_SUCCESS, showQpForm);
}


export default function* rootSaga() {
  yield all([
    watchEditWidget(),
    watchGetAbstractItemInfo(),
    watchGetAbstractItemInfoSuccess(),
  ]);
}

