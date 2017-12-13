import { connect } from 'react-redux';
import {
  toggleState,
  toggleLeftPosition,
  toggleRightPosition,
  toggleTab,
} from '../actions/sidebarActions';
import Sidebar from '../Components/Sidebar';
import { getShowAllZones, getShowAllWidgets } from '../selectors/componentsHighlight';

const mapStateToProps = state => ({
  opened: state.sidebar.opened,
  side: state.sidebar.side,
  showAllZones: getShowAllZones(state),
  showAllWidgets: getShowAllWidgets(state),
  activeTab: state.sidebar.activeTab,
  widgetScreenSearchText: state.sidebar.widgetScreenSearchText,
});

const mapDispatchToProps = dispatch => ({
  toggleSidebar: () => {
    dispatch(toggleState());
  },
  toggleLeft: () => {
    dispatch(toggleLeftPosition());
  },
  toggleRight: () => {
    dispatch(toggleRightPosition());
  },
  toggleTab: (value) => {
    dispatch(toggleTab(value));
  },

});

const SidebarContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(Sidebar);

export default SidebarContainer;
