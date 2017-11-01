import { Component } from 'react';
import PropTypes from 'prop-types';
import ReactDOM from 'react-dom';
import './editcomponent.css';

class EditComponent extends Component {
  constructor(props) {
    super(props);
    const { type, properties: { widgetId, zoneName } } = props;

    if (type === 'zone') {
      this.root = document.querySelector(`[data-qa-zone-name="${zoneName}"]`);
    } else {
      this.root = document.querySelector(`[data-qa-widget-id="${widgetId}"]`);
    }

    this.el = document.createElement('div');
    this.el.classList.add('edit-qa-item');
  }

  componentDidMount() {
    // Append the element into the DOM on mount. We'll render
    // into the modal container element (see the HTML tab).
    this.root.insertBefore(this.el, this.root.firstChild);
  }

  componentWillUnmount() {
    // Remove the element from the DOM when we unmount
    this.root.removeChild(this.el);
  }

  render() {
    // Use a portal to render the children into the element
    return ReactDOM.createPortal(
      // Any valid React child: JSX, strings, arrays, etc.
      this.props.children,
      // A DOM element
      this.el,
    );
  }
}

EditComponent.propTypes = {
  type: PropTypes.string.isRequired,
  properties: PropTypes.oneOfType([
    PropTypes.shape({
      widgetId: PropTypes.string.isRequired,
      title: PropTypes.string.isRequired,
    }),
    PropTypes.shape({
      zoneName: PropTypes.string.isRequired,
    }),
  ]).isRequired,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node).isRequired,
    PropTypes.node.isRequired,
  ]).isRequired,
};

export default EditComponent;
