using System;

namespace QA.DotNetCore.Engine.QpData.Targeting
{
    public class CultureFilter : StringValueTargetingFilter
    {
        public const string Key = "culture";

        string _currentCultureCode;

        public CultureFilter(string currentCultureCode)
        {
            _currentCultureCode = currentCultureCode;
        }

        protected override string FilterValue => _currentCultureCode;

        protected override string TargetingKey => Key;
    }
}
