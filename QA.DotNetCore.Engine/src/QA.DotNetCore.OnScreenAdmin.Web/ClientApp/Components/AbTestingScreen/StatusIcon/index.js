import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import SvgIcon from 'material-ui/SvgIcon';
import { deepPurple, red } from 'material-ui/colors';

const styles = {
  statusIcon: {
    width: 20,
    height: 20,
    position: 'absolute',
    top: '50%',
    marginTop: -10,
    left: 19,
    borderRadius: 3,
  },
};

const StatusIcon = (props) => {
  const {
    classes,
    globalActive,
    sessionActive,
    paused,
    stoped,
  } = props;
  const getColor = () => {
    if (globalActive || sessionActive) return deepPurple[500];
    if (stoped || paused) return red[500];

    return 'transparent';
  };

  return (
    <SvgIcon className={classes.statusIcon}>
      <path d="M0 0h24v24H0z" fill={getColor()} />
      {(sessionActive || paused) &&
        <path transform="translate(7, 10) scale(0.7)" fill="white" d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
      }
    </SvgIcon>
  );
};

StatusIcon.propTypes = {
  classes: PropTypes.object.isRequired,
  globalActive: PropTypes.bool.isRequired,
  sessionActive: PropTypes.bool.isRequired,
  paused: PropTypes.bool.isRequired,
  stoped: PropTypes.bool.isRequired,
};

export default withStyles(styles)(StatusIcon);
