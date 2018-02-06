import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Button from 'material-ui/Button';
import { lightBlue, green, grey } from 'material-ui/colors';

const styles = theme => ({
  buttonRoot: {
    width: theme.spacing.unit * 5,
    height: theme.spacing.unit,
    minWidth: theme.spacing.unit * 5,
    fontSize: theme.typography.pxToRem(20),
  },
  zonesButtonChecked: {
    color: green[400],
  },
  widgetsButtonChecked: {
    color: lightBlue[400],
  },
  buttonUnchecked: {
    color: grey[500],
  },
});


const ToggleButtons = (props) => {
  const {
    classes,
    toggleAllWidgets,
    toggleAllZones,
    showAllWidgets,
    showAllZones,
  } = props;

  return (
    <Fragment>
      <Button
        classes={{ root: classes.buttonRoot }}
        onClick={toggleAllWidgets}
        className={showAllWidgets ? classes.widgetsButtonChecked : classes.buttonUnchecked}
      >
        W
      </Button>
      <Button
        classes={{ root: classes.buttonRoot }}
        onClick={toggleAllZones}
        className={showAllZones ? classes.zonesButtonChecked : classes.buttonUnchecked}
      >
        Z
      </Button>
    </Fragment>
  );
};

ToggleButtons.propTypes = {
  showAllZones: PropTypes.bool.isRequired,
  showAllWidgets: PropTypes.bool.isRequired,
  toggleAllZones: PropTypes.func.isRequired,
  toggleAllWidgets: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(ToggleButtons);

