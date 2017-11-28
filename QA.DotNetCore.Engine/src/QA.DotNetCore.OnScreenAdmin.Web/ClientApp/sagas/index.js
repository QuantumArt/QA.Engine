import { all } from 'redux-saga/effects';
import { watchSubtreeToggle } from './treeStateSagas';
import { watchEditWidget,
  watchGetAbstractItemInfo,
  watchGetAbstractItemInfoSuccess,
  watchQpFormChannel,
  watchQpFormClosed } from './editWidgetSagas';


export default function* rootSaga() {
  yield all([
    watchSubtreeToggle(),
    watchEditWidget(),
    watchGetAbstractItemInfo(),
    watchGetAbstractItemInfoSuccess(),
    watchQpFormChannel(),
    watchQpFormClosed(),
  ]);
}
