using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Атрибут-маркер, уведомляющий о поле, являющимся связью many-to-many
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Obsolete]
    public class LoadManyToManyRelationsAttribute : Attribute
    {

    }
}
