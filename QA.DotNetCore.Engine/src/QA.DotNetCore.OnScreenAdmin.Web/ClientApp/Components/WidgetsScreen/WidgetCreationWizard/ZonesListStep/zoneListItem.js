import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import {
  ListItem,
  ListItemText,
  ListItemIcon,
  // ListItemSecondaryAction,
} from 'material-ui/List';
import PanoramaHorizontal from 'material-ui-icons/PanoramaHorizontal';

const styles = (theme) => {
  console.log(theme);
  return {
    listItem: {
      height: theme.typography.pxToRem(76.8),
    },
    listItemText: {
      fontSize: theme.typography.fontSize,
    },

  };
};

class ZoneListItem extends Component {
  handleClick = () => {
    const { zoneName, onSelectZone } = this.props;
    onSelectZone(zoneName);
  }

  render() {
    const { classes, zoneName } = this.props;
    return (
      <Fragment>

        <ListItem
          classes={{
            root: classes.listItem,
          }}
          onClick={this.handleClick}
          button
        >
          <ListItemIcon>
            <PanoramaHorizontal />
          </ListItemIcon>
          <ListItemText
            primary={zoneName}
            classes={{ text: classes.listItemText }}
          />
        </ListItem>
      </Fragment>
    );
  }
}


ZoneListItem.propTypes = {
  zoneName: PropTypes.string.isRequired,
  onSelectZone: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,

};

export default withStyles(styles)(ZoneListItem);
