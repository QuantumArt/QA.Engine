import React from 'react';
import PropTypes from 'prop-types';
import { ListItem } from 'material-ui/List';

const WidgetComponentTreeItem = ({ properties, onClick }) => (
  <ListItem button onClick={onClick}>
    Widget id: {properties.widgetId} - {properties.title}
  </ListItem>
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
