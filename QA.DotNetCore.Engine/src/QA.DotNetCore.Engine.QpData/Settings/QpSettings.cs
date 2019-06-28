namespace QA.DotNetCore.Engine.QpData.Settings
{
    /// <summary>
    /// Настройки взаимодействия с QP
    /// </summary>
    public class QpSettings
    {
        public int SiteId { get; set; }

        public bool IsStage { get; set; }
        public string CustomerCode { get; set; }
        public string ConfigurationServiceUrl { get; set; }
        public string ConfigurationServiceToken { get; set; }
    }
}
