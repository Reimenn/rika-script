namespace RikaScript.Exception
{
    public class ImportLibraryException : RikaScriptException
    {
        public ImportLibraryException(string info) : base("导入类库失败，因为" + info)
        {
        }
    }
}