import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import Typography from 'material-ui/Typography';
import { withStyles } from 'material-ui/styles';
// import IconButton from 'material-ui/IconButton';
// import ArrowBack from 'material-ui-icons/ArrowBack';

const styles = () => ({
  text: {
    fontStyle: 'italic',
  },

});

const WizardSubHeader = ({ text, classes }) => (
  <Fragment>
    <Typography type="title" className={classes.text}>

      { text }
    </Typography>
  </Fragment>
);


WizardSubHeader.propTypes = {
  text: PropTypes.string.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(WizardSubHeader);
