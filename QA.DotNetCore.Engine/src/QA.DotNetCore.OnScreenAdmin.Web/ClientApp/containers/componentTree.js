import { connect } from 'react-redux';
import { selectComponent } from '../actions/componentTreeActions';
import ComponentTree from '../Components/ComponentTree';
import buildTree from '../utils/buildTree';

const mapStateToProps = (state) => {
  const tree = buildTree(state.componentTree.components);

  console.log('mapStateToProps', state, tree);
  return {
    components: tree,
  };
};

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
