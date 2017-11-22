import React, { Component } from 'react';
import PropTypes from 'prop-types';
// import Menu, { MenuItem } from 'material-ui/Menu';
import IconButton from 'material-ui/IconButton';
import MoreVertIcon from 'material-ui-icons/MoreVert';
// import { withStyles } from 'material-ui/styles';


// const styles = (theme) => {
//   console.log(theme);
//   return {

//   };
// };

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


  render() {
    const {
      type,
      onEditWidget,
    } = this.props;

    const isWidget = type !== 'zone';
    if (!isWidget) {
      return (
        <div />
      );
    }

    return (
      <div>
        <IconButton
          onClick={onEditWidget}
          aria-label="More"
        >
          <MoreVertIcon />
        </IconButton>

      </div>
    );
  }
}

ComponentControlMenu.propTypes = {
  onEditWidget: PropTypes.func.isRequired,
  type: PropTypes.string.isRequired,

};

export default ComponentControlMenu;
