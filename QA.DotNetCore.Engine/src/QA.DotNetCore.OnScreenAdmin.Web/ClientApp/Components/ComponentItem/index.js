import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import scrollToElement from 'scroll-to-element';
import { withStyles } from 'material-ui/styles';
import {
  ListItem,
  ListItemText,
  ListItemIcon,
  ListItemSecondaryAction,
} from 'material-ui/List';
import ExpandLess from 'material-ui-icons/ExpandLess';
import ExpandMore from 'material-ui-icons/ExpandMore';
import Widgets from 'material-ui-icons/Widgets';
import PanoramaHorizontal from 'material-ui-icons/PanoramaHorizontal';
import IconButton from 'material-ui/IconButton';
import Collapse from 'material-ui/transitions/Collapse';
import { deepPurple } from 'material-ui/colors';
import EditPortal from '../EditPortal';
import EditControl from '../EditControl';
import ComponentControlMenu from '../ComponentControlMenu';

const styles = (theme) => {
  console.log(theme);
  return {
    listItem: {
      height: theme.spacing.unit * 7.5,
      minWidth: 350,
    },
    listItemText: {
      fontSize: theme.typography.fontSize,
    },
    listItemIconSelected: {
      color: deepPurple[500],
    },
    listItemSecondaryAction: {
      paddingRight: 80,
    },
    expandNodeIcon: {
      width: theme.spacing.unit * 3,
      height: theme.spacing.unit * 3,
    },
    expandNodeRoot: {
      verticalAlign: 'bottom',
    },
  };
};

class ComponentItem extends Component {
  handleToggleClick = () => {
    this.props.onToggleComponent(this.props.onScreenId);
    scrollToElement(`[data-qa-component-on-screen-id="${this.props.onScreenId}"]`,
      { offset: -100,
        ease: 'in-out-expo', // https://github.com/component/ease#aliases
        duration: 1500,
      },
    );
  }

  handleSubtreeClick = () => {
    // this.setState({ opened: !this.state.opened });
    this.props.onToggleSubtree(this.props.onScreenId);
  }

  handleEditWidget = () => {
    this.props.onEditWidget(this.props.properties.widgetId);
  }

  handleAddWidget = () => {
    this.props.onAddWidget('some zone id');
  }

  renderPrimaryText = (type, properties) => (type === 'zone'
    ? `${properties.zoneName}`
    : `${properties.title}`);

  renderSecondaryText = (type, properties) => {
    if (type === 'zone') {
      let zoneSettings = '';
      if (properties.isRecursive) zoneSettings += ' recursive';
      if (properties.isGlobal) zoneSettings += ' global';

      return zoneSettings === '' ? 'zone' : `${type}:${zoneSettings}`;
    }

    return `${type}: ID - ${properties.widgetId}`;
  }

  renderContextMenu = (isSelected, type, properties) => {
    if (!isSelected) { return null; }

    return (
      <ComponentControlMenu
        onEditWidget={this.handleEditWidget}
        onAddWidget={this.handleAddWidget}
        properties={properties}
        type={type}
      />
    );
  }

  renderSubtree = (isOpened, subtree) => {
    if (!subtree) {
      return null;
    }

    return (
      <Collapse in={isOpened}>
        {subtree}
      </Collapse>
    );
  }

  renderCollapseButton = (isOpened, subtree, classes) => {
    if (!subtree) {
      return null;
    }
    return (
      <IconButton
        onClick={this.handleSubtreeClick}
        classes={{ icon: classes.expandNodeIcon, root: classes.expandNodeRoot }}
      >
        {isOpened ? <ExpandLess /> : <ExpandMore />}
      </IconButton>
    );
  }

  renderListItem = (subtree) => {
    const {
      onScreenId,
      properties,
      selectedComponentId,
      classes,
      nestLevel,
      isOpened,
      type,
      showListItem,
    } = this.props;
    const isSelected = selectedComponentId === onScreenId;

    if (!showListItem) { return null; }
    return (
      <ListItem
        classes={{
          root: classes.listItemRoot,
          secondaryAction: classes.listItemSecondaryAction,
        }}
        style={{ paddingLeft: nestLevel > 1 ? `${nestLevel}em` : '16px' }}
        onClick={this.handleToggleClick}
        button
      >
        <ListItemIcon
          className={isSelected
            ? classes.listItemIconSelected
            : ''}
        >
          {type === 'zone'
            ? <PanoramaHorizontal />
            : <Widgets />
          }
        </ListItemIcon>
        <ListItemText
          primary={this.renderPrimaryText(type, properties)}
          secondary={this.renderSecondaryText(type, properties)}
          classes={{ text: classes.listItemText }}
        />
        <ListItemSecondaryAction>
          { this.renderContextMenu(isSelected, type, properties) }
          { this.renderCollapseButton(isOpened, subtree, classes) }
        </ListItemSecondaryAction>
      </ListItem>
    );
  }

  renderEditPortal = () => {
    const {
      onScreenId,
      properties,
      selectedComponentId,
      showAllZones,
      type,
    } = this.props;
    const isSelected = selectedComponentId === onScreenId;

    return (
      <EditPortal type={type} onScreenId={onScreenId}>
        <EditControl
          properties={properties}
          type={type}
          isSelected={isSelected}
          showAllZones={showAllZones}
          handleToggleClick={this.handleToggleClick}
        />
      </EditPortal>
    );
  }

  render() {
    const {
      onToggleComponent,
      onToggleSubtree,
      onEditWidget,
      onAddWidget,
      // type,
      // onScreenId,
      // properties,
      selectedComponentId,
      showAllZones,
      children,
      classes,
      nestLevel,
      isOpened,
      showListItem,
    } = this.props;
    let currentLevel = nestLevel;
    let subtree = null;

    if (children.length > 0) {
      currentLevel += 1;
      subtree = children.map(child => (
        <ComponentItem
          properties={child.properties}
          key={child.onScreenId}
          type={child.type}
          onScreenId={child.onScreenId}
          isOpened={child.isOpened}
          onToggleComponent={onToggleComponent}
          onToggleSubtree={onToggleSubtree}
          onEditWidget={onEditWidget}
          onAddWidget={onAddWidget}
          selectedComponentId={selectedComponentId}
          showAllZones={showAllZones}
          classes={classes}
          nestLevel={currentLevel}
          showListItem={showListItem}
        >
          {child.children}
        </ComponentItem>
      ));
    }

    return (
      <Fragment>
        { this.renderListItem(subtree) }
        { this.renderEditPortal() }
        { this.renderSubtree(isOpened, subtree) }
      </Fragment>
    );
  }
}

ComponentItem.propTypes = {
  onToggleComponent: PropTypes.func.isRequired,
  onToggleSubtree: PropTypes.func.isRequired,
  onEditWidget: PropTypes.func.isRequired,
  onAddWidget: PropTypes.func.isRequired,
  type: PropTypes.string.isRequired,
  onScreenId: PropTypes.string.isRequired,
  isOpened: PropTypes.bool,
  selectedComponentId: PropTypes.string.isRequired,
  showAllZones: PropTypes.bool.isRequired,
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
  children: PropTypes.arrayOf(PropTypes.object).isRequired,
  classes: PropTypes.object.isRequired,
  nestLevel: PropTypes.number.isRequired,
  showListItem: PropTypes.bool.isRequired,
};

ComponentItem.defaultProps = {
  nestLevel: 1,
  isOpened: false,
};


export default withStyles(styles)(ComponentItem);