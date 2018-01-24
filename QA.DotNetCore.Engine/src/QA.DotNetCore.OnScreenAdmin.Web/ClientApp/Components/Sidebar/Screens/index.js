import React from 'react';
import PropTypes from 'prop-types';
import SwipeableViews from 'react-swipeable-views';
import WidgetsScreen from 'containers/WidgetsScreen';
import AbTestingScreen from 'containers/AbTestingScreen';


const Screens = ({ widgetTabAvailable, abTestsTabAvailable, activeTab }) => {
  const severalTabsAvailable = widgetTabAvailable && abTestsTabAvailable;
  if (!severalTabsAvailable) {
    if (widgetTabAvailable) { return (<WidgetsScreen />); }
    if (abTestsTabAvailable) { return (<AbTestingScreen />); }
  }

  return (
    <SwipeableViews axis="x" index={activeTab}>
      {widgetTabAvailable &&
      <WidgetsScreen />
      }
      {abTestsTabAvailable &&
      <AbTestingScreen />
      }
    </SwipeableViews>
  );
};

Screens.propTypes = {
  activeTab: PropTypes.number.isRequired,
  widgetTabAvailable: PropTypes.bool.isRequired,
  abTestsTabAvailable: PropTypes.bool.isRequired,
};

export default Screens;
