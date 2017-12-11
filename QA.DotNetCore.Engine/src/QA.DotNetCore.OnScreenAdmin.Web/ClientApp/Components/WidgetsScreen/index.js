import React, { Fragment, Component } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Toolbar from 'material-ui/Toolbar';
import TextField from 'material-ui/TextField';
// import { FormControlLabel } from 'material-ui/Form';
// import Switch from 'material-ui/Switch';
import { lightBlue, green } from 'material-ui/colors';
import ComponentTree from '../../containers/componentTree';
import EditComponentTree from '../../containers/editComponentTree';
import ComponentHighlightToolbar from '../../containers/componentHighlightToolbar';

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
  switchToolbar: {
    justifyContent: 'space-between',
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

class WidgetsScreen extends Component {
  // debouncedSearchChange = e => _.debounce(() => { console.log(e); }, 250);
  handleSearchChange = (event) => {
    const { changeSearchText } = this.props;
    console.log('handle change', event.target.value);
    changeSearchText(event.target.value);
  };

  render() {
    const {
      classes,
      side,
      searchText,
    } = this.props;
    return (
      <Fragment>
        <ComponentHighlightToolbar />
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
            onChange={this.handleSearchChange}
          />
        </Toolbar>
        <ComponentTree
          side={side}
        />
        <EditComponentTree />
      </Fragment>
    );
  }
}

WidgetsScreen.propTypes = {
  side: PropTypes.string.isRequired,
  changeSearchText: PropTypes.func.isRequired,
  searchText: PropTypes.string.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(WidgetsScreen);
