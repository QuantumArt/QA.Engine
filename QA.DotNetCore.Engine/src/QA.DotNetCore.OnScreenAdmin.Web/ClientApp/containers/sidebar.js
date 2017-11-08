import { connect } from 'react-redux';
import {
  toggleState,
  toggleLeftPosition,
  toggleRightPosition,
} from '../actions/sidebarActions';
import Sidebar from '../Components/Sidebar';

const mapStateToProps = state => ({
  opened: state.sidebar.opened,
  side: state.sidebar.side,
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
});

const SidebarContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(Sidebar);

export default SidebarContainer;
