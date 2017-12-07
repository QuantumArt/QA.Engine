import { select, call, put, takeEvery, all } from 'redux-saga/effects';
import { CONTENT_META_INFO_ACTION } from '../actions/actionTypes';
import { getAvailableWidgets as apiGetAvailableWidgets, getMeta as apiGetMeta } from '../api';

const availableWidgetsSelector = state => state.metaInfo.availableWidgets;
const abstractItemMetaInfoSelector = state => state.metaInfo.abstractItemMetaInfo;

function* getAvailableWidgets(action) {
  console.log('getAvailableWidgets');
  try {
    const cachedAvailableWidgets = yield select(availableWidgetsSelector);
    if (cachedAvailableWidgets) {
      yield put({
        type: CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_SUCCESS,
        availableWidgets: cachedAvailableWidgets,
        source: action.source,
      });
    } else {
      const availableWidgetsInfo = yield call(apiGetAvailableWidgets);
      yield put({
        type: CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_SUCCESS,
        availableWidgets: availableWidgetsInfo.data.data,
        source: action.source,
      });
    }
  } catch (e) {
    yield put({
      type: CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_FAIL,
      error: e,
      source: action.source,
    });
  }
}


function* getAbstractItemInfo(action) {
  console.log('getAbstractItemInfo');
  try {
    const cachedAbstractItemInfo = yield select(abstractItemMetaInfoSelector);
    console.log('cached:', cachedAbstractItemInfo);
    if (cachedAbstractItemInfo) {
      yield put({
        type: CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_SUCCESS,
        info: cachedAbstractItemInfo,
        source: action.source });
    } else {
      const abstractItemInfo = yield call(apiGetMeta, 'QPAbstractItem');
      yield put({
        type: CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_SUCCESS,
        info: abstractItemInfo.data.data,
        source: action.source });
    }
  } catch (e) {
    yield put({
      type: CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_FAIL,
      error: e,
      source: action.source });
  }
}

function* watchGetAvailableWidgets() {
  yield takeEvery(CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_REQUESTED, getAvailableWidgets);
}

function* watchGetAbstractItemInfo() {
  yield takeEvery(CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_REQUESTED, getAbstractItemInfo);
}

export default function* rootSaga() {
  yield all([
    watchGetAvailableWidgets(),
    watchGetAbstractItemInfo(),
  ]);
}
