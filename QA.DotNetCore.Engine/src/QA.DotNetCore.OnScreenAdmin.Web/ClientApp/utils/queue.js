export default class Queue {
  constructor() {
    this.value = [];
  }

  get length() {
    return this.value.length;
  }

  enqueue(val) {
    return this.value.push(val);
  }

  dequeue() {
    return this.value.shift();
  }

  peekLast() {
    const index = this.value.length - 1;
    if (index < 0) {
      return null;
    }

    return this.value[index];
  }

  peek() {
    return this.value[0];
  }

  contains(val) {
    return this.value.indexOf(val) !== -1;
  }

  toArray() {
    return this.value;
  }

  toString() {
    return this.value.toString();
  }
}
