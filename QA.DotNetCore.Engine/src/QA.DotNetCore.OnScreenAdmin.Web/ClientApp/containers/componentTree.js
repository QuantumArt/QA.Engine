import { connect } from 'react-redux';
import { selectComponent } from '../actions/componentTreeActions';
import ComponentTree from '../Components/ComponentTree';

const mapStateToProps = (state) => {
  console.log('mapStateToProps', state);
  return {
    components: state.componentTree.components,
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
