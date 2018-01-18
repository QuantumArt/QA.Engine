import { connect } from 'react-redux';
import {
  getShowComponentTree,
  getShowAvailableWidgets,
  getShowMoveWidgetScreen,
} from 'selectors/widgetsScreen';
import WidgetsScreen from 'Components/WidgetsScreen';

const mapStateToProps = state => ({
  showComponentTree: getShowComponentTree(state),
  showAvailableWidgets: getShowAvailableWidgets(state),
  showMoveWidgetScreen: getShowMoveWidgetScreen(state),
});

const WidgetsScreenContainer = connect(
  mapStateToProps,
  null,
)(WidgetsScreen);

export default WidgetsScreenContainer;
