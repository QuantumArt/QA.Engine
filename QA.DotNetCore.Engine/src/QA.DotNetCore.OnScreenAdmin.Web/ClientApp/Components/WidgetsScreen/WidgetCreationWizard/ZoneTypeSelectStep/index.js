
import React, { Fragment } from 'react';
import { withStyles } from 'material-ui/styles';
import PropTypes from 'prop-types';


import Button from 'material-ui/Button';
import WizardSubheader from '../../WizardSubheader';


const styles = theme => ({
  button: {
    'fontSize': theme.typography.pxToRem(25),
    'width': 'calc((50%) - 2px)',
    'border': '1px solid rgba(0,0,0,.03)',
    'border-radius': '2px',
    'boxShadow': '0 2px 2px rgba(0,0,0,.1)',
    'margin-bottom': 4,
    '&:hover': {
      backgroundColor: 'rgba(0, 0, 0, 0.03)',
      boxShadow: '0 2px 2px rgba(0,0,0,.2)',
    },
    '&:active': {
      boxShadow: 'inset 0 2px 2px rgba(0,0,0,.1)',
    },
    '&+&': {
      marginLeft: 4,
    },
  },
  title: {
    margin: '25px 0',
    fontSize: '16px',
    textAlign: 'center',
  },
  titleClass: {
    fontSize: 16,
  },
});

const ZoneTypeSelectStep = ({ onSelectCustomZone, onSelectExistingZone, classes }) => (
  <Fragment>
    <WizardSubheader className={classes.title} textClass={classes.titleClass} text="Select target zone type" />
    <Button
      fullWidth
      size="large"
      className={classes.button}
      onClick={onSelectExistingZone}
    >
      Add to existing{'\u00A0'}zone
    </Button>
    <Button
      fullWidth
      size="large"
      className={classes.button}
      onClick={onSelectCustomZone}
    >
      Add to custom{'\u00A0'}zone
    </Button>
  </Fragment>
);

ZoneTypeSelectStep.propTypes = {
  onSelectCustomZone: PropTypes.func.isRequired,
  onSelectExistingZone: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(ZoneTypeSelectStep);
