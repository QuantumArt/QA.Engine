import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Toolbar from 'material-ui/Toolbar';
import TextField from 'material-ui/TextField';
import { FormControlLabel } from 'material-ui/Form';
import Switch from 'material-ui/Switch';
import ComponentTree from '../../containers/componentTree';

const styles = theme => ({
  searchField: {
    marginLeft: theme.spacing.unit * 2,
    marginRight: theme.spacing.unit * 2,
    fontSize: theme.spacing.unit * 2,
  },
  searchInput: {
    fontSize: theme.spacing.unit * 2,
  },
  searchFieldLabel: {
    fontWeight: 'normal',
    fontSize: theme.spacing.unit * 2,
  },
  switchLabel: {
    fontSize: theme.spacing.unit * 1.8,
  },
});

const WidgetsScreen = ({ classes, showAllZones, toggleAllZones }) => (
  <Fragment>
    <Toolbar disableGutters>
      <TextField
        id="search"
        label="Search items"
        type="text"
        margin="normal"
        fullWidth
        className={classes.searchField}
        InputLabelProps={{
          className: classes.searchFieldLabel,
        }}
        InputProps={{
          className: classes.searchInput,
        }}
      />
    </Toolbar>
    <Toolbar>
      <FormControlLabel
        control={
          <Switch
            checked={showAllZones}
            onChange={toggleAllZones}
            aria-label="AllZonesChecked"
          />
        }
        label="Toggle all zones mode"
        classes={{ label: classes.switchLabel }}
      />
    </Toolbar>
    <ComponentTree />
  </Fragment>
);

WidgetsScreen.propTypes = {
  showAllZones: PropTypes.bool.isRequired,
  toggleAllZones: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(WidgetsScreen);
