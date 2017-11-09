import { connect } from 'react-redux';
import { selectComponent } from '../actions/componentTreeActions';
import ComponentTree from '../Components/ComponentTree';
import buildTree from '../utils/buildTree';

const mapStateToProps = state => ({
  components: buildTree(state.componentTree.components),
});

const mapDispatchToProps = dispatch => ({
  onSelectComponent: (id) => {
    dispatch(selectComponent(id));
  },
});

const ComponentTreeContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentTree);

export default ComponentTreeContainer;
