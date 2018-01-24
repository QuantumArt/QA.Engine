import React from 'react';
import PropTypes from 'prop-types';
import Toolbar from 'material-ui/Toolbar';
import { withStyles } from 'material-ui/styles';
import Tabs, { Tab } from 'material-ui/Tabs';
import DeveloperBoard from 'material-ui-icons/DeveloperBoard';
import Tune from 'material-ui-icons/Tune';

const styles = () => ({
  tabsToolbar: {
    padding: '10px 0 0',
  },
  tabRoot: {
    minWidth: 120,
  },
});

const TabsToolbar = ({ classes, toggleTab, showTabs, widgetTabAvailable, abTestsTabAvailable, activeTab }) => (

  !showTabs
    ? null
    : (
      <Toolbar classes={{ root: classes.tabsToolbar }}>
        <Tabs
          value={activeTab}
          onChange={(e, value) => { toggleTab(value); }}
          indicatorColor="primary"
          textColor="primary"
        >
          {!widgetTabAvailable
            ? null
            : (
              <Tab
                icon={<DeveloperBoard />}
                label="WIDGETS"
                classes={{ root: classes.tabRoot }}
              />
            )}
          {!abTestsTabAvailable
            ? null
            : (
              <Tab
                icon={<Tune />}
                label="A/B TESTS"
                classes={{ root: classes.tabRoot }}
              />
            )
          }
        </Tabs>
      </Toolbar>
    ));

TabsToolbar.propTypes = {
  toggleTab: PropTypes.func.isRequired,
  showTabs: PropTypes.bool.isRequired,
  widgetTabAvailable: PropTypes.bool.isRequired,
  abTestsTabAvailable: PropTypes.bool.isRequired,
  activeTab: PropTypes.number.isRequired,
  classes: PropTypes.object.isRequired,
};


export default withStyles(styles)(TabsToolbar);
