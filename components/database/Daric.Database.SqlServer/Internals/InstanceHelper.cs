namespace Daric.Database.SqlServer.Internals
{
    internal class InstanceHelper
    {
        public static TResult? CreateInstance<TResult>(IServiceProvider serviceProvider, params object[] args)
           where TResult : class
        {
            var constructor = typeof(TResult).GetConstructors().FirstOrDefault();
            var constructorParameters = constructor?.GetParameters();
            var constructorParameterCount = constructorParameters?.Length;

            var parameterValues = new List<object>();
            parameterValues.AddRange(args);

            if (constructorParameterCount is not null && constructorParameterCount > args.Length)
            {
                for (var i = args.Length; i < constructorParameterCount; i++)
                {
                    var parameterValue = serviceProvider.GetService(constructorParameters![i].ParameterType);
                    if (parameterValue is not null)
                        parameterValues.Add(parameterValue);
                }
            }

            return Activator.CreateInstance(typeof(TResult), [.. parameterValues]) as TResult;
        }
    }
}
