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
using System.Text;
namespace It.Unina.Dis.Logbus.Utils
{
    internal class Randomizer
    {
        private Randomizer() { }

        /// <summary>
        /// Returns a random hexadecimal string (uppercase)
        /// </summary>
        /// <param name="length">Desired length</param>
        /// <returns></returns>
        public static string RandomHexString(int length)
        {
            char[] ret = new char[length];
            char[] CHARS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                ret[i] = CHARS[rnd.Next(0, 15)];
            }

            return new string(ret);
        }

        /// <summary>
        /// Returns a random string (upper case)
        /// </summary>
        /// <param name="length">Desired length</param>
        /// <returns></returns>
        public static string RandomAlphanumericString(int length)
        {
            char[] ret = new char[length];
            char[] CHARS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
                           'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                ret[i] = CHARS[rnd.Next(0, 15)];
            }

            return new string(ret);
        }

        /// <summary>
        /// Returns a random string (upper case)
        /// </summary>
        /// <param name="size">Desired length</param>
        /// <returns></returns>
        public static string RandomAlphabeticalString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

    }

}
