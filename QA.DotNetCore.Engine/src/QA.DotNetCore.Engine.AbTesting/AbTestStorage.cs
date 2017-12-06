using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.AbTesting
{
    public class AbTestStorage
    {
        public AbTestPersistentData GetTestById(int testId)
        {
            return new AbTestPersistentData { Percentage = new int[2] { 50, 50 } };
        }

        public bool HasContainersForPage(string pagePath)
        {
            // надо ли учитывать хост? или только path 
            // надо как-то таргетирование еще учитывать?.. регион например
            return true;
        }

        public AbTestWithContainers[] GetTestsWithContainersForPage(string pagePath)
        {
            throw new NotImplementedException();
        }
    }
}
