import React, { Fragment } from 'react';
import PropTypes from 'prop-types';


import AvailableWidgetsList from 'containers/WidgetsScreen/AvailableWidgetsScreen/availableWidgetsList';
import AvailableWidgetsSearch from 'containers/WidgetsScreen/AvailableWidgetsScreen/availableWidgetsSearch';
import WizardSubheader from '../../WizardSubheader';


const AvailableWidgetsScreen = ({ onSelectWidget, onToggleSearchBox, showSearchBox }) => (
  <Fragment>
    <WizardSubheader
      text="Select widget to add"
      showSearchButton
      searchButtonClick={onToggleSearchBox}
    />
    {showSearchBox && (<AvailableWidgetsSearch />) }
    <AvailableWidgetsList
      onSelectWidget={onSelectWidget}
    />
  </Fragment>
);

AvailableWidgetsScreen.propTypes = {
  onSelectWidget: PropTypes.func.isRequired,
  onToggleSearchBox: PropTypes.func.isRequired,
  showSearchBox: PropTypes.bool.isRequired,
};

export default AvailableWidgetsScreen;
