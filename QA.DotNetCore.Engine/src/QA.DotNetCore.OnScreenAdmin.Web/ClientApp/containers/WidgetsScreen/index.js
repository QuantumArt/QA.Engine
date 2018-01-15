import { connect } from 'react-redux';
import {
  getShowComponentTree,
  getShowAvailableWidgets,
} from 'selectors/widgetsScreen';
import WidgetsScreen from 'Components/WidgetsScreen';

const mapStateToProps = state => ({
  showComponentTree: getShowComponentTree(state),
  showAvailableWidgets: getShowAvailableWidgets(state),
});

const WidgetsScreenContainer = connect(
  mapStateToProps,
  null,
)(WidgetsScreen);

export default WidgetsScreenContainer;
