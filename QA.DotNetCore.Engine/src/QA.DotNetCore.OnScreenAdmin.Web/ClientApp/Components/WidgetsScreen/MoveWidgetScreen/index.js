import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import Button from 'material-ui/Button';
import ComponentTree from 'containers/WidgetsScreen/ComponentTreeScreen/componentTree';
import ComponentTreeSearch from 'containers/WidgetsScreen/ComponentTreeScreen/componentTreeSearch';
import WizardHeader from '../WizardHeader';
import WizardSubheader from '../WizardSubheader';

const MoveWidgetScreen = ({ onCancel }) => (
  <Fragment>
    <WizardHeader text="MoveWidget" onClickBack={onCancel} />
    <WizardSubheader text="Select target zone for widget" />
    <ComponentTreeSearch />
    <ComponentTree />
    <Button
      variant="raised"
      onClick={onCancel}
    >Cancel</Button>
  </Fragment>
);

MoveWidgetScreen.propTypes = {
  onCancel: PropTypes.func.isRequired,
};

export default MoveWidgetScreen;
