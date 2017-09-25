namespace QA.DotNetCore.Engine.Widgets.OnScreen
{
    public class OnScreenContext
    {
        public bool IsAuthorised { get; set; }
        public bool IsEditMode { get; set; }
        public OnScreenUser User {get; set;}

    }
}