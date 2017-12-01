import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Menu, { MenuItem } from 'material-ui/Menu';
import IconButton from 'material-ui/IconButton';
import MoreVertIcon from 'material-ui-icons/MoreVert';

const styles = ({
  menuItem: {
    fontSize: 15,
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
    const { onEditWidget } = this.props;
    this.handleRequestClose();
    onEditWidget();
  }

  handleAddWidget = () => {
    const { onAddWidget } = this.props;
    this.handleRequestClose();
    onAddWidget();
  }

  renderZoneMenu = () => {
    const { classes } = this.props;
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
          onRequestClose={this.handleRequestClose}
        >
          <MenuItem
            key="addWidget"
            onClick={this.handleAddWidget}
            classes={{ root: classes.menuItem }}
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
    const { classes } = this.props;
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
          onRequestClose={this.handleRequestClose}
        >
          <MenuItem
            key="editWidget"
            onClick={this.handleEditWidget}
            classes={{ root: classes.menuItem }}
          >
            Edit
          </MenuItem>
          <MenuItem
            key="dummyMenuItem"
            onClick={this.handleRequestClose}
            classes={{ root: classes.menuItem }}
          >
            Some other widget action
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
  onAddWidget: PropTypes.func.isRequired,
  type: PropTypes.string.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(ComponentControlMenu);
