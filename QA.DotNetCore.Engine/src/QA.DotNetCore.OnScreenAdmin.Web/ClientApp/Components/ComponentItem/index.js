import React, { Component } from 'react';
import PropTypes from 'prop-types';
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
  };
};

class ComponentItem extends Component {
  state = {
    opened: false,
  }

  handleSelectClick = () => {
    this.props.onSelectComponent(this.props.properties.onScreenId);
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
          key={child.properties.onScreenId}
          type={child.type}
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
              <IconButton onClick={this.handleSubtreeClick}>
                {this.state.opened ? <ExpandLess /> : <ExpandMore />}
              </IconButton>
            </ListItemSecondaryAction>
          </ListItem>
          <EditComponent type={type} properties={properties}>
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
        <EditComponent type={type} properties={properties}>
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

  properties: PropTypes.oneOfType([
    PropTypes.shape({
      onScreenId: PropTypes.string.isRequired,
      widgetId: PropTypes.string.isRequired,
      title: PropTypes.string.isRequired,
      alias: PropTypes.string.isRequired,
    }),
    PropTypes.shape({
      onScreenId: PropTypes.string.isRequired,
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
