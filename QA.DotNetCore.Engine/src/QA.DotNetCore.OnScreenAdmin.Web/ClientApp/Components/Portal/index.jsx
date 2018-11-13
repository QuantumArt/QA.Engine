import React from 'react';
import ReactDOM from 'react-dom';
import PropTypes from 'prop-types';

export default class Portal extends React.Component {
  render() {
    const doc = typeof document === 'undefined' ? {} : document;
    const body = doc.body;

    if (!body) {
      return null;
    }

    return ReactDOM.createPortal(
      this.props.children,
      body,
    );
  }
}

Portal.propTypes = {
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node).isRequired,
    PropTypes.node.isRequired,
  ]).isRequired,
};
