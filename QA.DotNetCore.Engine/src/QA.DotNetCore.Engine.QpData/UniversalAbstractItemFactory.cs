using QA.DotNetCore.Engine.QpData.Interfaces;

namespace QA.DotNetCore.Engine.QpData
{
    public class UniversalAbstractItemFactory : IAbstractItemFactory
    {
        public AbstractItem Create(string discriminator)
        {
            return new UniversalAbstractItem(discriminator);
        }
    }
}
