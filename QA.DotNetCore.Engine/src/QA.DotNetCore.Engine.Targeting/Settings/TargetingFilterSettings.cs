namespace QA.DotNetCore.Engine.Targeting.Settings
{
    public class TargetingFilterSettings
    {
        /// <summary>
        /// Список названий подключаемых сборок с реализацией <typeparamref name="ITargetingRegistration"/> 
        /// </summary>
        public string[] TargetingLibraries { get; set; }
    }
}
