import { connect } from 'react-redux';
import {
  toggleState,
  toggleLeftPosition,
  toggleRightPosition,
  toggleAllZones,
  toggleAllWidgets,
  toggleTab,
  widgetScreenChangeSearchText,
} from '../actions/sidebarActions';
import Sidebar from '../Components/Sidebar';

const mapStateToProps = state => ({
  opened: state.sidebar.opened,
  side: state.sidebar.side,
  showAllZones: state.sidebar.showAllZones,
  showAllWidgets: state.sidebar.showAllWidgets,
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
  toggleAllZones: () => {
    dispatch(toggleAllZones());
  },
  toggleAllWidgets: () => {
    dispatch(toggleAllWidgets());
  },
  toggleTab: (value) => {
    dispatch(toggleTab(value));
  },
  widgetScreenChangeSearchText: (value) => {
    dispatch(widgetScreenChangeSearchText(value));
  },
});

const SidebarContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(Sidebar);

export default SidebarContainer;
