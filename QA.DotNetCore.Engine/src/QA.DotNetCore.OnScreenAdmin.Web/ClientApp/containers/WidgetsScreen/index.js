import { connect } from 'react-redux';
import {
  getShowComponentTree,
  getShowWidgetsCreationWizard,
  getShowMoveWidgetScreen,
} from 'selectors/widgetsScreen';
import WidgetsScreen from 'Components/WidgetsScreen';

const mapStateToProps = state => ({
  showComponentTree: getShowComponentTree(state),
  showWidgetCreationWizard: getShowWidgetsCreationWizard(state),
  showMoveWidgetScreen: getShowMoveWidgetScreen(state),
});

const WidgetsScreenContainer = connect(
  mapStateToProps,
  null,
)(WidgetsScreen);

export default WidgetsScreenContainer;
