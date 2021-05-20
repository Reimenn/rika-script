namespace RikaScript.Methods
{
    public struct MethodName
    {
        public string Name;
        public int ArgNum;

        public MethodName(string name, int argNum)
        {
            Name = name;
            ArgNum = argNum;
        }

        public override string ToString()
        {
            return ToString(Name, ArgNum);
        }

        public static string ToString(string name, int argNum)
        {
            return name + "^" + argNum;
        }
    }
}