export default function checkIsIframe() {
  try {
    return window.self !== window.top;
  } catch (e) {
    return true;
  }
}
