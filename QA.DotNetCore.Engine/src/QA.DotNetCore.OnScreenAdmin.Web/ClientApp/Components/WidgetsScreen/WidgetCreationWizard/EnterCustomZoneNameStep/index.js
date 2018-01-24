/* eslint-disable */
import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Toolbar from 'material-ui/Toolbar';
import TextField from 'material-ui/TextField';
import Button from 'material-ui/Button';
import WizardSubheader from '../../WizardSubheader';


const styles = theme => ({
  zoneNameField: {
    marginLeft: theme.spacing.unit * 2,
    marginRight: theme.spacing.unit * 2,
    marginTop: 0,
    fontSize: theme.spacing.unit * 2,
  },
  zoneNameInput: {
    fontSize: theme.spacing.unit * 2,
  },
  zoneNameFieldLabel: {
    fontWeight: 'normal',
    fontSize: theme.spacing.unit * 2,
  },
});


const EnterCustomZoneNameStep = ({
  classes,
  customZoneName,
  onChangeCustomZoneName,
  onConfirmCustomZoneName,
}) => (
  <Fragment>
    <WizardSubheader text="Enter custom zone name" />
    <Toolbar disableGutters>
      <TextField
        id="customZoneName"
        label="Zone name"
        type="text"
        margin="normal"
        fullWidth
        className={classes.zoneNameField}
        InputLabelProps={{
          className: classes.zoneNameFieldLabel,
        }}
        InputProps={{
          className: classes.zoneNameInput,
        }}
        value={customZoneName}
        onChange={onChangeCustomZoneName}
      />
      <Button raised onClick={() => {onConfirmCustomZoneName(customZoneName)}}>Continue</Button>
    </Toolbar>
    
  </Fragment>
);

EnterCustomZoneNameStep.propTypes = {
  customZoneName: PropTypes.string.isRequired,
  onChangeCustomZoneName: PropTypes.func.isRequired,
  onConfirmCustomZoneName: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};


export default withStyles(styles)(EnterCustomZoneNameStep);
