import { connect } from 'react-redux';
import { getComponentsListSelector } from 'selectors/componentTree';
import ComponentsOutlines from 'Components/ComponentsOutlines';
import { updateComponents } from 'actions/componentTreeActions';


const mapStateToProps = state => ({
  components: getComponentsListSelector(state),
});

const mapDispatchToProps = dispatch => ({
  updateComponents: components => dispatch(updateComponents(components)),
});

const ComponentsOutlinesContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentsOutlines);

export default ComponentsOutlinesContainer;
