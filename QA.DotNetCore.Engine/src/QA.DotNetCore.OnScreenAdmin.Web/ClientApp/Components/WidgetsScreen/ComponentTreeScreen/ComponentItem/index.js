/* eslint-disable */
import _ from 'lodash';
import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import scrollToElement from 'scroll-to-element';
import { withStyles } from 'material-ui/styles';
import {
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
} from 'material-ui/List';
import Avatar from 'material-ui/Avatar';
import Tooltip from 'material-ui/Tooltip';
import ExpandLess from 'material-ui-icons/ExpandLess';
import ExpandMore from 'material-ui-icons/ExpandMore';
import Widgets from 'material-ui-icons/Widgets';
import NewWidget from 'material-ui-icons/NewReleases';
import IconButton from 'material-ui/IconButton';
import Icon from 'material-ui/Icon';
import ZoneIcon from 'Components/Icons/Zone';
import Collapse from 'material-ui/transitions/Collapse';
import { deepPurple, red } from 'material-ui/colors';
import ComponentControlMenu from 'containers/WidgetsScreen/componentControlMenu';
import { MAX_COMPONENT_PRIMARY_TEXT_LENGTH } from 'constants/general';

const styles = (theme) => {
  console.log(theme);
  return {
    componentAvatar: {
      borderRadius: 0,
      color: 'inherit',
      width: theme.typography.pxToRem(30),
      height: theme.typography.pxToRem(30),
    },
    componentAvatarSelected: {
      borderRadius: 0,
      color: deepPurple[500],
      fontWeight: 'bold',
      width: theme.typography.pxToRem(30),
      height: theme.typography.pxToRem(30),
    },
    newWidgetOverlay: {
      width: theme.typography.pxToRem(25),
      height: theme.typography.pxToRem(25),
      marginLeft: '-10px',
      marginTop: '-20px',
      color: red[500],
    },
    listItem: {
      height: theme.typography.pxToRem(76.8),
    },
    listItemTextRoot: {
      marginLeft: theme.spacing.unit * 2,
      fontSize: 14,
      justifyContent: 'flex-start',
    },
    listItemTextSelected: {
      fontWeight: 'bold',
      color: deepPurple[500],
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
    tooltip: {
      fontSize: theme.typography.pxToRem(20),
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
      const outlineElem = document.querySelector(`.component--${onScreenId}`);
      if (outlineElem) {
        scrollToElement(`.component--${onScreenId}`,
          { offset: -100,
            ease: 'in-out-expo',
            duration: 1500,
          },
        );
      }
    }
  }

  handleOnScreenToggleClick = () => {
    this.handleToggleClick();
    this.props.onToggleFullSubtree(this.props.onScreenId);
  }

  handleSubtreeClick = () => {
    this.props.onToggleSubtree(this.props.onScreenId);
  }

  renderSecondaryText = (type, properties) => {
    if (type === 'zone') {
      let zoneSettings = '';
      if (properties.isRecursive) zoneSettings += ' recursive';
      if (properties.isGlobal) zoneSettings += ' global';

      return zoneSettings === '' ? 'zone' : `${type}:${zoneSettings}`;
    }

    return `${type}: ID - ${properties.widgetId}`;
  }

  renderListItemText = (primaryText, isSelected, classes) => (
    <ListItemText
      primary={primaryText}
      classes={{
        root: classes.listItemTextRoot,
        primary: isSelected ? classes.listItemTextSelected : classes.primary,
      }}
    />
  )

  renderListItemTextWrapper = (type, properties, isSelected, classes) => {
    const primaryText = type === 'zone'
      ? properties.zoneName
      : `#${properties.widgetId} ${properties.title}`;
    const isPrimaryTextTruncated = primaryText.length > MAX_COMPONENT_PRIMARY_TEXT_LENGTH;
    const textToRender = isPrimaryTextTruncated
      ? `${primaryText.substring(0, MAX_COMPONENT_PRIMARY_TEXT_LENGTH)}…`
      : primaryText;

    return (
      <Tooltip
        title={isPrimaryTextTruncated ? primaryText : ''}
        classes={{ tooltip: classes.tooltip }}
        disableTriggerFocus={!isPrimaryTextTruncated}
        disableTriggerHover={!isPrimaryTextTruncated}
        disableTriggerTouch={!isPrimaryTextTruncated}
      >
        {this.renderListItemText(textToRender, isSelected, classes)}
      </Tooltip>
    );
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

  hasChildWidgets = (children) => {
    if (!children || children.length === 0) { return false; }

    return _.some(children, c => c.type === 'widget' || this.hasChildWidgets(c.children));
  }

  renderCollapseButton = (isOpened, subtree, showOnlyWidgets, hasChildWidgets, classes) => {
    if (!subtree) { return null; }
    if (showOnlyWidgets && !hasChildWidgets) { return null; }

    return (
      <IconButton
        onClick={this.handleSubtreeClick}
        classes={{ root: classes.expandNodeRoot }}
      >
        <Icon classes={{ root: classes.expandNodeIcon }}>
          {isOpened ? <ExpandLess /> : <ExpandMore />}
        </Icon>
      </IconButton>
    );
  }

  renderListItemIcon = (type, properties, isSelected, classes) => {
    const className = isSelected ? classes.componentAvatarSelected : classes.componentAvatar;
    if (type === 'zone') {
      return (
        <ZoneIcon className={className} />
      );
    }

    if (properties.widgetTypeIconSrc) {
      return (
        <Fragment>
          {properties.widgetTypeIconSrc
            ? (<Avatar className={className} src={properties.widgetTypeIconSrc} />)
            : (<Avatar className={className}><Widgets /></Avatar>)
          }
          {!properties.published && (<NewWidget className={classes.newWidgetOverlay} />)}
        </Fragment>
      );
    }
    return (<Avatar className={className}><Widgets /></Avatar>);
  }

  renderListItem = (subtree, hasChildWidgets) => {
    const {
      onScreenId,
      properties,
      selectedComponentId,
      classes,
      itemLevel,
      isOpened,
      isDisabled,
      type,
      showOnlyWidgets,
    } = this.props;
    const isSelected = selectedComponentId === onScreenId;

    return (
      <ListItem
        disabled={isDisabled}
        classes={{
          root: classes.listItem,
          secondaryAction: classes.listItemSecondaryAction,
        }}
        style={{ paddingLeft: itemLevel > 1 ? `${itemLevel * 1}em` : '16px' }}
        onClick={this.handleToggleClick}
        button
      >
        {this.renderListItemIcon(type, properties, isSelected, classes)}
        {this.renderListItemTextWrapper(type, properties, isSelected, classes)}
        <ListItemSecondaryAction>
          { this.renderContextMenu(isSelected) }
          { this.renderCollapseButton(isOpened, subtree, showOnlyWidgets, hasChildWidgets, classes) }
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
      isMovingWidget,
      onMovingWidgetSelectTargetZone,
      showOnlyWidgets,
      type,
      itemLevel,
    } = this.props;
    let subtree = null;

    const renderItem = !(showOnlyWidgets && type === 'zone');
    const childNestLevel = renderItem ? itemLevel + 1 : itemLevel;
    const hasChildWidgets = this.hasChildWidgets(children);


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
          isDisabled={child.isDisabled}
          isMovingWidget={isMovingWidget}
          onMovingWidgetSelectTargetZone={onMovingWidgetSelectTargetZone}
          showOnlyWidgets={showOnlyWidgets}
          itemLevel={childNestLevel}
        >
          {child.children}
        </ComponentItem>
      ));
    }

    return (
      <Fragment>
        { renderItem && this.renderListItem(subtree, hasChildWidgets) }
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
  itemLevel: PropTypes.number.isRequired,
  // maxNestLevel: PropTypes.number.isRequired,
  isMovingWidget: PropTypes.bool.isRequired,
  isDisabled: PropTypes.bool.isRequired,
  onMovingWidgetSelectTargetZone: PropTypes.func.isRequired,
  showOnlyWidgets: PropTypes.bool.isRequired,
  // showListItem: PropTypes.bool.isRequired,
};

ComponentItem.defaultProps = {
  isOpened: false,
};


export default withStyles(styles)(ComponentItem);
