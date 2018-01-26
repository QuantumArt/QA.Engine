import React from 'react';
import PropTypes from 'prop-types';
import Paper from 'material-ui/Paper';
import Typography from 'material-ui/Typography';
import { withStyles } from 'material-ui/styles';
// import IconButton from 'material-ui/IconButton';
// import ArrowBack from 'material-ui-icons/ArrowBack';

const styles = () => ({
  wrap: {
    padding: '0 16px',
  },
  text: {
    // fontStyle: 'italic',
    marginTop: 16,
  },
});

const WizardSubHeader = ({ text, classes }) => (
  <Paper className={classes.wrap} elevation={0}>
    <Typography type="title" align="center" className={classes.text}>
      { text }
    </Typography>
  </Paper>
);


WizardSubHeader.propTypes = {
  text: PropTypes.string.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(WizardSubHeader);
