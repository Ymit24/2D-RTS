
using System;
using System.Reflection;

namespace Game.Net
{
    class BindChanger : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            // Define the new type to bind to
            Type typeToDeserialize = null;
            // Get the assembly names
            string currentAssembly = Assembly.GetExecutingAssembly().FullName;
            // Create the new type and return it
            typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, currentAssembly));
            return typeToDeserialize;
        }
    }
}