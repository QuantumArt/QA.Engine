/* eslint-disable */
import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Typography from 'material-ui/Typography';
import Radio, { RadioGroup } from 'material-ui/Radio';
import { FormControl, FormControlLabel, FormHelperText } from 'material-ui/Form';

const styles = theme => ({
  comment: {
    fontSize: theme.typography.fontSize,
  },
  formControl: {
    fontSize: theme.typography.fontSize,
  },
  itemLabel: {
    fontSize: theme.typography.fontSize,
    marginTop: 1,
  },
  group: {
    marginTop: 16,
  },
});

const TestDetails = (props) => {
  const { classes, comment, percentage } = props;

  return (
    <Fragment>
      <Typography className={classes.comment}>{comment}</Typography>
      <FormControl component="fieldset" required className={classes.formControl}>
        <RadioGroup
          aria-label="hypothesis"
          name="hypothesis"
          className={classes.group}
          value={'0'}
          // onChange={this.handleChange}
        >
          {percentage.map((el, i) => (
            <FormControlLabel
              value={`${i}`}
              control={<Radio />}
              label={`Variant ${i}`}
              classes={{ label: classes.itemLabel }}
              key={el}
            />
          ))}
        </RadioGroup>
      </FormControl>
    </Fragment>);
};

TestDetails.propTypes = {
  classes: PropTypes.object.isRequired,
  comment: PropTypes.string.isRequired,
  percentage: PropTypes.array.isRequired,
};

export default withStyles(styles)(TestDetails);
