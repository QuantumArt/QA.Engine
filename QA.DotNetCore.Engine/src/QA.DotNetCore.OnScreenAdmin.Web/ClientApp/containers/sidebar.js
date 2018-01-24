import { connect } from 'react-redux';
import _ from 'lodash';
import {
  toggleState,
  toggleLeftPosition,
  toggleRightPosition,
  toggleTab,
} from 'actions/sidebarActions';
import { getShowAllZones, getShowAllWidgets } from 'selectors/componentsHighlight';
import { ONSCREEN_FEATURES } from 'constants/features';
import { getAvailableFeatures } from 'utils/features';
import Sidebar from '../Components/Sidebar';


const availableFeatures = getAvailableFeatures();

const getShowTabs = availableFeatures && availableFeatures.length > 1;
const getWidgetsTabAvailable =
  availableFeatures && _.indexOf(availableFeatures, ONSCREEN_FEATURES.WIDGETS_MANAGEMENT) !== -1;
const getAbTestsTabAvailable =
availableFeatures && _.indexOf(availableFeatures, ONSCREEN_FEATURES.ABTESTS) !== -1;

console.log('availableFeatures', availableFeatures);

const mapStateToProps = state => ({
  opened: state.sidebar.opened,
  side: state.sidebar.side,
  showAllZones: getShowAllZones(state),
  showAllWidgets: getShowAllWidgets(state),
  activeTab: state.sidebar.activeTab,
  widgetScreenSearchText: state.sidebar.widgetScreenSearchText,
  showTabs: getShowTabs,
  widgetTabAvailable: getWidgetsTabAvailable,
  abTestsTabAvailable: getAbTestsTabAvailable,
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
