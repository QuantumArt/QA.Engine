namespace QA.DotNetCore.Engine.Widgets
{
    public sealed class WidgetZoneTypeQualifier
    {
        public static WidgetZoneType QualifyZone(string zoneName)
        {
            if (zoneName.StartsWith("Recursive"))
                return WidgetZoneType.Recursive;
            if (zoneName.StartsWith("Site") || zoneName.StartsWith("Global"))
                return WidgetZoneType.Global;
            return WidgetZoneType.Regular;
        }
    }
}
