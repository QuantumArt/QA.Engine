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
import ComponentControlMenu from 'containers/WidgetsScreen/componentControlMenu';

const styles = (theme) => {
  console.log(theme);
  return {
    listItem: {
      height: theme.typography.pxToRem(76.8),
    },
    listItemText: {
      fontSize: 14,
    },
    listItemIconSelected: {
      color: deepPurple[500],
    },
    listItemSecondaryAction: {
      paddingRight: 80,
    },
    expandNodeIcon: {
      // width: theme.spacing.unit * 3,
      // height: theme.spacing.unit * 3,
    },
    expandNodeRoot: {
      verticalAlign: 'bottom',
    },
  };
};

class ComponentItem extends Component {
  handleToggleClick = () => {
    const { isMovingWidget, onScreenId, onToggleComponent, onMovingWidgetSelectTargetZone } = this.props;
    if (isMovingWidget) {
      onMovingWidgetSelectTargetZone(onScreenId);
    } else {
      onToggleComponent(onScreenId);
      scrollToElement(`[data-qa-component-on-screen-id="${onScreenId}"]`,
        { offset: -100,
          ease: 'in-out-expo', // https://github.com/component/ease#aliases
          duration: 1500,
        },
      );
    }
  }

  handleOnScreenToggleClick = () => {
    this.handleToggleClick();
    this.props.onToggleFullSubtree(this.props.onScreenId);
  }

  handleSubtreeClick = () => {
    this.props.onToggleSubtree(this.props.onScreenId);
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

  renderContextMenu = (isSelected) => {
    const { isMovingWidget } = this.props;
    if (!isSelected || isMovingWidget) { return null; }

    return (<ComponentControlMenu onScreenId={this.props.onScreenId} />);
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
      isDisabled,
      type,
    } = this.props;
    const isSelected = selectedComponentId === onScreenId;

    return (
      <ListItem
        disabled={isDisabled}
        classes={{
          root: classes.listItem,
          secondaryAction: classes.listItemSecondaryAction,
        }}
        style={{ paddingLeft: nestLevel > 1 ? `${nestLevel * 0.8}em` : '16px' }}
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
          primary={this.renderPrimaryText(type, properties, isDisabled)}
          // secondary={this.renderSecondaryText(type, properties)}
          classes={{ dense: classes.listItemText }}
        />
        <ListItemSecondaryAction>
          { this.renderContextMenu(isSelected) }
          { this.renderCollapseButton(isOpened, subtree, classes) }
        </ListItemSecondaryAction>
      </ListItem>
    );
  }


  render() {
    const {
      onToggleComponent,
      onToggleSubtree,
      onToggleFullSubtree,
      selectedComponentId,
      children,
      classes,
      isOpened,
      maxNestLevel,
      isMovingWidget,
      onMovingWidgetSelectTargetZone,
    } = this.props;
    let subtree = null;

    if (children.length > 0) {
      subtree = children.map(child => (
        <ComponentItem
          properties={child.properties}
          key={child.onScreenId}
          type={child.type}
          onScreenId={child.onScreenId}
          isOpened={child.isOpened}
          onToggleComponent={onToggleComponent}
          onToggleSubtree={onToggleSubtree}
          onToggleFullSubtree={onToggleFullSubtree}
          selectedComponentId={selectedComponentId}
          classes={classes}
          nestLevel={child.nestLevel}
          maxNestLevel={maxNestLevel}
          isDisabled={child.isDisabled}
          isMovingWidget={isMovingWidget}
          onMovingWidgetSelectTargetZone={onMovingWidgetSelectTargetZone}
          // showListItem={showListItem}
        >
          {child.children}
        </ComponentItem>
      ));
    }

    return (
      <Fragment>
        { this.renderListItem(subtree) }
        { this.renderSubtree(isOpened, subtree) }
      </Fragment>
    );
  }
}

ComponentItem.propTypes = {
  onToggleComponent: PropTypes.func.isRequired,
  onToggleFullSubtree: PropTypes.func.isRequired,
  onToggleSubtree: PropTypes.func.isRequired,
  type: PropTypes.string.isRequired,
  onScreenId: PropTypes.string.isRequired,
  isOpened: PropTypes.bool,
  selectedComponentId: PropTypes.string.isRequired,
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
  maxNestLevel: PropTypes.number.isRequired,
  isMovingWidget: PropTypes.bool.isRequired,
  isDisabled: PropTypes.bool.isRequired,
  onMovingWidgetSelectTargetZone: PropTypes.func.isRequired,
  // showListItem: PropTypes.bool.isRequired,
};

ComponentItem.defaultProps = {
  isOpened: false,
};


export default withStyles(styles)(ComponentItem);
