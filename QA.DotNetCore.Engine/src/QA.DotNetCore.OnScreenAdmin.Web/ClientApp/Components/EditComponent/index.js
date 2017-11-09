import { Component } from 'react';
import PropTypes from 'prop-types';
import ReactDOM from 'react-dom';

class EditComponent extends Component {
  constructor(props) {
    super(props);
    const { onScreenId } = props;

    console.log(onScreenId);
    this.root = document.querySelector(`[data-qa-component-on-screen-id="${onScreenId}"]`);

    this.el = document.createElement('div');
  }

  componentDidMount() {
    // Append the element into the DOM on mount. We'll render
    // into the modal container element (see the HTML tab).
    this.root.insertBefore(this.el, this.root.firstChild);
    this.el.classList.add('edit-qa-item', this.props.type);
  }

  componentWillUnmount() {
    // Remove the element from the DOM when we unmount
    this.root.removeChild(this.el);
    this.el.classList.remove('edit-qa-item', this.props.type);
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
  onScreenId: PropTypes.string.isRequired,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node).isRequired,
    PropTypes.node.isRequired,
  ]).isRequired,
};

export default EditComponent;
