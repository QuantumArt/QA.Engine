using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.PageModel;

namespace Common.Widgets
{
    public class WidgetContextScope 
    {
        Stack<AbstractItem> _stack = new Stack<AbstractItem>();
        
        public AbstractItem Get()
        {
            if (_stack.Count > 0)
            {
                return _stack.Peek();
            }

            return null;
        }

        public AbstractItem GetAndRemove()
        {
            if (_stack.Count > 0)
            {
                return _stack.Pop();
            }

            return null;
        }

        public void CreateScope(AbstractItem item)
        {
            _stack.Push(item);
        }
    }
}
