import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import { ListItem, ListItemIcon, ListItemText } from 'material-ui/List';
// import Collapse from 'material-ui/transitions/Collapse';
import StarBorder from 'material-ui-icons/StarBorder';

const styles = theme => ({
  nested: {
    paddingLeft: theme.spacing.unit * 4,
  },
});

const ZoneComponentTreeItem = ({ properties, onClick }) => (
  <ListItem button onClick={onClick} className="nested">
    <ListItemIcon>
      <StarBorder />
    </ListItemIcon>
    <ListItemText disableTypography primary={`zone name: ${properties.zoneName}`} />
  </ListItem>
);

ZoneComponentTreeItem.propTypes = {
  onClick: PropTypes.func.isRequired,
  properties: PropTypes.shape({
    zoneName: PropTypes.string.isRequired,
  }).isRequired,
};

export default withStyles(styles)(ZoneComponentTreeItem);
