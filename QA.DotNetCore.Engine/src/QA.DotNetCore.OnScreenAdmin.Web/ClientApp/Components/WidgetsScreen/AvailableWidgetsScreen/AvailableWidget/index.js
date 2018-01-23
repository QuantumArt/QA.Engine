import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import {
  ListItem,
  ListItemText,
  // ListItemSecondaryAction,
} from 'material-ui/List';
import Avatar from 'material-ui/Avatar';

const styles = (theme) => {
  console.log(theme);
  return {
    listItem: {
      height: theme.spacing.unit * 7.5,
      minWidth: 350,
    },
    listItemText: {
      fontSize: theme.typography.fontSize,
    },
    listItemSecondaryAction: {
      paddingRight: 80,
    },

  };
};

class AvailableWidget extends Component {
  handleSelectClick = () => {
    this.props.onSelectWidget(this.props.id);
  }

  render() {
    const {
      title,
      description,
      iconUrl,
      classes,
    } = this.props;

    return (
      <ListItem
        classes={{
          root: classes.listItemRoot,
        }}
        button
        onClick={this.handleSelectClick}
      >
        <Avatar src={iconUrl} />
        <ListItemText
          primary={title}
          secondary={description}
          classes={{ primary: classes.listItemText }}
        />
      </ListItem>


    );
  }
}

AvailableWidget.propTypes = {
  id: PropTypes.number.isRequired,
  // Discriminator: PropTypes.string.isRequired,
  // TypeName: PropTypes.string.isRequired,
  // IsPage: PropTypes.bool.isRequired,
  title: PropTypes.string.isRequired,
  description: PropTypes.string,
  iconUrl: PropTypes.string.isRequired,
  classes: PropTypes.object.isRequired,
  onSelectWidget: PropTypes.func.isRequired,
};

AvailableWidget.defaultProps = {
  description: '',
};

export default withStyles(styles)(AvailableWidget);
