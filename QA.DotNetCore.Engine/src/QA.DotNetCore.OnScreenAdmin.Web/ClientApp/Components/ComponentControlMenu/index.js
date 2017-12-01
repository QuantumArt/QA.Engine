import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Menu, { MenuItem } from 'material-ui/Menu';
import IconButton from 'material-ui/IconButton';
import MoreVertIcon from 'material-ui-icons/MoreVert';

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
    const open = Boolean(this.state.anchorEl);
    return (
      <div>
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
          <MenuItem key="addWidget" onClick={this.handleAddWidget}>Add widget</MenuItem>
          <MenuItem key="anotherZoneAction" onClick={this.handleRequestClose}>Another zone action</MenuItem>
        </Menu>
      </div>
    );
  }

  renderWidgetMenu = () => {
    const open = Boolean(this.state.anchorEl);
    return (
      <div>
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
          <MenuItem key="editWidget" onClick={this.handleEditWidget}>Edit</MenuItem>
          <MenuItem key="dummyMenuItem" onClick={this.handleRequestClose}>Some other widget action</MenuItem>
        </Menu>
      </div>
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

};

export default ComponentControlMenu;
