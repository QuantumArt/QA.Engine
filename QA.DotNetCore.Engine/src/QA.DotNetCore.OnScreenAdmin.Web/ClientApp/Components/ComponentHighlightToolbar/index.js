import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Toolbar from 'material-ui/Toolbar';
import { FormControlLabel } from 'material-ui/Form';
import Switch from 'material-ui/Switch';
import { lightBlue, green } from 'material-ui/colors';

const styles = theme => ({
  switchToolbar: {
    justifyContent: 'space-between',
    paddingTop: 16,
  },
  switchLabel: {
    fontSize: theme.spacing.unit * 1.8,
  },
  zoneSwitchBar: {},
  zoneSwitchChecked: {
    'color': green[400],
    '& + $zoneSwitchBar': {
      backgroundColor: green[400],
    },
  },
  widgetsSwitchBar: {},
  widgetSwitchChecked: {
    'color': lightBlue[400],
    '& + $widgetsSwitchBar': {
      backgroundColor: lightBlue[400],
    },
  },
});

const ComponentHighlightToolbar = ({ classes, showAllWidgets, showAllZones, toggleAllWidgets, toggleAllZones }) => (
  <Fragment>
    <Toolbar className={classes.switchToolbar}>
      <FormControlLabel
        control={
          <Switch
            checked={showAllZones}
            onChange={toggleAllZones}
            aria-label="AllZonesChecked"
            classes={{
              bar: classes.zoneSwitchBar,
              checked: classes.zoneSwitchChecked,
            }}
          />
        }
        label="Show zones"
        classes={{ label: classes.switchLabel }}
      />
      <FormControlLabel
        control={
          <Switch
            checked={showAllWidgets}
            onChange={toggleAllWidgets}
            aria-label="AllWidgetsChecked"
            classes={{
              bar: classes.widgetSwitchBar,
              checked: classes.widgetSwitchChecked,
            }}
          />
        }
        label="Show widgets"
        classes={{ label: classes.switchLabel }}
      />
    </Toolbar>
  </Fragment>
);

ComponentHighlightToolbar.propTypes = {
  showAllZones: PropTypes.bool.isRequired,
  showAllWidgets: PropTypes.bool.isRequired,
  toggleAllZones: PropTypes.func.isRequired,
  toggleAllWidgets: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(ComponentHighlightToolbar);
