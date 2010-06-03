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
namespace It.Unina.Dis.Logbus.Filters
{
    /// <summary>
    /// Instantiates custom filters according to type and parameters
    /// </summary>
    /// <remarks>Singleton</remarks>
    internal sealed class CustomFilterFactory
    {
        #region Singleton control
        private static CustomFilterFactory instance;

        static CustomFilterFactory()
        {
            instance = new CustomFilterFactory();
        }

        /// <summary>
        /// Returns singleton
        /// </summary>
        public static CustomFilterFactory Instance
        {
            get { return instance; }
        }
        #endregion


        private Dictionary<string, string> registered_types;

        #region Constructor
        #endregion

        public void RegisterCustomFilter(string tag, string type)
        {
            //Would we check if we can activate the object?
            registered_types.Add(tag, type);
        }

        public IFilter BuildFilter(string tag, IEnumerable<FilterParameter> parameters)
        {
            /*
             * 1. Check if filter is available
             * 2. Construct it using parameters as.... what? Constructor argument? Interface methods?
             * */

            throw new System.NotImplementedException();
        }
    }
}
