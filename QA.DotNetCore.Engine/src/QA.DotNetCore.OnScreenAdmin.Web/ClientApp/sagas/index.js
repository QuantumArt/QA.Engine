import { all } from 'redux-saga/effects';
import { watchSubtreeToggle } from './treeStateSagas';
import watchEditWidgetSaga from './editWidgetSagas';
// import watchAddWidgetSaga from './addWidgetSagas';
import watchQpForm from './qpFormSagas';
import watchMetaInfo from './metaInfoSagas';
import watchOnScreen from './onScreenEditControls';
import watchMoveWidget from './moveWidgetSagas';
import watchAbTests from './abTestsSagas';
import watchWidgetCreationWizard from './widgetCreationWizardSagas';

export default function* rootSaga() {
  yield all([
    watchSubtreeToggle(),
    watchEditWidgetSaga(),
    // watchAddWidgetSaga(),
    watchQpForm(),
    watchMetaInfo(),
    watchOnScreen(),
    watchAbTests(),
    watchMoveWidget(),
    watchWidgetCreationWizard(),
  ]);
}
