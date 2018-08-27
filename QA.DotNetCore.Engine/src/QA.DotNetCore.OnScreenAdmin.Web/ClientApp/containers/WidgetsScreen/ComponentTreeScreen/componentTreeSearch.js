import { connect } from 'react-redux';
import ComponentTreeSearch from 'Components/WidgetsScreen/ComponentTreeScreen/ComponentTreeSearch';
import { getSearchTextSelector } from 'selectors/componentTree';
import { changeSearchText } from 'actions/componentTreeActions';


const mapStateToProps = state => ({
  searchText: getSearchTextSelector(state),
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
