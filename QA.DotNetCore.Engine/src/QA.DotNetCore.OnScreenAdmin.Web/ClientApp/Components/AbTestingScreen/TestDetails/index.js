import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Typography from 'material-ui/Typography';
import TestCaseDetails from '../TestCaseDetails';

const styles = theme => ({
  commentRoot: {
    fontSize: theme.typography.fontSize,
    display: 'flex',
    alignItems: 'center',
    marginBottom: 20,
  },
});

const TestDetails = (props) => {
  const {
    classes,
    comment,
    variants,
    choice,
    setTestCase,
    stoped,
    paused,
    id,
  } = props;
  const variantIsActive = i => i === choice;

  return (
    <Fragment>
      <Typography className={classes.commentRoot} gutterBottom>
        {comment}
      </Typography>
      {variants.map((variant, i) => (
        <TestCaseDetails
          key={variant.percent}
          data={variant}
          index={i}
          id={id}
          active={variantIsActive(i)}
          paused={paused}
          stoped={stoped}
          setTestCase={setTestCase}
        />
      ))}
    </Fragment>
  );
};

TestDetails.propTypes = {
  classes: PropTypes.object.isRequired,
  comment: PropTypes.string.isRequired,
  variants: PropTypes.array.isRequired,
  choice: PropTypes.number,
  stoped: PropTypes.bool.isRequired,
  paused: PropTypes.bool.isRequired,
  setTestCase: PropTypes.func.isRequired,
  id: PropTypes.number.isRequired,
};

TestDetails.defaultProps = {
  choice: null,
};

export default withStyles(styles)(TestDetails);
