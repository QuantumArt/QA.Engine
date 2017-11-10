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
  state = {
    opened: false,
  }

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
    this.setState({ opened: !this.state.opened });
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
      type,
      onScreenId,
      properties,
      selectedComponentId,
      showAllZones,
      children,
      classes,
      nestLevel,
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
          onToggleComponent={onToggleComponent}
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
                {this.state.opened ? <ExpandLess /> : <ExpandMore />}
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
          <Collapse in={this.state.opened}>
            {subtree}
          </Collapse>
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
      </div>
    );
  }
}

ComponentItem.propTypes = {
  onToggleComponent: PropTypes.func.isRequired,
  type: PropTypes.string.isRequired,
  onScreenId: PropTypes.string.isRequired,
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
};


export default withStyles(styles)(ComponentItem);
