/* eslint-disable */
import React from 'react';
import { withStyles } from 'material-ui/styles';
import PropTypes from 'prop-types';
import ToolBar from 'material-ui/Toolbar';
import IconButton from 'material-ui/IconButton';
import SearchIcon from 'material-ui-icons/Search';
import Typography from 'material-ui/Typography';


const styles = () => ({
  toolbar: {
    justifyContent: 'center',
  },
});

const HeaderWithSearchButton = (props) => {
  const { 
    text, 
    classes,
    showSearchButton,
    searchButtonClick,
   } = props;
  return (
    <ToolBar disableGutters className={classes.toolbar}>
      {showSearchButton && 
        <IconButton
          onClick={searchButtonClick}>
          <SearchIcon />
        </IconButton>
      }
      <Typography variant="title">
        {text}
      </Typography>
    </ToolBar>
  );
};


HeaderWithSearchButton.propTypes = {
  showSearchButton: PropTypes.bool,
  searchButtonClick: PropTypes.func,
  text: PropTypes.string.isRequired,
  classes: PropTypes.object.isRequired,
};

HeaderWithSearchButton.defaultProps = {
  searchButtonClick: null,
  showSearchButton: false,
};

export default withStyles(styles)(HeaderWithSearchButton);

