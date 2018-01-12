/* eslint-disable */
import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import {deepPurple} from 'material-ui/colors';
import Typography from 'material-ui/Typography';
import Radio, { RadioGroup } from 'material-ui/Radio';
import List, { ListItem, ListItemIcon, ListItemText, ListItemSecondaryAction } from 'material-ui/List';
import Popover from 'material-ui/Popover';
import Tooltip from 'material-ui/Tooltip';
import IconButton from 'material-ui/IconButton';
import Button from 'material-ui/Button';
import MoreHoriz from 'material-ui-icons/MoreHoriz';
import PlayArrow from 'material-ui-icons/PlayArrow';

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
  lisItemActive: {
    backgroundColor: deepPurple['500'],
    '&:hover': {
      backgroundColor: deepPurple['800']
    }
  },
  listText: {
    fontSize: theme.typography.fontSize,
  },
  listTextActive: {
    fontSize: theme.typography.fontSize,
    fontWeight: 'bold',
    color: 'white',
  },
  actionTooltip: {
    fontSize: '11px',
    width: 90,
  }
});

const TestDetails = (props) => {
  const { classes, comment, variants, choice } = props;
  const isActive = (i) => i === choice;

  return (
    <Fragment>
      <Typography className={classes.commentRoot} component="div">
        <p className={classes.commentInner}>{comment}</p>
      </Typography>
      <List className={classes.list}>
        {variants.map((el, i) => (
          <ListItem key={el.percent} button className={isActive(i) ? classes.lisItemActive : ''} >
            <ListItemText
              primary={`# ${i} - ${el.percent}%`}
              classes={{text: isActive(i) ? classes.listTextActive : classes.listText}}
            />
            {/* <ListItemText primary={``} classes={{text: classes.listText}} /> */}
            {!isActive(i) &&
              <ListItemSecondaryAction>
                <Tooltip
                  id="runCase"
                  placement="left"
                  title="Turn this case"
                  classes={{tooltip: classes.actionTooltip}}
                >
                  <IconButton>
                    <PlayArrow />
                  </IconButton>
                </Tooltip>
              </ListItemSecondaryAction>
            }
          </ListItem>
        ))}
      </List>
    </Fragment>);
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
