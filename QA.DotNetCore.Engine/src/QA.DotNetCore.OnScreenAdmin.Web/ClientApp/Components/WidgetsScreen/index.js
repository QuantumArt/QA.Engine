import React, { Fragment, Component } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Toolbar from 'material-ui/Toolbar';
import TextField from 'material-ui/TextField';
import { FormControlLabel } from 'material-ui/Form';
import Switch from 'material-ui/Switch';
import { lightBlue, green } from 'material-ui/colors';
import ComponentTree from '../../containers/componentTree';

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
      showAllZones,
      showAllWidgets,
      toggleAllZones,
      toggleAllWidgets,
      side,
      searchText,
    } = this.props;
    return (
      <Fragment>
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
        <ComponentTree
          showAllZones={showAllZones}
          showAllWidgets={showAllWidgets}
          side={side}
        />
      </Fragment>
    );
  }
}

WidgetsScreen.propTypes = {
  showAllZones: PropTypes.bool.isRequired,
  showAllWidgets: PropTypes.bool.isRequired,
  toggleAllZones: PropTypes.func.isRequired,
  toggleAllWidgets: PropTypes.func.isRequired,
  side: PropTypes.string.isRequired,
  changeSearchText: PropTypes.func.isRequired,
  searchText: PropTypes.string.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(WidgetsScreen);
