import React, { Component } from 'react';
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
      minWidth: 250,
    },
    listItemText: {
      fontSize: theme.typography.fontSize,
    },
    listItemIconSelected: {
      color: deepPurple[500],
    },
    expandNodeIcon: {
      width: theme.spacing.unit * 3,
      height: theme.spacing.unit * 3,
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

  renderSecondaryText = (type, properties) => {
    if (type === 'zone') {
      let zoneSettings = '';
      if (properties.isRecursive) zoneSettings += ' recursive';
      if (properties.isGlobal) zoneSettings += ' global';

      return zoneSettings === '' ? 'zone' : `${type}:${zoneSettings}`;
    }

    return `${type}: ID - ${properties.widgetId}`;
  }

  render() {
    const {
      onToggleComponent,
      onToggleSubtree,
      onEditWidget,
      type,
      onScreenId,
      properties,
      selectedComponentId,
      showAllZones,
      children,
      classes,
      nestLevel,
      isOpened,
    } = this.props;
    const isSelected = selectedComponentId === onScreenId;
    let currentLevel = nestLevel;

    if (children.length > 0) {
      currentLevel += 1;
      const subtree = children.map(child => (
        <ComponentItem
          properties={child.properties}
          key={child.onScreenId}
          type={child.type}
          onScreenId={child.onScreenId}
          isOpened={child.isOpened}
          onToggleComponent={onToggleComponent}
          onToggleSubtree={onToggleSubtree}
          onEditWidget={onEditWidget}
          selectedComponentId={selectedComponentId}
          showAllZones={showAllZones}
          classes={classes}
          nestLevel={currentLevel}

        >
          {child.children}
        </ComponentItem>
      ));

      return (
        <div>
          <ListItem
            onClick={this.handleToggleClick}
            classes={{ root: classes.listItemRoot }}
            style={{ paddingLeft: nestLevel > 1 ? `${nestLevel}em` : '16px' }}
            button
          >
            <ListItemIcon className={isSelected
              ? classes.listItemIconSelected
              : ''}
            >
              {type === 'zone'
                ? <PanoramaHorizontal />
                : <Widgets />
              }
            </ListItemIcon>
            <ListItemText
              primary={type === 'zone'
                ? `${properties.zoneName}`
                : `${properties.title}`
              }
              secondary={this.renderSecondaryText(type, properties)}
              classes={{ text: classes.listItemText }}
            />
            <ListItemSecondaryAction>
              <IconButton
                onClick={this.handleSubtreeClick}
                classes={{ icon: classes.expandNodeIcon }}
              >
                {isOpened ? <ExpandLess /> : <ExpandMore />}
              </IconButton>
            </ListItemSecondaryAction>
          </ListItem>
          <EditPortal type={type} onScreenId={onScreenId}>
            <EditControl
              properties={properties}
              type={type}
              isSelected={isSelected}
              showAllZones={showAllZones}
              handleToggleClick={this.handleToggleClick}
            />
          </EditPortal>
          <Collapse in={isOpened}>
            {subtree}
          </Collapse>
          <ComponentControlMenu
            onEditWidget={this.handleEditWidget}
            properties={properties}
            type={type}
          />
        </div>
      );
    }

    return (
      <div>
        <ListItem
          onClick={this.handleToggleClick}
          classes={{ root: classes.listItemRoot }}
          style={{ paddingLeft: nestLevel > 1 ? `${nestLevel}em` : '16px' }}
          button
        >
          <ListItemIcon className={isSelected
            ? classes.listItemIconSelected
            : ''}
          >
            {type === 'zone'
              ? <PanoramaHorizontal />
              : <Widgets />
            }
          </ListItemIcon>
          <ListItemText
            primary={type === 'zone'
              ? `${properties.zoneName}`
              : `${properties.title}`
            }
            secondary={this.renderSecondaryText(type, properties)}
            classes={{ text: classes.listItemText }}
          />
        </ListItem>
        <EditPortal type={type} onScreenId={onScreenId} properties={properties}>
          <EditControl
            properties={properties}
            type={type}
            isSelected={isSelected}
            showAllZones={showAllZones}
            handleToggleClick={this.handleToggleClick}
          />
        </EditPortal>
        <ComponentControlMenu
          onEditWidget={this.handleEditWidget}
          properties={properties}
          type={type}
        />
      </div>
    );
  }
}

ComponentItem.propTypes = {
  onToggleComponent: PropTypes.func.isRequired,
  onToggleSubtree: PropTypes.func.isRequired,
  onEditWidget: PropTypes.func.isRequired,
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
};

ComponentItem.defaultProps = {
  nestLevel: 1,
  isOpened: false,
};


export default withStyles(styles)(ComponentItem);
