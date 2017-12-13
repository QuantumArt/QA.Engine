import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import List from 'material-ui/List';
import Button from 'material-ui/Button';
import AvailableWidget from '../AvailableWidget';

const AvailableWidgetsList = ({ availableWidgets, onSelectWidget, onCancel }) => (
  <Fragment>
    <List>
      {availableWidgets.map(widget => (
        <AvailableWidget
          {...widget}
          key={widget.id}
          onSelectWidget={onSelectWidget}
        />
      ))}
    </List>
    <Button raised onClick={onCancel}>Cancel</Button>
  </Fragment>
);


AvailableWidgetsList.propTypes = {
  availableWidgets: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.number.isRequired,
      discriminator: PropTypes.string.isRequired,
      typeName: PropTypes.string.isRequired,
      isPage: PropTypes.bool.isRequired,
      title: PropTypes.string.isRequired,
      description: PropTypes.string,
      iconUrl: PropTypes.string.isRequired,
    }).isRequired,
  ),
  onSelectWidget: PropTypes.func.isRequired,
  onCancel: PropTypes.func.isRequired,
};

AvailableWidgetsList.defaultProps = {
  availableWidgets: null,
};

export default AvailableWidgetsList;
