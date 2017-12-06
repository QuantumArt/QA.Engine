import React from 'react';
import PropTypes from 'prop-types';
import SwipeableViews from 'react-swipeable-views';
import { Scrollbars } from 'react-custom-scrollbars';
import { withStyles } from 'material-ui/styles';
import Drawer from 'material-ui/Drawer';
import Tabs, { Tab } from 'material-ui/Tabs';
import Divider from 'material-ui/Divider';
import Toolbar from 'material-ui/Toolbar';
import IconButton from 'material-ui/IconButton';
import ExitToApp from 'material-ui-icons/ExitToApp';
import BorderLeft from 'material-ui-icons/BorderLeft';
import BorderRight from 'material-ui-icons/BorderRight';
import DeveloperBoard from 'material-ui-icons/DeveloperBoard';
import Kitchen from 'material-ui-icons/Kitchen';
import Tune from 'material-ui-icons/Tune';
import 'typeface-roboto/index.css';
import OpenControl from '../OpenControl';
import WidgetsScreen from '../WidgetsScreen';

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
  closeButton: {
    width: theme.spacing.unit * 2.8,
    height: theme.spacing.unit * 2.8,
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
    activeTab,
    toggleSidebar,
    toggleLeft,
    toggleRight,
    toggleAllZones,
    toggleTab,
    widgetScreenSearchText,
    widgetScreenChangeSearchText,
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
            <IconButton
              color="primary"
              onClick={toggleSidebar}
              classes={{ icon: classes.closeButton }}
              style={{ transform: side === 'left' ? 'rotate(180deg)' : '' }}
            >
              <ExitToApp />
            </IconButton>
          </Toolbar>
          <Divider />
          <Toolbar classes={{ root: classes.tabsToolbar }}>
            <Tabs
              value={activeTab}
              onChange={(e, value) => { toggleTab(value); }}
              indicatorColor="primary"
              textColor="primary"
            >
              <Tab
                icon={<DeveloperBoard />}
                label="WIDGETS"
                classes={{ root: classes.tabRoot }}
              />
              <Tab
                icon={<Tune />}
                label="A/B TESTS"
                classes={{ root: classes.tabRoot }}
              />
              <Tab
                icon={<Kitchen />}
                label="ELSE"
                classes={{ root: classes.tabRoot }}
                disabled
              />
            </Tabs>
          </Toolbar>
          <SwipeableViews axis="x" index={activeTab}>
            <WidgetsScreen
              showAllZones={showAllZones}
              toggleAllZones={toggleAllZones}
              searchText={widgetScreenSearchText}
              changeSearchText={widgetScreenChangeSearchText}
            />
            <div>123</div>
          </SwipeableViews>
        </Scrollbars>
      </Drawer>
    </div>
  );
};

Sidebar.propTypes = {
  opened: PropTypes.bool.isRequired,
  showAllZones: PropTypes.bool.isRequired,
  side: PropTypes.string.isRequired,
  activeTab: PropTypes.number.isRequired,
  toggleSidebar: PropTypes.func.isRequired,
  toggleLeft: PropTypes.func.isRequired,
  toggleRight: PropTypes.func.isRequired,
  toggleAllZones: PropTypes.func.isRequired,
  toggleTab: PropTypes.func.isRequired,
  widgetScreenSearchText: PropTypes.string.isRequired,
  widgetScreenChangeSearchText: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Sidebar);
