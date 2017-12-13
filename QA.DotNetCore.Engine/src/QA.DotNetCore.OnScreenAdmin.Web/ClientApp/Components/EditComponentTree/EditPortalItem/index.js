import React, { Fragment, Component } from 'react';
import PropTypes from 'prop-types';
import EditPortal from '../EditPortal';
import EditControl from '../EditControl';

class EditPortalItem extends Component {
  handleToggleClick = () => {
    this.props.toggleComponent(this.props.onScreenId);
  }

  renderEditPortal = () => {
    const {
      onScreenId,
      properties,
      nestLevel,
      maxNestLevel,
      selectedComponentId,
      showAllZones,
      showAllWidgets,
      side,
      type,
    } = this.props;
    const isSelected = selectedComponentId === onScreenId;

    return (
      <EditPortal type={type} onScreenId={onScreenId}>
        <EditControl
          properties={properties}
          type={type}
          isSelected={isSelected}
          nestLevel={nestLevel}
          maxNestLevel={maxNestLevel}
          showAllZones={showAllZones}
          showAllWidgets={showAllWidgets}
          side={side}
          handleToggleClick={this.handleToggleClick}
        />
      </EditPortal>
    );
  }

  renderSubtree = (subtree) => {
    if (!subtree) {
      return null;
    }

    return (
      <Fragment>
        {subtree}
      </Fragment>
    );
  }


  render() {
    const {
      children,
      selectedComponentId,
      showAllWidgets,
      showAllZones,
      side,
      toggleComponent,
      maxNestLevel,
    } = this.props;
    let subtree = null;

    if (children.length > 0) {
      subtree = children.map(child => (
        <EditPortalItem
          {...child}
          key={child.onScreenId}
          selectedComponentId={selectedComponentId}
          showAllWidgets={showAllWidgets}
          showAllZones={showAllZones}
          side={side}
          toggleComponent={toggleComponent}
          maxNestLevel={maxNestLevel}
        >
          {child.children}
        </EditPortalItem>
      ));
    }

    return (
      <Fragment>
        { this.renderEditPortal() }
        { this.renderSubtree(subtree) }
      </Fragment>
    );
  }
}

EditPortalItem.propTypes = {
  children: PropTypes.arrayOf(PropTypes.object).isRequired,
  toggleComponent: PropTypes.func.isRequired,
  onScreenId: PropTypes.string.isRequired,
  properties: PropTypes.oneOfType([
    PropTypes.shape({
      widgetId: PropTypes.string.isRequired,
      title: PropTypes.string.isRequired,
      alias: PropTypes.string.isRequired,
    }),
    PropTypes.shape({
      zoneName: PropTypes.string.isRequired,
      isRecursive: PropTypes.bool.isRequired,
      isGlobal: PropTypes.bool.isRequired,
    }),
  ]).isRequired,
  nestLevel: PropTypes.number.isRequired,
  maxNestLevel: PropTypes.number.isRequired,
  selectedComponentId: PropTypes.string.isRequired,
  showAllZones: PropTypes.bool.isRequired,
  showAllWidgets: PropTypes.bool.isRequired,
  type: PropTypes.string.isRequired,
  side: PropTypes.string.isRequired,
};

export default EditPortalItem;
