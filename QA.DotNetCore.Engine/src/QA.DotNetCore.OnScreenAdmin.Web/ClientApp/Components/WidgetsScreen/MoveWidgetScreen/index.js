import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import Typography from 'material-ui/Typography';
import Button from 'material-ui/Button';
import ComponentTree from 'containers/WidgetsScreen/ComponentTreeScreen/componentTree';
import ComponentTreeSearch from 'containers/WidgetsScreen/ComponentTreeScreen/componentTreeSearch';

const MoveWidgetScreen = ({ onCancel }) => (
  <Fragment>
    <ComponentTreeSearch />
    <Typography type="headline" align="center">
      Select target zone
    </Typography>
    <ComponentTree />
    <Button raised onClick={onCancel}>Cancel</Button>
  </Fragment>
);

MoveWidgetScreen.propTypes = {
  onCancel: PropTypes.func.isRequired,
};

export default MoveWidgetScreen;
