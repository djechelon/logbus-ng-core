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
    /// Implement this interface when you need to build custom filters.
    /// ICustomFilter implements the IFilter interface plus a configuration property
    /// </summary>
    public interface ICustomFilter
        : IFilter
    {
        /// <summary>
        /// Sets configuration parameters for the filter.
        /// Their syntax and semantics completely depend on the design contract
        /// </summary>
        /// <remarks>In practice, it will be an array</remarks>
        IEnumerable<FilterParameter> Configuration { set; }
    }
}