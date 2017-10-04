import {Component } from "react";
import ReactDOM from 'react-dom';


class EditComponent extends Component {
  constructor(props){
    super(props);
    this.root = document.querySelector(`[data-qa-component-on-screen-id="${props.onScreenId}"]`);
    this.el = document.createElement('span');
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

export default EditComponent;