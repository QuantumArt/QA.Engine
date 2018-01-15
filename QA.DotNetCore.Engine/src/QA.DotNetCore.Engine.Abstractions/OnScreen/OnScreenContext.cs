namespace QA.DotNetCore.Engine.Abstractions.OnScreen
{
    public class OnScreenContext
    {
        /// <summary>
        /// Доступен ли OnScreen
        /// </summary>
        public bool Enabled { get { return Features != OnScreenFeatures.None; } }
        /// <summary>
        /// Доступные фичи OnScreen
        /// </summary>
        public OnScreenFeatures Features { get; set; }
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public OnScreenUser User { get; set; }
        /// <summary>
        /// Проверка, доступна ли фича
        /// </summary>
        /// <param name="feature">фича OnScreen</param>
        /// <returns></returns>
        public bool HasFeature(OnScreenFeatures feature)
        {
            return (Features & feature) > 0;
        }
    }
}