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

using System;

namespace It.Unina.Dis.Logbus.FieldFailureData
{
    /// <summary>
    /// Provides FFDA-specific logging services using the underlying Logbus-ng infrastructure
    /// </summary>
    /// <remarks>
    /// FFDA messages have the following costraints:
    /// <list>
    /// <item>Facility defaults to Local0</item>
    /// <item>Severity equals to Info or Alert</item>
    /// <item>MessageID equals to <c>FFDA</c></item>
    /// <item>Text matches regular expression <c>^(SST|SEN|RIS|RIE|EIS|EIE|COA|CMP)[-]?</c></item>
    /// </list>
    /// The COA message is a special message that is triggered <b>only</b> by an external entity that detects a failure in a monitored entity
    /// Use the CMP message to report about self-detect failures
    /// </remarks>
    public interface IFieldFailureDataLogger
        : IDisposable
    {
        /// <summary>
        /// Gets or sets an object from which to obtain the flow's ID
        /// </summary>
        /// <remarks>By default, the flow ID coincides with the current thread's hash code</remarks>
        object Flow { get; set; }

        /// <summary>
        /// Logs the event of Service Start
        /// </summary>
        void LogSST();

        /// <summary>
        /// Logs the event of an identified Service Start
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        void LogSST(string id);

        /// <summary>
        /// Logs the event of a Service End
        /// </summary>
        void LogSEN();

        /// <summary>
        /// Logs the event of an identified Service End
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        void LogSEN(string id);

        /// <summary>
        /// Logs the event of an Entity Interaction Start
        /// </summary>
        void LogEIS();

        /// <summary>
        /// Logs the event of an identified Entity Interaction Start
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        void LogEIS(string id);

        /// <summary>
        /// Logs the event of an Entity Interaction End
        /// </summary>
        void LogEIE();

        /// <summary>
        /// Logs the event of an identified Entity Interaction End
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        void LogEIE(string id);

        /// <summary>
        /// Logs the event of a Resource Interaction Start
        /// </summary>
        void LogRIS();

        /// <summary>
        /// Logs the event of an identified Resource Interaction Start
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        void LogRIS(string id);

        /// <summary>
        /// Logs the event of a Resource Interaction End
        /// </summary>
        void LogRIE();

        /// <summary>
        /// Logs the event of an identified Resource Interaction End
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        void LogRIE(string id);

        /// <summary>
        /// Logs the event of a Complaint
        /// </summary>
        void LogCMP();

        /// <summary>
        /// Logs the event of an identified Complaint
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        /// <remarks>Never use Exception.Message as id: if you need to log such message, use another logger!</remarks>
        void LogCMP(string id);
    }
}