import React from 'react';
import Drawer from 'material-ui/Drawer';
import './sidebar.css';
import ComponentTree from '../../containers/componentTree';


const Sidebar = () => (
  <div className="sidebar">
    <Drawer type="permanent">
      <ComponentTree />
    </Drawer>
  </div>
);

export default Sidebar;
