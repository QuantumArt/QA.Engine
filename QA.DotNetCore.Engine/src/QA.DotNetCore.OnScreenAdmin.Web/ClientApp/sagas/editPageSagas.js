import { select, put, takeEvery, all } from 'redux-saga/effects';
import { EDIT_PAGE_ACTIONS, CONTENT_META_INFO_ACTION } from '../actions/actionTypes';
import { qpFormCallback } from './qpFormSagas';
import { editPage as editPageQpForm } from '../articleManagement';

const abstractItemMetaInfoSelector = state => state.metaInfo.abstractItemMetaInfo;
const selfSource = 'meta_info_edit_page';

function* editPage() {
  // см metaInfoSagas.js
  yield put({
    type: CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_REQUESTED,
    source: selfSource,
  });
}

function* showQpForm(action) {
  if (action.source !== selfSource) { return; }
  const currentPageId = window.currentPageId;
  const abstractItemInfo = yield select(abstractItemMetaInfoSelector);
  editPageQpForm(currentPageId, qpFormCallback, abstractItemInfo);
  yield put({ type: EDIT_PAGE_ACTIONS.SHOW_QP_FORM });
}

function* watchEditPage() {
  yield takeEvery(EDIT_PAGE_ACTIONS.EDIT_PAGE, editPage);
}

function* watchGetAbstractItemInfoSuccess() {
  yield takeEvery(CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_SUCCESS, showQpForm);
}

export default function* rootSaga() {
  yield all([
    watchEditPage(),
    watchGetAbstractItemInfoSuccess(),
  ]);
}

