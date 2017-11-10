import { connect } from 'react-redux';
import { toggleComponent } from '../actions/componentTreeActions';
import ComponentTree from '../Components/ComponentTree';
import buildTree from '../utils/buildTree';

const mapStateToProps = state => ({
  components: buildTree(state.componentTree.components),
  selectedComponentId: state.componentTree.selectedComponentId,
  showAllZones: state.sidebar.showAllZones,
});

const mapDispatchToProps = dispatch => ({
  onToggleComponent: (id) => {
    dispatch(toggleComponent(id));
  },
});

const ComponentTreeContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentTree);

export default ComponentTreeContainer;
