import { connect } from 'react-redux';
import {
  toggleState,
  toggleLeftPosition,
  toggleRightPosition,
  toggleTab,
  saveCords,
} from 'actions/sidebarActions';
import { getShowAllZones, getShowAllWidgets } from 'selectors/componentsHighlight';

import {
  availableFeatures,
  getShowTabs,
  getWidgetsTabAvailable,
  getAbTestsTabAvailable,
  getActiveTabIndex,
  getCords,
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
  cords: getCords(state),
});

const bindActions = {
  toggleSidebar: toggleState,
  toggleLeft: toggleLeftPosition,
  toggleRight: toggleRightPosition,
  toggleTab,
  saveCords,
};

const SidebarContainer = connect(
  mapStateToProps,
  bindActions,
)(Sidebar);

export default SidebarContainer;
