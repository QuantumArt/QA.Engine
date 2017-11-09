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
import Button from 'material-ui/Button';
import ExpandLess from 'material-ui-icons/ExpandLess';
import ExpandMore from 'material-ui-icons/ExpandMore';
import Widgets from 'material-ui-icons/Widgets';
import PanoramaHorizontal from 'material-ui-icons/PanoramaHorizontal';
import IconButton from 'material-ui/IconButton';
import Collapse from 'material-ui/transitions/Collapse';
import EditComponent from '../EditComponent';

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

  handleSelectClick = () => {
    this.props.onSelectComponent(this.props.onScreenId);
    scrollToElement(`[data-qa-component-on-screen-id="${this.props.onScreenId}"]`, {
      offset: 0,
      ease: 'in-out-expo', // https://github.com/component/ease#aliases
      duration: 1500,
    });
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
      onSelectComponent,
      type,
      onScreenId,
      properties,
      children,
      classes,
      nestLevel,
    } = this.props;
    let currentLevel = nestLevel;

    if (children.length > 0) {
      currentLevel += 1;
      const subtree = children.map(child => (
        <ComponentItem
          properties={child.properties}
          key={child.onScreenId}
          type={child.type}
          onScreenId={child.onScreenId}
          onSelectComponent={onSelectComponent}
          classes={classes}
          nestLevel={currentLevel}
        >
          {child.children}
        </ComponentItem>
      ));

      return (
        <div>
          <ListItem
            onClick={this.handleSelectClick}
            classes={{ root: classes.listItemRoot }}
            style={{ paddingLeft: nestLevel > 1 ? `${nestLevel}em` : '16px' }}
            button
          >
            <ListItemIcon>
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
          <EditComponent type={type} onScreenId={onScreenId} properties={properties}>
            <Button
              raised
              color="accent"
              component="span"
              onClick={this.handleSelectClick}
            >
              Edit {type === 'zone' ? properties.zoneName : properties.title}
            </Button>
          </EditComponent>
          <Collapse in={this.state.opened}>
            {subtree}
          </Collapse>
        </div>
      );
    }

    return (
      <div>
        <ListItem
          onClick={this.handleSelectClick}
          classes={{ root: classes.listItemRoot }}
          style={{ paddingLeft: nestLevel > 1 ? `${nestLevel}em` : '16px' }}
          button
        >
          <ListItemIcon>
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
        <EditComponent type={type} onScreenId={onScreenId} properties={properties}>
          <Button
            raised
            color="accent"
            component="span"
            onClick={this.handleSelectClick}
          >
            Edit {type === 'zone' ? properties.zoneName : properties.title}
          </Button>
        </EditComponent>
      </div>
    );
  }
}

ComponentItem.propTypes = {
  onSelectComponent: PropTypes.func.isRequired,
  type: PropTypes.string.isRequired,
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
  children: PropTypes.arrayOf(PropTypes.object).isRequired,
  classes: PropTypes.object.isRequired,
  nestLevel: PropTypes.number.isRequired,
};

ComponentItem.defaultProps = {
  nestLevel: 1,
};


export default withStyles(styles)(ComponentItem);
