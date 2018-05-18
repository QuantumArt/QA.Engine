import React, { Fragment } from 'react';
import { v4 } from 'uuid';
import PropTypes from 'prop-types';
import moment from 'moment';
import { withStyles } from 'material-ui/styles';
import Typography from 'material-ui/Typography';
import Divider from 'material-ui/Divider';
import TestCaseDetails from '../TestCaseDetails';

const styles = theme => ({
  commentRoot: {
    fontSize: theme.typography.fontSize,
    display: 'flex',
    alignItems: 'center',
    marginBottom: 20,
  },
  DateSection: {
    fontSize: theme.typography.fontSize,
  },
  DateSectionItem: {
    margin: '10px 0',
  },
  DateSectionTitle: {
    marginBottom: 5,
    fontWeight: 'bold',
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
    startDate,
    endDate,
  } = props;
  const variantIsActive = i => i === choice;
  const startDateFormated = startDate ?
    moment(startDate).format('dddd, MMMM Do YYYY, h:mm a').toString() :
    null;
  const endDateFormated = endDate ?
    moment(endDate).format('dddd, MMMM Do YYYY, h:mm a').toString() :
    null;
  // const currentDate = moment().toString();
  return (
    <Fragment>
      {comment &&
        <Typography className={classes.commentRoot} gutterBottom>
          {comment}
        </Typography>
      }
      <div className={classes.DateSection}>
        {startDateFormated &&
          <div className={classes.DateSectionItem}>
            <Typography className={classes.DateSectionTitle} variant="subheading">Начало теста</Typography>
            <Typography variant="body1">{startDateFormated}</Typography>
          </div>
        }
        {startDateFormated && endDateFormated &&
          <Divider />
        }
        {endDateFormated &&
          <div className={classes.DateSectionItem}>
            <Typography className={classes.DateSectionTitle} variant="subheading">Окончание теста</Typography>
            <Typography variant="body1">{endDateFormated}</Typography>
          </div>
        }
      </div>
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
  startDate: PropTypes.string,
  endDate: PropTypes.string,
};

TestDetails.defaultProps = {
  choice: null,
  comment: null,
  startDate: null,
  endDate: null,
};

export default withStyles(styles)(TestDetails);
