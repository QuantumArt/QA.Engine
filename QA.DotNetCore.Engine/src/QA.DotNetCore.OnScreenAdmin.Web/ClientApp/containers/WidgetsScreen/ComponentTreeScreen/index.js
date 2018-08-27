import { connect } from 'react-redux';
import ComponentTreeScreen from 'Components/WidgetsScreen/ComponentTreeScreen';
import { getShowSearchBoxSelector, getShowOnlyWidgetsSelector } from 'selectors/componentTree';
import { toggleComponentTreeSearchBox } from 'actions/componentTreeActions';

const mapStateToProps = state => ({
  showSearchBox: getShowSearchBoxSelector(state),
  showOnlyWidgets: getShowOnlyWidgetsSelector(state),
});

const mapDispatchToProps = dispatch => ({
  toggleSearchBoxVisibility: () => {
    dispatch(toggleComponentTreeSearchBox());
  },
});

const ComponentTreeScreenContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentTreeScreen);

export default ComponentTreeScreenContainer;
