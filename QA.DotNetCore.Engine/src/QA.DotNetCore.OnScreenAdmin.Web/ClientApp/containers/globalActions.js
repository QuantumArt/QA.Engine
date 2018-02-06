import { connect } from 'react-redux';
import { toggleAllWidgets, toggleAllZones } from 'actions/componentHighlightActions';
import { getShowAllZones, getShowAllWidgets } from 'selectors/componentsHighlight';
import { getEnabledMenuKeys } from 'selectors/globalContextMenu';
import { beginWidgetCreation } from 'actions/widgetCreation/actions';
import { WIDGET_CREATION_MODE } from 'constants/widgetCreation';

import GlobalActions from 'Components/Sidebar/GlobalActions';

const mapStateToProps = state => ({
  showAllZones: getShowAllZones(state),
  showAllWidgets: getShowAllWidgets(state),
  enabledMenuKeys: getEnabledMenuKeys(state),
});

const mapDispatchToProps = dispatch => ({
  toggleAllZones: () => {
    dispatch(toggleAllZones());
  },
  toggleAllWidgets: () => {
    dispatch(toggleAllWidgets());
  },
  addWidgetToPage: () => {
    const payload = {
      creationMode: WIDGET_CREATION_MODE.PAGE_CHILD,
    };
    dispatch(beginWidgetCreation(payload));
  },
});

const GlobalActionsContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(GlobalActions);

export default GlobalActionsContainer;

