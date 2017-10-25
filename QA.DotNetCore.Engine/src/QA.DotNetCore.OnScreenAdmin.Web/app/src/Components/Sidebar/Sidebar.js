import React, { Component } from "react";
import Drawer from "material-ui/Drawer";
import './sidebar.css'
import ComponentTree from '../../containers/componentTree'


class Sidebar extends Component {
  render() {
    return (
      <div className="sidebar">
        <Drawer type="permanent">
          <ComponentTree />
        </Drawer>
      </div>
    );
  }
}

export default Sidebar;
