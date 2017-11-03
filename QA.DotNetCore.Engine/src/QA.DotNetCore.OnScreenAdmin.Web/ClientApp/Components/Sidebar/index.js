import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Drawer from 'material-ui/Drawer';
import Divider from 'material-ui/Divider';
import Toolbar from 'material-ui/Toolbar';
import IconButton from 'material-ui/IconButton';
import ExitToApp from 'material-ui-icons/ExitToApp';
import TextField from 'material-ui/TextField';
import 'typeface-roboto/index.css';
import ComponentTree from '../../containers/componentTree';
import OpenControl from '../OpenControl';

const styles = theme => ({
  sidebar: {

  },
  drawer: {

  },
  toolbar: {

  },
  closeButton: {
    transform: 'rotate(180deg)',
  },
  searchField: {
    marginLeft: theme.spacing.unit * 2,
  },
  searchFieldLabel: {
    fontWeight: 'normal',
  },
});

class Sidebar extends Component {
  eventLogger = (e: MouseEvent, data: Object) => {
    console.log('Event: ', e);
    console.log('Data: ', data);
  };

  render() {
    const { opened, toggleSidebar, classes } = this.props;

    return (
      <div className={classes.sidebar}>
        <OpenControl onClick={toggleSidebar} drawerOpened={opened} />
        <Drawer type="persistent" open={opened} classes={{ paper: classes.drawer }}>
          <Toolbar disableGutters classes={{ root: classes.toolbar }}>
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
            />
            <IconButton
              color="primary"
              onClick={toggleSidebar}
              className={classes.closeButton}
            >
              <ExitToApp />
            </IconButton>
          </Toolbar>
          <Divider light />
          <ComponentTree />
        </Drawer>
      </div>
    );
  }
}

Sidebar.propTypes = {
  opened: PropTypes.bool.isRequired,
  toggleSidebar: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Sidebar);
