import _ from 'lodash';
import { ONSCREEN_FEATURES } from 'constants/features';

const sortOrder = (feature) => {
  switch (feature) {
    case ONSCREEN_FEATURES.WIDGETS_MANAGEMENT:
      return 0;
    case ONSCREEN_FEATURES.ABTESTS:
      return 1;
    default:
      return 100;
  }
};

const mapFeature = (featureString) => {
  const trimmedLoweredFeature = _.trim(featureString).toLowerCase();
  if (trimmedLoweredFeature === 'widgets') { return ONSCREEN_FEATURES.WIDGETS_MANAGEMENT; }
  if (trimmedLoweredFeature === 'abtests') { return ONSCREEN_FEATURES.ABTESTS; }
  return null;
};

/* eslint-disable import/prefer-default-export */
export const getAvailableFeatures = () => {
  const features = window.onScreenFeatures;
  const splittedFeatures = _.split(features, ',');
  console.log('splittedFeatures', splittedFeatures);
  const mapped = _.map(splittedFeatures, f => mapFeature(f));
  console.log(mapped);
  const withoutNulls = _.reject(mapped, _.isNull);
  return _.sortBy(withoutNulls, [f => sortOrder(f)]);
};

export const isTestsAvailable = () => window.onScreenFeatures.indexOf('AbTests') !== -1;
export const isWidgetsAvailable = () => window.onScreenFeatures.indexOf('Widgets') !== -1;
