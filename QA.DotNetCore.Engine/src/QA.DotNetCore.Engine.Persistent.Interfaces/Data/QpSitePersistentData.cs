using System;
using System.Collections.Generic;
using System.Text;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    /// <summary>
    /// Настройки сайта в qp. (Таблица SITE)
    /// </summary>
    public class QpSitePersistentData
    {
        public bool UseAbsoluteUploadUrl { get; set; }

        public string UploadUrlPrefix { get; set; }

        public string UploadUrl { get; set; }

        public string Dns { get; set; }
    }
}
