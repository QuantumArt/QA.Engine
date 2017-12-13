import React from 'react';
import PropTypes from 'prop-types';
import SearchToolBar from '../../SearchToolBar';

const ComponentTreeSearch = ({ searchText, changeSearchText }) => (
  <SearchToolBar
    searchText={searchText}
    changeSearchText={changeSearchText}
  />
);

ComponentTreeSearch.propTypes = {
  searchText: PropTypes.string.isRequired,
  changeSearchText: PropTypes.func.isRequired,
};

export default ComponentTreeSearch;
