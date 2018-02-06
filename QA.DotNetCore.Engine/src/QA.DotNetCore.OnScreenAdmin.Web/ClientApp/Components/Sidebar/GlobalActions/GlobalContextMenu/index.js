import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Menu, { MenuItem } from 'material-ui/Menu';
import IconButton from 'material-ui/IconButton';
import MoreVertIcon from 'material-ui-icons/MoreVert';
import _ from 'lodash';
import { ADD_WIDGET_TO_PAGE_KEY } from 'constants/globalContextMenu';


const styles = theme => ({
  menuItem: {
    fontSize: theme.spacing.unit * 1.8,
  },
});

class GlobalContextMenu extends Component {
  state = {
    anchorEl: null,
  };
  handleClick = (event) => {
    this.setState({ anchorEl: event.currentTarget });
  };

  handleRequestClose = () => {
    this.setState({ anchorEl: null });
  };

  handleAddWidgetToPage = () => {
    const { addWidgetToPage } = this.props;
    this.handleRequestClose();
    addWidgetToPage();
  }

  render() {
    const {
      classes,
      enabledMenuKeys,

    } = this.props;
    const open = Boolean(this.state.anchorEl);
    console.log(enabledMenuKeys);
    if (enabledMenuKeys.length === 0) { return null; }
    const showAddWidgetToPage = _.includes(enabledMenuKeys, ADD_WIDGET_TO_PAGE_KEY);
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
          { showAddWidgetToPage &&
          <MenuItem
            key={ADD_WIDGET_TO_PAGE_KEY}
            onClick={this.handleAddWidgetToPage}
            classes={{ root: classes.menuItem }}
          >
          Add widget
          </MenuItem>
          }
        </Menu>
      </Fragment>
    );
  }
}

// 

GlobalContextMenu.propTypes = {
  addWidgetToPage: PropTypes.func.isRequired,
  enabledMenuKeys: PropTypes.array.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(GlobalContextMenu);
