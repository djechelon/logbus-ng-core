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
        }

        private Dictionary<string, string> registered_types;

        /// <summary>
        /// Scans an assembly for user-defined filters and registers all of them
        /// </summary>
        /// <param name="to_scan"></param>
        public void ScanAssemblyAndRegister(Assembly to_scan)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers a custom filter for the given tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="type"></param>
        public void RegisterCustomFilter(string tag, string typeName)
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
        /// Gets or sets the type name associated to the given tag
        /// </summary>
        /// <param name="tag">Unique tag of the custom filter</param>
        /// <returns></returns>
        public string this[string tag]
        {
            get
            {
                return (registered_types.ContainsKey(tag)) ? registered_types[tag] : null;
            }
            set
            {
                RegisterCustomFilter(tag, value);
            }
        }

       
    }
}
