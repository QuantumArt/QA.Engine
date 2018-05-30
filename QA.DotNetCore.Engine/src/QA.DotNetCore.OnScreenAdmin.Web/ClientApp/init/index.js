import { initHostId } from '../qpInteraction/utils';

const init = () => {
  const fakeGear = document.getElementById('fakeGear');
  if (fakeGear) {
    fakeGear.style.opacity = 0;
    fakeGear.style.visibility = 'hidden';
  }
  initHostId();
};

export default init;
