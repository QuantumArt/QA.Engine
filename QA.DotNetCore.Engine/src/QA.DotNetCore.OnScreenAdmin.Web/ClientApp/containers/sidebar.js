import { connect } from 'react-redux';
import {
  toggleState,
  toggleLeftPosition,
  toggleRightPosition,
  toggleTab,
} from 'actions/sidebarActions';
import { getShowAllZones, getShowAllWidgets } from 'selectors/componentsHighlight';

import {
  availableFeatures,
  getShowTabs,
  getWidgetsTabAvailable,
  getAbTestsTabAvailable,
  getActiveTabIndex,
} from 'selectors/sidebar';
import Sidebar from 'Components/Sidebar';


const mapStateToProps = state => ({
  opened: state.sidebar.opened,
  side: state.sidebar.side,
  showAllZones: getShowAllZones(state),
  showAllWidgets: getShowAllWidgets(state),
  activeTab: getActiveTabIndex(state),
  widgetScreenSearchText: state.sidebar.widgetScreenSearchText,
  showTabs: getShowTabs(),
  featuresCount: availableFeatures.length,
  widgetTabAvailable: getWidgetsTabAvailable(),
  abTestsTabAvailable: getAbTestsTabAvailable(),
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
