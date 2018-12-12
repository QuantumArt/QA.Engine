import MutationSummary from 'mutation-summary';
import _ from 'lodash';
import { updateComponents } from 'actions/componentTreeActions';
import buildFlatList from 'utils/buildFlatList';

function MutationWatcher(store) {
  const mutationCallback = (summaries) => {
    const newComponentTree = buildFlatList();
    store.dispatch(updateComponents(newComponentTree));
  };

  /* eslint-disable no-new */
  new MutationSummary({
    callback: _.debounce(mutationCallback, 1000),
    queries: [{
      element: 'body',
    }],
  });
}

export default MutationWatcher;

