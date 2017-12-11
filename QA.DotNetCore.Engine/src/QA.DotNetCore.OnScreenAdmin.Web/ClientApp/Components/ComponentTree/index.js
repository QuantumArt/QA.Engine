import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import List from 'material-ui/List';
import Button from 'material-ui/Button';
import ComponentItem from '../ComponentItem';
import AvailableWidget from '../AvailableWidget';

class ComponentTree extends Component {
  renderComponentsList = () => {
    const {
      components,
      maxNestLevel,
      selectedComponentId,
      onToggleComponent,
      onToggleSubtree,
      onToggleFullSubtree,
      // onEditWidget,
      // onAddWidgetToZone,
      showAllZones,
      showAllWidgets,
      side,
      showAvailableWidgets,
    } = this.props;

    return (
      <List dense >
        {components.map(component => (
          <ComponentItem
            {...component}
            maxNestLevel={maxNestLevel}
            selectedComponentId={selectedComponentId}
            isOpened={component.isOpened}
            key={component.onScreenId}
            onToggleComponent={onToggleComponent}
            onToggleSubtree={onToggleSubtree}
            onToggleFullSubtree={onToggleFullSubtree}
            // onEditWidget={onEditWidget}
            // onAddWidget={onAddWidgetToZone}
            showAllZones={showAllZones}
            showAllWidgets={showAllWidgets}
            side={side}
            showListItem={!showAvailableWidgets}
          />))}
      </List>
    );
  }

  renderAvailableWidgets = () => {
    const {
      showAvailableWidgets,
      availableWidgets,
      onSelectWidgetToAdd,
      onCancelAddWidget,
    } = this.props;
    if (!showAvailableWidgets || !availableWidgets) { return null; }
    return (
      <Fragment>
        <List>
          {availableWidgets.map(widget => (
            <AvailableWidget
              {...widget}
              key={widget.id}
              onSelectWidget={onSelectWidgetToAdd}
            />
          ))}
        </List>
        <Button raised onClick={onCancelAddWidget}>Cancel</Button>
      </Fragment>
    );
  }

  render() {
    return (
      <Fragment>
        { this.renderComponentsList() }
        { this.renderAvailableWidgets() }
      </Fragment>
    );
  }
}

ComponentTree.propTypes = {
  components: PropTypes.arrayOf(
    PropTypes.shape({
      type: PropTypes.string.isRequired,
      onScreenId: PropTypes.string.isRequired,
      isOpened: PropTypes.bool,
      properties: PropTypes.object.isRequired,
      children: PropTypes.array.isRequired,
    }).isRequired,
  ).isRequired,
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
  maxNestLevel: PropTypes.number.isRequired,
  selectedComponentId: PropTypes.string.isRequired,
  onToggleComponent: PropTypes.func.isRequired,
  onToggleSubtree: PropTypes.func.isRequired,
  onToggleFullSubtree: PropTypes.func.isRequired,
  onSelectWidgetToAdd: PropTypes.func.isRequired,
  onCancelAddWidget: PropTypes.func.isRequired,
  showAllZones: PropTypes.bool.isRequired,
  showAllWidgets: PropTypes.bool.isRequired,
  side: PropTypes.string.isRequired,
  showAvailableWidgets: PropTypes.bool.isRequired,
};

ComponentTree.defaultProps = {
  availableWidgets: null,
};

export default ComponentTree;
