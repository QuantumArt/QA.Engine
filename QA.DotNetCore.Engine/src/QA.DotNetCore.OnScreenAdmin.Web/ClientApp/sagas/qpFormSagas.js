import _ from 'lodash/core';
import { select, put, take, takeEvery, all } from 'redux-saga/effects';
import { channel } from 'redux-saga';
import { QP_FORM_ACTIONS } from '../actions/actionTypes';


const getNeedReload = state => state.articleManagement.needReload;
const qpFormChannel = channel();
const qpFormActionsNeedReload = [
  'update_article',
  'update_article_and_up',
  'move_to_archive_article',
  'remove_article',
  'save_article',
];

export function qpFormCallback(eventType, details) {
  console.log('qpFormCallback', eventType, details);
  if (eventType === 1) { // host unbinded
    if (details.reason === 'closed') { // closed 
      qpFormChannel.put({ type: QP_FORM_ACTIONS.CLOSE_QP_FORM, eventType, details });
    }
  }
  if (eventType === 2) { // action executed
    if (_.includes(qpFormActionsNeedReload, details.actionCode)) {
      qpFormChannel.put({ type: QP_FORM_ACTIONS.NEED_RELOAD });
    }
  }
}


function* qpFormClosed(action) {
  console.log('qpFormClosed', action);
  const isReloadNeeded = yield select(getNeedReload);
  if (isReloadNeeded) {
    location.reload();
  }
}

function* watchQpFormClosed() {
  yield takeEvery(QP_FORM_ACTIONS.CLOSE_QP_FORM, qpFormClosed);
}

function* watchQpFormChannel() {
  while (true) {
    const action = yield take(qpFormChannel);
    yield put(action);
  }
}

export default function* watchQpForm() {
  yield all([
    watchQpFormClosed(),
    watchQpFormChannel(),
  ]);
}
