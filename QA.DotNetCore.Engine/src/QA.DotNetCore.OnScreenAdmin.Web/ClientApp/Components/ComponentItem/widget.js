import React from 'react';
import PropTypes from 'prop-types';

const WidgetComponentTreeItem = ({ properties, onClick }) => (
  <li onClick={onClick}>
    Widget id: {properties.widgetId} - {properties.title}
  </li>
);

WidgetComponentTreeItem.propTypes = {
  onClick: PropTypes.func.isRequired,
  properties: PropTypes.shape({
    widgetId: PropTypes.string.isRequired,
    title: PropTypes.string.isRequired,
  }).isRequired,
};

WidgetComponentTreeItem.defaultProps = {
  onClick: () => {},
};

export default WidgetComponentTreeItem;
