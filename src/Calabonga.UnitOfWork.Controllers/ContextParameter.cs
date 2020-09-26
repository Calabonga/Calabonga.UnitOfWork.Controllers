namespace Calabonga.UnitOfWork.Controllers
{
    /// <summary>
    /// Work parameters
    /// </summary>
    public class ContextParameter
    {
        public ContextParameter() { }

        public ContextParameter(string name, object value)
        {
            Name = name;
            Value = value;
            TypeName = value.GetType().FullName;
            AssemblyName = value.GetType().Assembly.GetName().FullName;
        }

        public string AssemblyName { get; }

        public string Name { get; }

        public string TypeName { get; }

        public object Value { get; }
    }
}