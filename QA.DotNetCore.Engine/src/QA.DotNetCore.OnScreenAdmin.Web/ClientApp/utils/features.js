import _ from 'lodash';
import { ONSCREEN_FEATURES } from 'constants/features';

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
  return _.reject(mapped, _.isNull);
};
