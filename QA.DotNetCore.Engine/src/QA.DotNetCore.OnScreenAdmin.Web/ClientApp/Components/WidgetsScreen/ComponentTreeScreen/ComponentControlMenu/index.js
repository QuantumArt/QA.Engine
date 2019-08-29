import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Menu, { MenuItem } from 'material-ui/Menu';
import IconButton from 'material-ui/IconButton';
import MoreVertIcon from 'material-ui-icons/MoreVert';

const styles = theme => ({
  menuItem: {
    fontSize: theme.spacing.unit * 1.8,
  },
});

class ComponentControlMenu extends Component {
  state = {
    anchorEl: null,
  };
  handleClick = (event) => {
    this.setState({ anchorEl: event.currentTarget });
  };

  handleRequestClose = () => {
    this.setState({ anchorEl: null });
  };

  handleEditWidget = () => {
    const { onEditWidget, onScreenId } = this.props;
    this.handleRequestClose();
    onEditWidget(onScreenId);
  }

  handleAddWidgetToZone = () => {
    const { onAddWidgetToZone, onScreenId, zoneName } = this.props;
    this.handleRequestClose();
    onAddWidgetToZone(onScreenId, zoneName);
  }

  handleAddChildWidget = () => {
    const { onAddChildWidget, onScreenId } = this.props;
    this.handleRequestClose();
    onAddChildWidget(onScreenId);
  }

  handleMoveWidget = () => {
    const { onMoveWidget, onScreenId } = this.props;
    this.handleRequestClose();
    onMoveWidget(onScreenId);
  }

  renderZoneMenu = () => {
    const { classes, isIframe } = this.props;
    const open = Boolean(this.state.anchorEl);
    return (
      <Fragment>
        <IconButton
          aria-label="More"
          aria-owns={open ? 'long-menu' : null}
          aria-haspopup="true"
          onClick={this.handleClick}
        >
          <MoreVertIcon />
        </IconButton>
        <Menu
          id="long-menu"
          anchorEl={this.state.anchorEl}
          open={open}
          onClose={this.handleRequestClose}
        >
          <MenuItem
            key="addWidget"
            onClick={this.handleAddWidgetToZone}
            classes={{ root: classes.menuItem }}
            disabled={!isIframe}
          >
            Add widget
          </MenuItem>
          <MenuItem
            key="anotherZoneAction"
            onClick={this.handleRequestClose}
            classes={{ root: classes.menuItem }}
          >
            Another zone action
          </MenuItem>
        </Menu>
      </Fragment>
    );
  }

  renderWidgetMenu = () => {
    const { classes, isIframe } = this.props;
    const open = Boolean(this.state.anchorEl);
    return (
      <Fragment>
        <IconButton
          aria-label="More"
          aria-owns={open ? 'long-menu' : null}
          aria-haspopup="true"
          onClick={this.handleClick}
        >
          <MoreVertIcon />
        </IconButton>
        <Menu
          id="long-menu"
          anchorEl={this.state.anchorEl}
          open={open}
          onClose={this.handleRequestClose}
        >
          <MenuItem
            key="editWidget"
            onClick={this.handleEditWidget}
            classes={{ root: classes.menuItem }}
            disabled={!isIframe}
          >
            Edit
          </MenuItem>
          <MenuItem
            key="addChildWidget"
            onClick={this.handleAddChildWidget}
            classes={{ root: classes.menuItem }}
            disabled={!isIframe}
          >
            Add child widget
          </MenuItem>
          <MenuItem
            key="moveWidget"
            onClick={this.handleMoveWidget}
            classes={{ root: classes.menuItem }}
            disabled={!isIframe}
          >
            Move
          </MenuItem>
        </Menu>
      </Fragment>
    );
  }

  render() {
    const {
      type,
    } = this.props;

    const isWidget = type !== 'zone';
    if (!isWidget) {
      return this.renderZoneMenu();
    }

    return this.renderWidgetMenu();
  }
}

ComponentControlMenu.propTypes = {
  onEditWidget: PropTypes.func.isRequired,
  onAddWidgetToZone: PropTypes.func.isRequired,
  onAddChildWidget: PropTypes.func.isRequired,
  onMoveWidget: PropTypes.func.isRequired,
  type: PropTypes.string.isRequired,
  zoneName: PropTypes.string.isRequired,
  onScreenId: PropTypes.string.isRequired,
  classes: PropTypes.object.isRequired,
  isIframe: PropTypes.bool.isRequired,
};

export default withStyles(styles)(ComponentControlMenu);
