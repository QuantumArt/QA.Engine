namespace QA.DotNetCore.Engine.QpData.Replacements
{
    /// <summary>
    /// Правила построения урлов до файлов медиа-библиотеки QP
    /// </summary>
    public interface IQpUrlResolver
    {
        string UploadUrl(bool removeScheme = false);
        string UrlForImage(int contentId, string fieldName, bool removeScheme = false);
    }
}
