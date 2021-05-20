namespace RikaScript.Methods
{
    public interface IMethod
    {
        bool Call(object[] args, out object res);
    }
}