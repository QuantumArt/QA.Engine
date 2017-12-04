import { connect } from 'react-redux';
import {
  toggleState,
  toggleLeftPosition,
  toggleRightPosition,
  toggleAllZones,
  toggleTab,
} from '../actions/sidebarActions';
import Sidebar from '../Components/Sidebar';

const mapStateToProps = state => ({
  opened: state.sidebar.opened,
  side: state.sidebar.side,
  showAllZones: state.sidebar.showAllZones,
  activeTab: state.sidebar.activeTab,
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
  toggleTab: (value) => {
    dispatch(toggleTab(value));
  },
});

const SidebarContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(Sidebar);

export default SidebarContainer;
