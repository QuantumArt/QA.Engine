using Microsoft.Extensions.Options;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготавливаемой строителем, строится и обновляется в фоновом режиме.
    /// </summary>
    public class TrackableAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        readonly Func<IAbstractItemStorageBuilder> _builder;
        readonly QpSiteStructureSettings _settings;
        readonly Timer _job;

        public TrackableAbstractItemStorageProvider(
            Func<IAbstractItemStorageBuilder> builder,
            IOptions<QpSiteStructureSettings> settings)
        {
            _builder = builder;
            _settings = settings.Value;
            //_job = new Timer(OnUpdate, null, TimeSpan.FromTicks(0), _settings.PollPeriod);
        }

        public AbstractItemStorage Get()
        {
            throw new NotImplementedException();
        }

        protected void OnUpdate(object state)
        {

        }
    }
}
