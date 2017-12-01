import React from 'react';
import PropTypes from 'prop-types';
import { Scrollbars } from 'react-custom-scrollbars';
import { withStyles } from 'material-ui/styles';
import Drawer from 'material-ui/Drawer';
import Tabs, { Tab } from 'material-ui/Tabs';
import Divider from 'material-ui/Divider';
import Switch from 'material-ui/Switch';
import { FormControlLabel } from 'material-ui/Form';
import Toolbar from 'material-ui/Toolbar';
import IconButton from 'material-ui/IconButton';
import ExitToApp from 'material-ui-icons/ExitToApp';
import TextField from 'material-ui/TextField';
import BorderLeft from 'material-ui-icons/BorderLeft';
import BorderRight from 'material-ui-icons/BorderRight';
import DeveloperBoard from 'material-ui-icons/DeveloperBoard';
import Kitchen from 'material-ui-icons/Kitchen';
import Tune from 'material-ui-icons/Tune';
import 'typeface-roboto/index.css';
import ComponentTree from '../../containers/componentTree';
import OpenControl from '../OpenControl';

const styles = theme => ({
  sidebar: {

  },
  drawer: {
    width: 361,
  },
  controlToolbar: {
    minHeight: 40,
    marginTop: 10,
    marginBottom: 10,
    marginRight: 6,
    justifyContent: 'flex-end',
  },
  controlButtonRoot: {
    width: theme.spacing.unit * 5,
    height: theme.spacing.unit * 5,
  },
  controlButtonIcon: {
    width: theme.spacing.unit * 2.8,
    height: theme.spacing.unit * 2.8,
  },
  tabsToolbar: {
    padding: '10px 0 0',
  },
  treeToolbar: {

  },
  closeButton: {
    width: theme.spacing.unit * 2.8,
    height: theme.spacing.unit * 2.8,
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
  switchLabel: {
    fontSize: theme.spacing.unit * 1.8,
  },
  tabRoot: {
    minWidth: 120,
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
        <Scrollbars autoHide>
          <Toolbar disableGutters classes={{ root: classes.controlToolbar }}>
            <IconButton
              color="primary"
              onClick={toggleSidebar}
              classes={{ icon: classes.closeButton }}
              style={{ transform: side === 'left' ? 'rotate(180deg)' : '' }}
            >
              <ExitToApp />
            </IconButton>
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
              onClick={toggleRight}
            >
              <BorderRight />
            </IconButton>
          </Toolbar>
          <Divider />
          <Toolbar classes={{ root: classes.tabsToolbar }}>
            <Tabs
              value={1}
              // onChange={this.handleChange}
              indicatorColor="primary"
              textColor="primary"
            >
              <Tab icon={<Tune />} label="WIDGETS" classes={{ root: classes.tabRoot }} />
              <Tab icon={<DeveloperBoard />} label="A/B TESTS" classes={{ root: classes.tabRoot }} />
              <Tab icon={<Kitchen />} label="ELSE" classes={{ root: classes.tabRoot }} />
            </Tabs>
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
          </Toolbar>
          <Toolbar>
            <FormControlLabel
              control={
                <Switch
                  checked={showAllZones}
                  onChange={toggleAllZones}
                  aria-label="AllZonesChecked"
                />
              }
              label="Toggle all zones mode"
              classes={{ label: classes.switchLabel }}
            />
          </Toolbar>
          <ComponentTree />
        </Scrollbars>
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
