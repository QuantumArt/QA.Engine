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
  tabsRoot: {
    width: '100%',
  },
});

const TabsToolbar = (props) => {
  const {
    classes,
    toggleTab,
    widgetTabAvailable,
    abTestsTabAvailable,
    featuresCount,
    activeTab,
  } = props;
  const tabWidth = `${100 / featuresCount}%`;

  return (
    <Toolbar classes={{ root: classes.tabsToolbar }}>
      <Tabs
        value={activeTab}
        onChange={(e, value) => { toggleTab(value); }}
        indicatorColor="primary"
        textColor="primary"
        classes={{ root: classes.tabsRoot }}
      >
        {widgetTabAvailable &&
          <Tab
            icon={<DeveloperBoard />}
            label="WIDGETS"
            style={{ width: tabWidth }}
          />
        }
        {abTestsTabAvailable &&
          <Tab
            icon={<Tune />}
            label="A/B TESTS"
            style={{ width: tabWidth }}
          />
        }
      </Tabs>
    </Toolbar>
  );
};

TabsToolbar.propTypes = {
  toggleTab: PropTypes.func.isRequired,
  widgetTabAvailable: PropTypes.bool.isRequired,
  abTestsTabAvailable: PropTypes.bool.isRequired,
  activeTab: PropTypes.number.isRequired,
  featuresCount: PropTypes.number.isRequired,
  classes: PropTypes.object.isRequired,
};


export default withStyles(styles)(TabsToolbar);
