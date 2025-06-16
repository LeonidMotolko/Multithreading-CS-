using System;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter class name:");
        string className = Console.ReadLine();

        Console.WriteLine("Enter method name:");
        string methodName = Console.ReadLine();

        Console.WriteLine("Enter arguments (ex1, ex2...):");
        string argInput = Console.ReadLine();
        string[] argStrings = string.IsNullOrWhiteSpace(argInput) ? Array.Empty<string>() : argInput.Split(',');

        try
        {
            // Finding a type in the current build
            Type type = Type.GetType(className) ??
                AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == className);

            if (type == null)
                throw new Exception("Class not found.");

            // Creating an instance
            object instance = Activator.CreateInstance(type);

            // Finding the right method
            MethodInfo[] methods = type.GetMethods().Where(m => m.Name == methodName).ToArray();
            MethodInfo targetMethod = methods.FirstOrDefault(m => m.GetParameters().Length == argStrings.Length);

            if (targetMethod == null)
                throw new Exception("Method with this number of arguments not found.");

            // Conversion of arguments
            ParameterInfo[] paramInfos = targetMethod.GetParameters();
            object[] parsedArgs = new object[argStrings.Length];

            for (int i = 0; i < argStrings.Length; i++)
            {
                parsedArgs[i] = Convert.ChangeType(argStrings[i].Trim(), paramInfos[i].ParameterType);
            }

            // Metod call
            object result = targetMethod.Invoke(instance, parsedArgs);
            Console.WriteLine("Result: " + (result?.ToString() ?? "null"));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
