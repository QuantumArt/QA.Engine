import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import Typography from 'material-ui/Typography';
import { withStyles } from 'material-ui/styles';
import IconButton from 'material-ui/IconButton';
import ArrowBack from 'material-ui-icons/ArrowBack';

const styles = theme => ({
  text: {
    fontSize: theme.typography.pxToRem(20),
  },

});

const WizardHeader = ({ text, onClickBack, classes }) => (
  <Fragment>
    <Typography type="display1" align="left">
      <IconButton className={classes.button} aria-label="Back" onClick={onClickBack}>
        <ArrowBack />
      </IconButton>
      { text }
    </Typography>
  </Fragment>
);


WizardHeader.propTypes = {
  text: PropTypes.string.isRequired,
  onClickBack: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(WizardHeader);
