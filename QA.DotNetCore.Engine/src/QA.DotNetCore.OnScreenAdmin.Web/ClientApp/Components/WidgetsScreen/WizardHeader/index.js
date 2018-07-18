import React from 'react';
import PropTypes from 'prop-types';
import AppBar from 'material-ui/AppBar';
import Toolbar from 'material-ui/Toolbar';
import Typography from 'material-ui/Typography';
import { withStyles } from 'material-ui/styles';
import IconButton from 'material-ui/IconButton';
import ArrowBack from 'material-ui-icons/ArrowBack';

const styles = theme => ({
  appbar: {
    // marginTop: 10,
  },
  toolbar: {
    // justifyContent: 'center',
  },
  text: {
    fontSize: theme.typography.pxToRem(20),
  },
  button: {
    marginRight: 24,
  },
  actionIcon: {
    width: 25,
    height: 25,
  },
});

const WizardHeader = ({ text, onClickBack, classes }) => (
  <AppBar position="static" className={classes.appbar}>
    <Toolbar disableGutters className={classes.toolbar}>
      <IconButton
        className={classes.button}
        aria-label="Back"
        onClick={onClickBack}
        color="inherit"
      >
        <ArrowBack className={classes.actionIcon} />
      </IconButton>
      <Typography variant="display1" align="left" color="inherit">
        { text }
      </Typography>
    </Toolbar>
  </AppBar>
);


WizardHeader.propTypes = {
  text: PropTypes.string.isRequired,
  onClickBack: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(WizardHeader);
