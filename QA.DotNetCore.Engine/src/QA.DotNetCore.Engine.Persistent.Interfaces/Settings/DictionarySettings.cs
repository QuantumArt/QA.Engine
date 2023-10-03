namespace QA.DotNetCore.Engine.Persistent.Interfaces.Settings
{
    /// <summary>
    /// Настройки подключенния справочника
    /// </summary>
    public class DictionarySettings
    {
        public string Key { get; set; }
        public string NetName { get; set; }
        public string ParentIdFieldName { get; set; }
        public string AliasFieldName { get; set; }
        public string TitleFieldName { get; set; }
    }
}
