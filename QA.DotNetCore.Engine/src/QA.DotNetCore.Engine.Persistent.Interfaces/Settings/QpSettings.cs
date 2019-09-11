namespace QA.DotNetCore.Engine.Persistent.Interfaces.Settings
{
    /// <summary>
    /// Настройки взаимодействия с QP
    /// </summary>
    public class QpSettings
    {
        /// <summary>
        /// Строка подключения к базе QP
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Тип базы QP: MSSQL/PG
        /// </summary>
        public string DatabaseType { get; set; }
        /// <summary>
        /// Id сайта в QP
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// Режим работы c QP
        /// </summary>
        public bool IsStage { get; set; }
        /// <summary>
        /// Кастомер код в QP
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// Урл конфигурационного сервиса QP (может служить источником подключения к базе QP)
        /// </summary>
        public string ConfigurationServiceUrl { get; set; }
        /// <summary>
        /// Токен конфигурационного сервиса QP
        /// </summary>
        public string ConfigurationServiceToken { get; set; }
    }
}
