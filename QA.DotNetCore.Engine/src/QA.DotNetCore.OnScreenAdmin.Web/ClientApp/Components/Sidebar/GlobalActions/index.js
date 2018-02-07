import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import ToggleButtons from './ToggleButtons';
import GlobalContextMenu from './GlobalContextMenu';


const GlobalActions = (props) => {
  const {
    toggleAllWidgets,
    toggleAllZones,
    showAllWidgets,
    showAllZones,
    enabledMenuKeys,
    addWidgetToPage,
    toggleShowOnlyWidgets,
    showOnlyWidgets,
  } = props;

  return (
    <Fragment>
      <ToggleButtons
        showAllWidgets={showAllWidgets}
        showAllZones={showAllZones}
        toggleAllWidgets={toggleAllWidgets}
        toggleAllZones={toggleAllZones}
      />
      <GlobalContextMenu
        enabledMenuKeys={enabledMenuKeys}
        addWidgetToPage={addWidgetToPage}
        toggleShowOnlyWidgets={toggleShowOnlyWidgets}
        showOnlyWidgets={showOnlyWidgets}
      />
    </Fragment>
  );
};

GlobalActions.propTypes = {
  showAllZones: PropTypes.bool.isRequired,
  showAllWidgets: PropTypes.bool.isRequired,
  toggleAllZones: PropTypes.func.isRequired,
  toggleAllWidgets: PropTypes.func.isRequired,
  enabledMenuKeys: PropTypes.array.isRequired,
  addWidgetToPage: PropTypes.func.isRequired,
  toggleShowOnlyWidgets: PropTypes.func.isRequired,
  showOnlyWidgets: PropTypes.bool.isRequired,
};

export default GlobalActions;

