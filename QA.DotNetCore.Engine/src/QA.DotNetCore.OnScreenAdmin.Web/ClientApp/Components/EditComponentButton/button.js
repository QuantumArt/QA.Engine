import React from 'react';
import PropTypes from 'prop-types';

const EditComponentButton = ({ onClick }) => (
  <button onClick={onClick}>edit</button>
);

EditComponentButton.propTypes = {
  onClick: PropTypes.func.isRequired,
};

export default EditComponentButton;
