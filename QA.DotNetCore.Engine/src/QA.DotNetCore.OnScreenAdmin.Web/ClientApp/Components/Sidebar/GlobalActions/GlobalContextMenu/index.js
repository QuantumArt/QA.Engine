import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Menu, { MenuItem } from 'material-ui/Menu';
import IconButton from 'material-ui/IconButton';
import MenuIcon from 'material-ui-icons/Menu';
import _ from 'lodash';
import { ADD_WIDGET_TO_PAGE_KEY, TOGGLE_SHOW_ONLY_WIDGETS_KEY, EDIT_PAGE_KEY } from 'constants/globalContextMenu';


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

  handleToggleShowOnlyWidgets = () => {
    const { toggleShowOnlyWidgets } = this.props;
    this.handleRequestClose();
    toggleShowOnlyWidgets();
  }

  handleEditPage = () => {
    const { editPage } = this.props;
    this.handleRequestClose();
    editPage();
  }

  render() {
    const {
      classes,
      enabledMenuKeys,
      showOnlyWidgets,
      isIframe,
    } = this.props;
    const open = Boolean(this.state.anchorEl);
    const showAddWidgetToPage = _.includes(enabledMenuKeys, ADD_WIDGET_TO_PAGE_KEY);
    const showToggleShowOnlyWidgets = _.includes(enabledMenuKeys, TOGGLE_SHOW_ONLY_WIDGETS_KEY);
    return (
      <Fragment>
        <IconButton
          aria-label="More"
          aria-owns={open ? 'long-menu' : null}
          aria-haspopup="true"
          onClick={this.handleClick}
          disabled={enabledMenuKeys.length === 0}
        >
          <MenuIcon />
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
            disabled={!isIframe}
          >
          Add widget
          </MenuItem>
          }
          { showToggleShowOnlyWidgets &&
          <MenuItem
            key={TOGGLE_SHOW_ONLY_WIDGETS_KEY}
            onClick={this.handleToggleShowOnlyWidgets}
            classes={{ root: classes.menuItem }}
          >
            {showOnlyWidgets ? 'Tree view: zones and widgets' : 'Tree view: only widgets'}
          </MenuItem>
          }
          <MenuItem
            key={EDIT_PAGE_KEY}
            onClick={this.handleEditPage}
            classes={{ root: classes.menuItem }}
            disabled={!isIframe}
          >
            Edit page
          </MenuItem>
        </Menu>
      </Fragment>
    );
  }
}

//

GlobalContextMenu.propTypes = {
  enabledMenuKeys: PropTypes.array.isRequired,
  toggleShowOnlyWidgets: PropTypes.func.isRequired,
  addWidgetToPage: PropTypes.func.isRequired,
  showOnlyWidgets: PropTypes.bool.isRequired,
  classes: PropTypes.object.isRequired,
  isIframe: PropTypes.bool.isRequired,
  editPage: PropTypes.func.isRequired,
};

export default withStyles(styles)(GlobalContextMenu);
