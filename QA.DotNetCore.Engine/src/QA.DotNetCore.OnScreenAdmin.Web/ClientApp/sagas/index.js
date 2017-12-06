import { all } from 'redux-saga/effects';
import { watchSubtreeToggle } from './treeStateSagas';
import watchEditWidgetSaga from './editWidgetSagas';
import watchAddWidgetSaga from './addWidgetSagas';
import watchQpForm from './qpFormSagas';
import watchMetaInfo from './metaInfoSagas';


export default function* rootSaga() {
  yield all([
    watchSubtreeToggle(),
    watchEditWidgetSaga(),
    watchAddWidgetSaga(),
    watchQpForm(),
    watchMetaInfo(),
  ]);
}
