import { all } from 'redux-saga/effects';
import { watchSubtreeToggle } from './treeStateSagas';
import watchEditWidgetSaga from './editWidgetSagas';
import watchQpForm from './qpFormSagas';


export default function* rootSaga() {
  yield all([
    watchSubtreeToggle(),
    watchEditWidgetSaga(),
    watchQpForm(),
  ]);
}
