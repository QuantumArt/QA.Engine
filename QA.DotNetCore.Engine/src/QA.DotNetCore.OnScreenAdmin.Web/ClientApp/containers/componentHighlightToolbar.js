import { connect } from 'react-redux';
import { toggleAllWidgets, toggleAllZones } from '../actions/componentHighlightActions';
import { getShowAllZones, getShowAllWidgets } from '../selectors/componentsHighlight';

import ComponentHighlightToolbar from '../Components/ComponentHighlightToolbar';

const mapStateToProps = state => ({
  showAllZones: getShowAllZones(state),
  showAllWidgets: getShowAllWidgets(state),
});

const mapDispatchToProps = dispatch => ({
  toggleAllZones: () => {
    dispatch(toggleAllZones());
  },
  toggleAllWidgets: () => {
    dispatch(toggleAllWidgets());
  },
});

const ComponentHighlightToolbarContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentHighlightToolbar);

export default ComponentHighlightToolbarContainer;

