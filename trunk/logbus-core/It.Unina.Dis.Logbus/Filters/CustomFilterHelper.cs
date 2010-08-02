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
using System.IO;
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
        private static CustomFilterHelper instance;

        static CustomFilterHelper()
        {
            instance = new CustomFilterHelper();
        }

        /// <summary>
        /// Returns singleton
        /// </summary>
        public static CustomFilterHelper Instance
        {
            get { return instance; }
        }
        #endregion

        private CustomFilterHelper()
        {
            registered_types = new Dictionary<string, string>();
            registered_descriptions = new Dictionary<string, string>();
        }

        private Dictionary<string, string> registered_types, registered_descriptions;

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
                object[] custom_attrs = t.GetCustomAttributes(typeof(It.Unina.Dis.Logbus.Design.CustomFilterAttribute), false);
                if (custom_attrs == null || custom_attrs.Length < 1) continue;
                CustomFilterAttribute attr = custom_attrs[0] as CustomFilterAttribute;

                try
                {
                    if (attr != null) RegisterCustomFilter(attr.Tag, typename, attr.Description);
                }
                catch (LogbusException)
                {
                    throw;
                }
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
            Type filter_type;
            try
            {
                filter_type = Type.GetType(typeName);

                if (!typeof(ICustomFilter).IsAssignableFrom(filter_type))
                {
                    LogbusException ex = new LogbusException("Given type does not implement ICustomFilter");
                    ex.Data.Add("typeName", typeName);
                    throw ex;
                }

                if (registered_types.ContainsKey(tag))
                    registered_types.Remove(tag);

                registered_types.Add(tag, typeName);

                registered_descriptions.Add(tag, description);
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

            if (!registered_types.ContainsKey(tag))
            {
                throw new LogbusException(string.Format("No filter registered for tag {0}", tag));
            }

            Type filter_type = Type.GetType(registered_types[tag]);
            ICustomFilter ret = Activator.CreateInstance(filter_type) as ICustomFilter;
            ret.Configuration = parameters;
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
                return (registered_types.ContainsKey(tag)) ? registered_types[tag] : null;
            }
        }

        /// <summary>
        /// Retrieves the description for a registered filter
        /// </summary>
        /// <param name="tag">Tag of the filter</param>
        /// <returns></returns>
        public string GetDescription(string tag)
        {
            return registered_descriptions[tag];
        }

        /// <summary>
        /// Sets the description for a filter
        /// </summary>
        /// <param name="tag">Tag of the filter</param>
        /// <param name="description">Description for the filter</param>
        private void SetDescription(string tag, string description)
        {
            if (registered_descriptions.ContainsKey(tag)) registered_descriptions.Remove(tag);
            registered_descriptions.Add(tag, description);
        }

        /// <summary>
        /// Returns the list of registered filters and their descriptions
        /// </summary>
        public IDictionary<string, string> AvailableFilters
        {
            get
            {
                return new Dictionary<string, string>(registered_descriptions);
            }
        }

        /// <summary>
        /// Returns the list of available custom filter keys
        /// </summary>
        /// <returns>String array containing all the keys</returns>
        public string[] GetAvailableCustomFilters()
        {
            string[] ret = new string[this.registered_types.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> kvp in this.registered_types)
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
            if (!registered_types.ContainsKey(key)) return null;
            return new FilterDescription()
            {
                id = key,
                description = this.registered_descriptions[key]
            };
        }
    }
}
