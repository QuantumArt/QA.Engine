using QA.DotNetCore.Engine.Abstractions;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Widgets
{
    public class WidgetContextScope 
    {
        Stack<IAbstractItem> _stack = new Stack<IAbstractItem>();
        
        public IAbstractItem Get()
        {
            if (_stack.Count > 0)
            {
                return _stack.Peek();
            }

            return null;
        }

        public IAbstractItem GetAndRemove()
        {
            if (_stack.Count > 0)
            {
                return _stack.Pop();
            }

            return null;
        }

        public void CreateScope(IAbstractItem item)
        {
            _stack.Push(item);
        }
    }
}
