import { connect } from 'react-redux';
import cancelMoveWidget from 'actions/moveWidgetActions';
import { getShowSearchBoxSelector } from 'selectors/componentTree';
import { toggleComponentTreeSearchBox } from 'actions/componentTreeActions';

import MoveWidgetScreen from 'Components/WidgetsScreen/MoveWidgetScreen';

const mapStateToProps = state => ({
  showSearchBox: getShowSearchBoxSelector(state),
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
