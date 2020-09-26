using System;
using System.Collections.Generic;
using System.Linq;
using Calabonga.Microservices.Core.Exceptions;
using Newtonsoft.Json;

namespace Calabonga.UnitOfWork.Controllers.Helpers
{
    /// <summary>
    /// Parameters container adds to class additional  to held any parameters 
    /// </summary>
    public abstract class ParamsProperty
    {
        #region Available parameters

        public static string ParameterDate = "Date";
        public static string ParameterDateTime = "DateTime";
        public static string ParameterDateFrom = "DateFrom";
        public static string ParameterDateTo = "DateTo";

        public static string ParameterTotal = "Total";
        public static string ParameterCount = "Count";

        public static string ParameterId = "Identifier";

        public static string ParameterMessage = "Message";
        public static string ParameterItems = "Items";

        public static string ParameterUserName = "UserName";
        public static string ParameterPonyIdentifier = "PonyIdentifier";

        #endregion

        /// <summary>
        /// Serialized to string parameters for work
        /// </summary>
        public List<ContextParameter> Parameters { get; private set; }

        /// <summary>
        /// Adds a bunch of parameters for current work
        /// </summary>
        /// <param name="parameter"></param>
        [Obsolete("This method will be removed in future. Please use method AddOrUpdateParameter")]
        public void AddParameter(ContextParameter parameter)
        {
            if (Parameters == null)
            {
                Parameters = new List<ContextParameter>();
            }
            Parameters.Add(parameter);
        }

        /// <summary>
        /// Adds or updates new context instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        protected void AddOrUpdateParameter<T>(string keyName, T value)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentNullException(nameof(keyName));
            }
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (Parameters == null)
            {
                Parameters = new List<ContextParameter>();
            }

            var instance = GetParamByName<T>(keyName);
            if (instance == null)
            {
                Parameters.Add(new ContextParameter(keyName, value));
            }
            else
            {
                var removeItem = Parameters.SingleOrDefault(x => x.Name == keyName);
                if (removeItem == null)
                {

                    Parameters.Add(new ContextParameter(keyName, value));
                }
                else
                {
                    Parameters.Remove(removeItem);
                    Parameters.Add(new ContextParameter(keyName, value));
                }
            }
        }

        /// <summary>
        /// Returns parameters for current task work
        /// </summary>
        /// <returns></returns>
        public T GetParamByName<T>(string name)
        {
            if (Parameters == null)
            {
                return default(T);
            }

            var parameter = Parameters.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
            if (parameter == null)
            {
                return default(T);
            }
            var typeName = $"{parameter.TypeName}, {parameter.AssemblyName}";

            var type = Type.GetType(typeName);
            if (type == null)
            {
                return (T)parameter.Value;
            }
            if (type.IsPrimitive || type.IsClass)
            {
                if (TypeHelper.CanChangeType(parameter.Value, type))
                {
                    return (T)Convert.ChangeType(parameter.Value, type);
                }
                return (T)parameter.Value;
            }

            switch (type.Name)
            {
                case "Decimal":
                    return (T)Convert.ChangeType(parameter.Value, type);

                case "DateTime":
                    return (T)Convert.ChangeType(parameter.Value, type);

                case "String":
                    var resultString = parameter.Value.ToString();
                    return (T)Convert.ChangeType(resultString, type);

                case "Guid":
                    var resultGuid = Guid.Parse(parameter.Value.ToString());
                    return (T)Convert.ChangeType(resultGuid, type);

                default:
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(parameter.Value.ToString());
                    }
                    catch
                    {
                        throw new MicroserviceArgumentNullException();
                    }
            }
        }
    }
}
