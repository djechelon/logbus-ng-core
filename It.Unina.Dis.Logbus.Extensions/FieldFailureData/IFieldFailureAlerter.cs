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

using It.Unina.Dis.Logbus.Loggers;

namespace It.Unina.Dis.Logbus.FieldFailureData
{
    /// <summary>
    /// Used to submit alert logs into Logbus when anomalies are detected
    /// </summary>
    public interface IFieldFailureAlerter
        : ILog
    {
        /// <summary>
        /// Logs the event of a Computational Alert
        /// </summary>
        void LogCOA();

        /// <summary>
        /// Logs the event of an identified Computational Alert
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        void LogCOA(string id);

        /// <summary>
        /// Logs the event of a Entity Interaction Alert
        /// </summary>
        void LogEIA();

        /// <summary>
        /// Logs the event of an identified Entity Interaction Alert
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        void LogEIA(string id);

        /// <summary>
        /// Logs the event of a Resource Interaction Alert
        /// </summary>
        void LogRIA();

        /// <summary>
        /// Logs the event of an identified Resource Interaction Alert
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        void LogRIA(string id);
    }
}