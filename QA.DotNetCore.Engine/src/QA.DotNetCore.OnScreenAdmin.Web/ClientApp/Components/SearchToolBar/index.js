import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Toolbar from 'material-ui/Toolbar';
import TextField from 'material-ui/TextField';

const styles = theme => ({
  searchField: {
    marginLeft: theme.spacing.unit * 2,
    marginRight: theme.spacing.unit * 2,
    marginTop: 0,
    fontSize: theme.spacing.unit * 2,
  },
  searchInput: {
    fontSize: theme.spacing.unit * 2,
  },
  searchFieldLabel: {
    fontWeight: 'normal',
    fontSize: theme.spacing.unit * 2,
  },
});

const SearchToolBar = ({ classes, searchText, changeSearchText }) => (
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
      value={searchText}
      onChange={changeSearchText}
    />
  </Toolbar>
);

SearchToolBar.propTypes = {
  classes: PropTypes.object.isRequired,
  searchText: PropTypes.string.isRequired,
  changeSearchText: PropTypes.func.isRequired,
};

export default withStyles(styles)(SearchToolBar);
