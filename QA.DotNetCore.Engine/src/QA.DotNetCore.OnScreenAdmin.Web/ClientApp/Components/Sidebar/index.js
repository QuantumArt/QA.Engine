import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Drawer from 'material-ui/Drawer';
// import Divider from 'material-ui/Divider';
import Toolbar from 'material-ui/Toolbar';
import IconButton from 'material-ui/IconButton';
import ExitToApp from 'material-ui-icons/ExitToApp';
import TextField from 'material-ui/TextField';
import BorderLeft from 'material-ui-icons/BorderLeft';
import BorderRight from 'material-ui-icons/BorderRight';
import FlipToBack from 'material-ui-icons/FlipToBack';
import FlipToFront from 'material-ui-icons/FlipToFront';
import 'typeface-roboto/index.css';
import ComponentTree from '../../containers/componentTree';
import OpenControl from '../OpenControl';

const styles = theme => ({
  sidebar: {

  },
  drawer: {
    width: 360,
  },
  controlToolbar: {
    minHeight: 40,
    marginTop: 10,
    justifyContent: 'space-around',
  },
  controlButtonRoot: {
    width: 40,
    height: 40,
  },
  controlButtonIcon: {
    width: 20,
    height: 20,
  },
  treeToolbar: {

  },
  closeButton: {
    width: theme.spacing.unit * 3,
    height: theme.spacing.unit * 3,
  },
  searchField: {
    marginLeft: theme.spacing.unit * 2,
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

const Sidebar = (props) => {
  const {
    opened,
    showAllZones,
    side,
    toggleSidebar,
    toggleLeft,
    toggleRight,
    toggleAllZones,
    classes,
  } = props;

  return (
    <div className={classes.sidebar}>
      <OpenControl
        toggleSidebar={toggleSidebar}
        toggleLeft={toggleLeft}
        toggleRight={toggleRight}
        drawerOpened={opened}
      />
      <Drawer
        type="persistent"
        open={opened}
        classes={{ paper: classes.drawer }}
        anchor={side}
      >
        <Toolbar disableGutters classes={{ root: classes.controlToolbar }}>
          <IconButton
            color="primary"
            classes={{ icon: classes.controlButtonIcon, root: classes.controlButtonRoot }}
            onClick={toggleLeft}
          >
            <BorderLeft />
          </IconButton>
          <IconButton
            color="primary"
            classes={{ icon: classes.controlButtonIcon, root: classes.controlButtonRoot }}
            onClick={toggleAllZones}
          >
            {showAllZones ? <FlipToFront /> : <FlipToBack />}
          </IconButton>
          <IconButton
            color="primary"
            classes={{ icon: classes.controlButtonIcon, root: classes.controlButtonRoot }}
            onClick={toggleRight}
          >
            <BorderRight />
          </IconButton>
        </Toolbar>
        <Toolbar disableGutters classes={{ root: classes.treeToolbar }}>
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
          />
          <IconButton
            color="primary"
            onClick={toggleSidebar}
            classes={{ icon: classes.closeButton }}
            style={{ transform: side === 'left' ? 'rotate(180deg)' : '' }}
          >
            <ExitToApp />
          </IconButton>
        </Toolbar>
        <ComponentTree />
      </Drawer>
    </div>
  );
};

Sidebar.propTypes = {
  opened: PropTypes.bool.isRequired,
  showAllZones: PropTypes.bool.isRequired,
  side: PropTypes.string.isRequired,
  toggleSidebar: PropTypes.func.isRequired,
  toggleLeft: PropTypes.func.isRequired,
  toggleRight: PropTypes.func.isRequired,
  toggleAllZones: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Sidebar);
