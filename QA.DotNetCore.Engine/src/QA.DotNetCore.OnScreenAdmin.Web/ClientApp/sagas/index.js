import { all } from 'redux-saga/effects';
import { watchSubtreeToggle } from './treeStateSagas';
import { watchEditWidget, watchGetAbstractItemInfo, watchGetAbstractItemInfoSuccess } from './editWidgetSagas';


export default function* rootSaga() {
  yield all([
    watchSubtreeToggle(),
    watchEditWidget(),
    watchGetAbstractItemInfo(),
    watchGetAbstractItemInfoSuccess(),
  ]);
}
