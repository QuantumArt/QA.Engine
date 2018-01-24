import { connect } from 'react-redux';
import {
  getShowComponentTree,
  getShowWidgetsCreationWizard,
  getShowMoveWidgetScreen,
} from 'selectors/widgetsScreen';
import { beginWidgetCreation } from 'actions/widgetCreation/actions';
import { WIDGET_CREATION_MODE } from 'constants/widgetCreation';
import WidgetsScreen from 'Components/WidgetsScreen';

const mapStateToProps = state => ({
  showComponentTree: getShowComponentTree(state),
  showWidgetCreationWizard: getShowWidgetsCreationWizard(state),
  showMoveWidgetScreen: getShowMoveWidgetScreen(state),
});

const mapDispatchToProps = dispatch => ({
  addWidgetToPage: () => {
    const payload = {
      creationMode: WIDGET_CREATION_MODE.PAGE_CHILD,
    };
    dispatch(beginWidgetCreation(payload));
  },
});

const WidgetsScreenContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(WidgetsScreen);

export default WidgetsScreenContainer;
