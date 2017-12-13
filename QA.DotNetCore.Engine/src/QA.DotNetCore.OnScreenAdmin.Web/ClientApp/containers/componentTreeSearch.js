import { connect } from 'react-redux';
import ComponentTreeSearch from '../Components/ComponentTreeSearch';
import { getSearchText } from '../selectors/componentTree';
import { changeSearchText } from '../actions/componentTreeActions';

const mapStateToProps = state => ({
  searchText: getSearchText(state),
});

const mapDispatchToProps = dispatch => ({
  changeSearchText: (event) => {
    dispatch(changeSearchText(event.target.value));
  },
});

const ComponentTreeSearchContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentTreeSearch);

export default ComponentTreeSearchContainer;
