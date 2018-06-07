import React from 'react';
import PropTypes from 'prop-types';
import Paper from 'material-ui/Paper';
// import ToolBar from 'material-ui/Toolbar';
import IconButton from 'material-ui/IconButton';
import SearchIcon from 'material-ui-icons/Search';
import Typography from 'material-ui/Typography';
import { withStyles } from 'material-ui/styles';


const styles = () => ({
  wrap: {
    padding: '0 16px',
  },
  text: {
    // fontStyle: 'italic',
    // marginTop: 16,
  },
  toolbar: {
    justifyContent: 'center',
    padding: '0 40px',
    // marginBottom: '0px',
  },
});

const WizardSubHeader = ({ text, classes, showSearchButton, searchButtonClick, className, textClass }) => (
  <Paper className={classes.toolbar} elevation={0}>
    <Typography variant="title" className={textClass}>
      {showSearchButton &&
        <IconButton
          onClick={searchButtonClick}
        >
          <SearchIcon />
        </IconButton>}
      { text }
    </Typography>
  </Paper>
);


WizardSubHeader.propTypes = {
  text: PropTypes.string.isRequired,
  classes: PropTypes.object.isRequired,
  showSearchButton: PropTypes.bool,
  searchButtonClick: PropTypes.func,
  className: PropTypes.any,
  textClass: PropTypes.any,
};

WizardSubHeader.defaultProps = {
  searchButtonClick: null,
  showSearchButton: false,
  className: null,
  textClass: null,
};

export default withStyles(styles)(WizardSubHeader);
