import { connect } from 'react-redux';
import cancelMoveWidget from 'actions/moveWidgetActions';
import { getShowSearchBox } from 'selectors/componentTree';
import { toggleComponentTreeSearchBox } from 'actions/componentTreeActions';

import MoveWidgetScreen from 'Components/WidgetsScreen/MoveWidgetScreen';

const mapStateToProps = state => ({
  showSearchBox: getShowSearchBox(state),
});

const mapDispatchToProps = dispatch => ({
  onCancel: () => {
    dispatch(cancelMoveWidget());
  },
  toggleSearchBoxVisibility: () => {
    dispatch(toggleComponentTreeSearchBox());
  },
});

const MoveWidgetScreenContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(MoveWidgetScreen);

export default MoveWidgetScreenContainer;
