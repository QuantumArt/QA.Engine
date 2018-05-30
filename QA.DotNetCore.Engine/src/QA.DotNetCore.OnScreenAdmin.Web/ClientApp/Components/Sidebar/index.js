import React from 'react';
import PropTypes from 'prop-types';
import { Scrollbars } from 'react-custom-scrollbars';
import { withStyles } from 'material-ui/styles';
import Drawer from 'material-ui/Drawer';
import Divider from 'material-ui/Divider';
import Toolbar from 'material-ui/Toolbar';
import IconButton from 'material-ui/IconButton';
import ExitToApp from 'material-ui-icons/ExitToApp';
import BorderLeft from 'material-ui-icons/BorderLeft';
import BorderRight from 'material-ui-icons/BorderRight';
import { DRAWER_WIDTH } from 'constants/general';
import GlobalActions from 'containers/globalActions';
import 'typeface-roboto/index.css';
import OpenControl from '../OpenControl';
import TabsToolbar from './TabsToolbar';
import Screens from './Screens';


const styles = theme => ({
  sidebar: {

  },
  drawer: {
    width: DRAWER_WIDTH,
  },
  topToolbar: {
    justifyContent: 'space-between',
  },
  topLeftRoot: {
    minHeight: 40,
    marginTop: 10,
    marginBottom: 10,
    marginLeft: 6,
    justifyContent: 'flex-start',

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
    // width: theme.spacing.unit * 2.8,
    width: theme.spacing.unit * 2.8,
    height: theme.spacing.unit * 2.8,
  },
  closeButton: {
    width: theme.spacing.unit * 2.8,
    height: theme.spacing.unit * 2.8,
  },
});

const Sidebar = (props) => {
  const {
    opened,
    side,
    activeTab,
    toggleSidebar,
    toggleLeft,
    toggleRight,
    toggleTab,
    saveCords,
    classes,
    showTabs,
    widgetTabAvailable,
    abTestsTabAvailable,
    featuresCount,
    cords,
  } = props;


  return (
    <div className={classes.sidebar}>
      <OpenControl
        toggleSidebar={toggleSidebar}
        toggleLeft={toggleLeft}
        toggleRight={toggleRight}
        cords={cords}
        saveCords={saveCords}
        drawerOpened={opened}
      />
      <Drawer
        variant="persistent"
        open={opened}
        classes={{ paper: classes.drawer }}
        anchor={side}
      >
        <Scrollbars autoHide>
          <Toolbar disableGutters classes={{ root: classes.topToolbar }} >
            <div className={classes.topLeftRoot}>
              <GlobalActions />
            </div>
            <div className={classes.controlToolbar}>
              <IconButton
                color="primary"
                classes={{ root: classes.controlButtonRoot }}
                onClick={toggleLeft}
              >
                <BorderLeft classes={{ root: classes.controlButtonIcon }} />
              </IconButton>
              <IconButton
                color="primary"
                classes={{ root: classes.controlButtonRoot }}
                onClick={toggleRight}
              >
                <BorderRight className={classes.controlButtonIcon} />
              </IconButton>
              <IconButton
                color="primary"
                onClick={toggleSidebar}
                classes={{ root: classes.controlButtonRoot }}
                style={{ transform: side === 'left' ? 'rotate(180deg)' : '' }}
              >

                <ExitToApp className={classes.closeButton} />
              </IconButton>
            </div>
          </Toolbar>
          <Divider />
          {showTabs &&
            <TabsToolbar
              widgetTabAvailable={widgetTabAvailable}
              abTestsTabAvailable={abTestsTabAvailable}
              featuresCount={featuresCount}
              toggleTab={toggleTab}
              activeTab={activeTab}
            />
          }
          <Screens
            widgetTabAvailable={widgetTabAvailable}
            abTestsTabAvailable={abTestsTabAvailable}
            activeTab={activeTab}
          />
        </Scrollbars>
      </Drawer>
    </div>
  );
};

Sidebar.propTypes = {
  opened: PropTypes.bool.isRequired,
  side: PropTypes.string.isRequired,
  activeTab: PropTypes.number.isRequired,
  toggleSidebar: PropTypes.func.isRequired,
  toggleLeft: PropTypes.func.isRequired,
  toggleRight: PropTypes.func.isRequired,
  toggleTab: PropTypes.func.isRequired,
  cords: PropTypes.object.isRequired,
  saveCords: PropTypes.func.isRequired,
  showTabs: PropTypes.bool.isRequired,
  widgetTabAvailable: PropTypes.bool.isRequired,
  abTestsTabAvailable: PropTypes.bool.isRequired,
  featuresCount: PropTypes.number.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Sidebar);
