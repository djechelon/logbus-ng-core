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

namespace It.Unina.Dis.Logbus.Design
{
    /// <summary>
    /// Marks the current class as a Custom Filter.
    /// The filter must be first registered, then it will be invokable through the usage of its unique tag
    /// </summary>
    [global::System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class CustomFilterAttribute : System.Attribute
    {
        public CustomFilterAttribute(string name)
        {
            this.Tag = name;
        }

        /// <summary>
        /// Unique tag for the filter. Identifies the current filter when referenced in the Custom filter instantiation
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// Human-readable description of the filter
        /// </summary>
        public string Description { get; set; }
    }
}
