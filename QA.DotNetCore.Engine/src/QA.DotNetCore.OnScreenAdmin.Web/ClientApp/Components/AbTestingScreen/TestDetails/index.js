/* eslint-disable */
import React, { Component, Fragment } from 'react';
import { findDOMNode } from 'react-dom';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Typography from 'material-ui/Typography';
import Radio, { RadioGroup } from 'material-ui/Radio';
import List, {
  ListItem,
  ListItemIcon,
  ListItemText,
  ListItemSecondaryAction
} from 'material-ui/List';
import Popover from 'material-ui/Popover';
import Tooltip from 'material-ui/Tooltip';
import IconButton from 'material-ui/IconButton';
import Button from 'material-ui/Button';
import MoreHoriz from 'material-ui-icons/MoreHoriz';
import PlayArrow from 'material-ui-icons/PlayArrow';
import TestCaseDetails from '../TestCaseDetails';

const styles = theme => ({
  commentRoot: {
    fontSize: theme.typography.fontSize,
    display: 'flex',
    alignItems: 'center',
  },
  commentInner: {
    margin: 0,
  },
  formControl: {
    fontSize: theme.typography.fontSize,
  },
  itemLabel: {
    fontSize: theme.typography.fontSize,
    marginTop: 1,
  },
  list: {
    marginTop: 15,
    padding: 0,
  },
});

class TestDetails extends Component {

  render() {
    const { classes, comment, variants, choice } = this.props;
    const isActive = (i) => i === choice;

    return (
      <Fragment>
        <Typography className={classes.commentRoot} component="div">
          <p className={classes.commentInner}>{comment}</p>
        </Typography>
        <List className={classes.list}>
          {variants.map((variant, i) => (
            <TestCaseDetails
              key={variant.percent}
              data={variant}
              index={i}
              active={isActive(i)}
            />
          ))}
        </List>
      </Fragment>);
  }
};

TestDetails.propTypes = {
  classes: PropTypes.object.isRequired,
  comment: PropTypes.string.isRequired,
  variants: PropTypes.array.isRequired,
  choice: PropTypes.number,
};

TestDetails.defaultProps = {
  choice: null,
};

export default withStyles(styles)(TestDetails);
