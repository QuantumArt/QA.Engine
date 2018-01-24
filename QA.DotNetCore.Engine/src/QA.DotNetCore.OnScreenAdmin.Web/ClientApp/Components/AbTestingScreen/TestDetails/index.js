import React, { Fragment } from 'react';
import { v4 } from 'uuid';
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
    globalStopped,
    sessionStopped,
    id,
  } = props;
  const variantIsActive = i => i === choice;

  return (
    <Fragment>
      {comment &&
        <Typography className={classes.commentRoot} gutterBottom>
          {comment}
        </Typography>
      }
      {variants.map((variant, i) => (
        <TestCaseDetails
          key={v4()}
          data={variant}
          index={i}
          id={id}
          active={variantIsActive(i)}
          sessionStopped={sessionStopped}
          globalStopped={globalStopped}
          setTestCase={setTestCase}
        />
      ))}
    </Fragment>
  );
};

TestDetails.propTypes = {
  classes: PropTypes.object.isRequired,
  comment: PropTypes.string,
  variants: PropTypes.array.isRequired,
  choice: PropTypes.number,
  globalStopped: PropTypes.bool.isRequired,
  sessionStopped: PropTypes.bool.isRequired,
  setTestCase: PropTypes.func.isRequired,
  id: PropTypes.number.isRequired,
};

TestDetails.defaultProps = {
  choice: null,
  comment: null,
};

export default withStyles(styles)(TestDetails);
