using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    /// <summary>
    /// Провайдер возможных значений по ключу (или ключам) таргетирования
    /// </summary>
    public interface ITargetingPossibleValuesProvider
    {
        IDictionary<string, IEnumerable<object>> GetPossibleValues();
    }
}
