import React from 'react';
import PropTypes from 'prop-types';

const ZoneComponentTreeItem = ({ properties, onClick }) => (
  <li onClick={onClick}>
    Zone name: {properties.zoneName}
  </li>
);

ZoneComponentTreeItem.propTypes = {
  onClick: PropTypes.func.isRequired,
  properties: PropTypes.shape({
    zoneName: PropTypes.string.isRequired,
  }).isRequired,
};

export default ZoneComponentTreeItem;
