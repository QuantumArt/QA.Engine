import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import ComponentTree from 'containers/WidgetsScreen/ComponentTreeScreen/componentTree';
import ComponentTreeSearch from 'containers/WidgetsScreen/ComponentTreeScreen/componentTreeSearch';
import WizardHeader from '../WizardHeader';
import WizardSubheader from '../WizardSubheader';

const MoveWidgetScreen = ({ onCancel, showSearchBox, toggleSearchBoxVisibility }) => (
  <Fragment>
    <WizardHeader text="MoveWidget" onClickBack={onCancel} />
    <WizardSubheader
      text="Select target zone for widget"
      showSearchButton
      searchButtonClick={toggleSearchBoxVisibility}
    />
    {showSearchBox &&
      <ComponentTreeSearch />
    }
    <ComponentTree />
  </Fragment>
);

MoveWidgetScreen.propTypes = {
  onCancel: PropTypes.func.isRequired,
  showSearchBox: PropTypes.bool.isRequired,
  toggleSearchBoxVisibility: PropTypes.func.isRequired,
};

export default MoveWidgetScreen;
