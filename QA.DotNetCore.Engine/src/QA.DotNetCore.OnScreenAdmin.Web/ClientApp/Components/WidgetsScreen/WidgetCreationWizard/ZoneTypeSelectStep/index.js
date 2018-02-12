
import React, { Fragment } from 'react';
import { withStyles } from 'material-ui/styles';
import PropTypes from 'prop-types';


import Button from 'material-ui/Button';
import WizardSubheader from '../../WizardSubheader';


const styles = theme => ({
  button: {
    fontSize: theme.typography.pxToRem(25),
  },

});

const ZoneTypeSelectStep = ({ onSelectCustomZone, onSelectExistingZone, classes }) => (
  <Fragment>
    <WizardSubheader text="Select target zone type" />
    <Button
      fullWidth
      size="large"
      className={classes.button}
      onClick={onSelectExistingZone}
    >
      Add to existing zone
    </Button>
    <Button
      fullWidth
      size="large"
      className={classes.button}
      onClick={onSelectCustomZone}
    >
      Add to custom zone
    </Button>
  </Fragment>
);

ZoneTypeSelectStep.propTypes = {
  onSelectCustomZone: PropTypes.func.isRequired,
  onSelectExistingZone: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(ZoneTypeSelectStep);
