/*
 *                  Logbus-ng project
 *    ©2010 Logbus Reasearch Team - Some rights reserved
 *
 *  Created by:
 *      Vittorio Alfieri - vitty85@users.sourceforge.net
 *      Antonio Anzivino - djechelon@users.sourceforge.net
 *
 *  Based on the research project "Logbus" by
 *
 *  Dipartimento di Informatica e Sistemistica
 *  University of Naples "Federico II"
 *  via Claudio, 21
 *  80121 Naples, Italy
 *
 *  Software is distributed under Microsoft Reciprocal License
 *  Documentation under Creative Commons 3.0 BY-SA License
*/

using System.Collections.Generic;
using System;
using System.Reflection;
using It.Unina.Dis.Logbus.Design;
using It.Unina.Dis.Logbus.RemoteLogbus;
namespace It.Unina.Dis.Logbus.Filters
{
    /// <summary>
    /// Instantiates custom filters according to type and parameters
    /// </summary>
    /// <remarks>Singleton</remarks>
    public sealed class CustomFilterHelper
    {
        #region Singleton control
        private static readonly CustomFilterHelper _instance;

        static CustomFilterHelper()
        {
            _instance = new CustomFilterHelper();
        }

        /// <summary>
        /// Returns singleton
        /// </summary>
        public static CustomFilterHelper Instance
        {
            get { return _instance; }
        }
        #endregion

        private CustomFilterHelper()
        {
            _registeredTypes = new Dictionary<string, string>();
            _registeredDescriptions = new Dictionary<string, string>();

            //Load builtin filters
            ScanAssemblyAndRegister(GetType().Assembly);
        }

        private readonly Dictionary<string, string> _registeredTypes, _registeredDescriptions;

        /// <summary>
        /// Scans an assembly for user-defined filters and registers all of them
        /// </summary>
        /// <param name="assemblyToScan">Assembly that must be scanned for filters</param>
        public void ScanAssemblyAndRegister(Assembly assemblyToScan)
        {
            if (assemblyToScan == null) throw new ArgumentNullException("assemblyToScan");
            foreach (Type t in assemblyToScan.GetTypes())
            {
                string typename = t.AssemblyQualifiedName;
                object[] customAttrs = t.GetCustomAttributes(typeof(CustomFilterAttribute), false);
                if (customAttrs.Length < 1) continue;
                CustomFilterAttribute attr = customAttrs[0] as CustomFilterAttribute;


                if (attr != null) RegisterCustomFilter(attr.Tag, typename, attr.Description);
            }
        }

        /// <summary>
        /// Registers a custom filter that is decorated with CustomFilterAttribute
        /// </summary>
        /// <param name="t">Type of the filter to register</param>
        public void RegisterCustomFilter(Type t)
        {
            if (t == null) throw new ArgumentNullException("t");
            object[] customAttrs = t.GetCustomAttributes(typeof(CustomFilterAttribute), false);

            if (customAttrs.Length < 1)
                throw new ArgumentException("Given type is not decorated with CustomFilterAttribute");
            CustomFilterAttribute attr = customAttrs[0] as CustomFilterAttribute;

            if (attr != null)
            {
                string typeName = t.AssemblyQualifiedName;
                if (!typeof(ICustomFilter).IsAssignableFrom(t))
                {
                    LogbusException ex = new LogbusException("Given type does not implement ICustomFilter");
                    ex.Data.Add("typeName", t);
                    throw ex;
                }


                if (_registeredTypes.ContainsKey(attr.Tag))
                    _registeredTypes.Remove(attr.Tag);

                _registeredTypes.Add(attr.Tag, typeName);
                _registeredDescriptions.Add(attr.Tag, attr.Description);
            }
        }

        /// <summary>
        /// Registers a custom filter for the given tag
        /// </summary>
        /// <param name="tag">Unique ID of filter</param>
        /// <param name="typeName">Type for filter</param>
        /// <param name="description">Human-readable description for filter</param>
        public void RegisterCustomFilter(string tag, string typeName, string description)
        {
            if (string.IsNullOrEmpty(tag)) throw new ArgumentNullException("tag");
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentNullException("typeName");

            //Examine type
            try
            {
                Type filterType = Type.GetType(typeName);

                if (!typeof(ICustomFilter).IsAssignableFrom(filterType))
                {
                    LogbusException ex = new LogbusException("Given type does not implement ICustomFilter");
                    ex.Data.Add("typeName", typeName);
                    throw ex;
                }

                if (_registeredTypes.ContainsKey(tag))
                    _registeredTypes.Remove(tag);

                _registeredTypes.Add(tag, typeName);
                _registeredDescriptions.Add(tag, description);
            }
            catch (LogbusException) { throw; }
            catch (Exception ex) //Usually TypeLoadException
            {
                throw new LogbusException("Unable to load type for custom filter", ex);
            }
        }

        /// <summary>
        /// Constructs a filter for the given tag and with the given properties
        /// </summary>
        /// <param name="tag">Unique tag of filter</param>
        /// <param name="parameters">Filter-specific parameters</param>
        /// <returns>A new instance of the requested custom filter</returns>
        public IFilter BuildFilter(string tag, IEnumerable<FilterParameter> parameters)
        {
            if (string.IsNullOrEmpty(tag)) throw new ArgumentNullException("tag");

            if (!_registeredTypes.ContainsKey(tag))
            {
                throw new LogbusException(string.Format("No filter registered for tag {0}", tag));
            }

            Type filterType = Type.GetType(_registeredTypes[tag]);
            ICustomFilter ret = Activator.CreateInstance(filterType) as ICustomFilter;
            if (parameters != null) ret.Configuration = parameters;
            return ret;
        }

        /// <summary>
        /// Constructs a filter for the given tag and with the given properties
        /// </summary>
        /// <param name="tag">Unique tag of filter</param>
        /// <param name="parameters">Filter-specific parameters</param>
        public IFilter this[string tag, IEnumerable<FilterParameter> parameters]
        {
            get
            {
                return BuildFilter(tag, parameters);
            }
        }

        /// <summary>
        /// Gets the type name associated to the given tag
        /// </summary>
        /// <param name="tag">Unique tag of the custom filter</param>
        /// <returns></returns>
        public string this[string tag]
        {
            get
            {
                return (_registeredTypes.ContainsKey(tag)) ? _registeredTypes[tag] : null;
            }
        }

        /// <summary>
        /// Retrieves the description for a registered filter
        /// </summary>
        /// <param name="tag">Tag of the filter</param>
        /// <returns></returns>
        public string GetDescription(string tag)
        {
            return _registeredDescriptions[tag];
        }

        /// <summary>
        /// Returns the list of registered filters and their descriptions
        /// </summary>
        public IDictionary<string, string> AvailableFilters
        {
            get
            {
                return new Dictionary<string, string>(_registeredDescriptions);
            }
        }

        /// <summary>
        /// Returns the list of available custom filter keys
        /// </summary>
        /// <returns>String array containing all the keys</returns>
        public string[] GetAvailableCustomFilters()
        {
            string[] ret = new string[_registeredTypes.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> kvp in _registeredTypes)
            {
                ret[i] = kvp.Key;
                i++;
            }
            return ret;
        }

        /// <summary>
        /// Describes a registered filter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public FilterDescription DescribeFilter(string key)
        {
            if (!_registeredTypes.ContainsKey(key)) return null;
            return new FilterDescription
                       {
                id = key,
                description = _registeredDescriptions[key]
            };
        }
    }
}
