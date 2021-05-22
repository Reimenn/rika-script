namespace RikaScript.Methods
{
    public interface IMethod
    {
        /// <summary>
        /// 调用一个方法，返回是否存在返回值
        /// </summary>
        /// <param name="args">方法参数</param>
        /// <param name="res">返回值</param>
        /// <returns>是否存在返回值</returns>
        bool Call(object[] args, out object res);
    }
}