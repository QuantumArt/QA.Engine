import { connect } from 'react-redux';
import cancelMoveWidget from 'actions/moveWidgetActions';

import MoveWidgetScreen from 'Components/WidgetsScreen/MoveWidgetScreen';


const mapDispatchToProps = dispatch => ({
  onCancel: () => {
    dispatch(cancelMoveWidget());
  },
});

const MoveWidgetScreenContainer = connect(
  null,
  mapDispatchToProps,
)(MoveWidgetScreen);

export default MoveWidgetScreenContainer;
