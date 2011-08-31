using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// Provides methods for converting <see cref="DateTime"/> structures 
    /// to and from the equivalent <a href="http://www.w3.org/Protocols/rfc822/#z28">RFC 822</a> 
    /// string representation.
    /// </summary>
    public sealed class Rfc822DateTime
    {
        // For FxCop rule: StaticHolderTypesShouldNotHaveConstructors
        private Rfc822DateTime() {}

        //============================================================
        //  Private members
        //============================================================
        #region Private Members
        /// <summary>
        /// Private member to hold array of formats that RFC 822 date-time representations conform to.
        /// </summary>
        private static string[] _formats = new string[0];
        /// <summary>
        /// Private member to hold the DateTime format string for representing a DateTime in the RFC 822 format.
        /// </summary>
        private const string Format = "ddd, dd MMM yyyy HH:mm:ss K";
        #endregion

        //============================================================
        //  Public Properties
        //============================================================
        #region Rfc822DateTimeFormat
        /// <summary>
        /// Gets the custom format specifier that may be used to represent a <see cref="DateTime"/> in the RFC 822 format.
        /// </summary>
        /// <value>A <i>DateTime format string</i> that may be used to represent a <see cref="DateTime"/> in the RFC 822 format.</value>
        /// <remarks>
        /// <para>
        /// This method returns a string representation of a <see cref="DateTime"/> that utilizes the time zone 
        /// offset (local differential) to represent the offset from Greenwich mean time in hours and minutes. 
        /// The <see cref="Rfc822DateTimeFormat"/> is a valid date-time format string for use 
        /// in the <see cref="DateTime.ToString(String, IFormatProvider)"/> method.
        /// </para>
        /// <para>
        /// The <a href="http://www.w3.org/Protocols/rfc822/#z28">RFC 822</a> Date and Time specification 
        /// specifies that the year will be represented as a two-digit value, but the 
        /// <a href="http://www.rssboard.org/rss-profile#data-types-datetime">RSS Profile</a> recommends that 
        /// all date-time values should use a four-digit year. The <see cref="Rfc822DateTime"/> class 
        /// follows the RSS Profile recommendation when converting a <see cref="DateTime"/> to the equivalent 
        /// RFC 822 string representation.
        /// </para>
        /// </remarks>
        public static string Rfc822DateTimeFormat
        {
            get
            {
                return Format;
            }
        }
        #endregion

        #region Rfc822DateTimePatterns
        /// <summary>
        /// Gets an array of the expected formats for RFC 822 date-time string representations.
        /// </summary>
        /// <value>
        /// An array of the expected formats for RFC 822 date-time string representations 
        /// that may used in the <see cref="DateTime.TryParseExact(String, string[], IFormatProvider, DateTimeStyles, out DateTime)"/> method.
        /// </value>
        /// <remarks>
        /// The array of the expected formats that is returned assumes that the RFC 822 time zone 
        /// is represented as or converted to a local differential representation.
        /// </remarks>
        /// <seealso cref="ConvertZoneToLocalDifferential(String)"/>
        public static string[] Rfc822DateTimePatterns
        {
            get
            {
                if (_formats.Length > 0)
                {
                    return _formats;
                }
                else
                {
                    _formats = new string[67];

                    // with seconds

                    // two-digit day, four-digit year patterns
                    _formats[0] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'fffffff zzzz";
                    _formats[1] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'ffffff zzzz";
                    _formats[2] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'fffff zzzz";
                    _formats[3] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'ffff zzzz";
                    _formats[4] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'fff zzzz";
                    _formats[5] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'ff zzzz";
                    _formats[6] = "ddd',' dd MMM yyyy HH':'mm':'ss'.'f zzzz";
                    _formats[7] = "ddd',' dd MMM yyyy HH':'mm':'ss zzzz";

                    // two-digit day, two-digit year patterns
                    _formats[8] = "ddd',' dd MMM yy HH':'mm':'ss'.'fffffff zzzz";
                    _formats[9] = "ddd',' dd MMM yy HH':'mm':'ss'.'ffffff zzzz";
                    _formats[10] = "ddd',' dd MMM yy HH':'mm':'ss'.'fffff zzzz";
                    _formats[11] = "ddd',' dd MMM yy HH':'mm':'ss'.'ffff zzzz";
                    _formats[12] = "ddd',' dd MMM yy HH':'mm':'ss'.'fff zzzz";
                    _formats[13] = "ddd',' dd MMM yy HH':'mm':'ss'.'ff zzzz";
                    _formats[14] = "ddd',' dd MMM yy HH':'mm':'ss'.'f zzzz";
                    _formats[15] = "ddd',' dd MMM yy HH':'mm':'ss zzzz";

                    // one-digit day, four-digit year patterns
                    _formats[16] = "ddd',' d MMM yyyy HH':'mm':'ss'.'fffffff zzzz";
                    _formats[17] = "ddd',' d MMM yyyy HH':'mm':'ss'.'ffffff zzzz";
                    _formats[18] = "ddd',' d MMM yyyy HH':'mm':'ss'.'fffff zzzz";
                    _formats[19] = "ddd',' d MMM yyyy HH':'mm':'ss'.'ffff zzzz";
                    _formats[20] = "ddd',' d MMM yyyy HH':'mm':'ss'.'fff zzzz";
                    _formats[21] = "ddd',' d MMM yyyy HH':'mm':'ss'.'ff zzzz";
                    _formats[22] = "ddd',' d MMM yyyy HH':'mm':'ss'.'f zzzz";
                    _formats[23] = "ddd',' d MMM yyyy HH':'mm':'ss zzzz";

                    // two-digit day, two-digit year patterns
                    _formats[24] = "ddd',' d MMM yy HH':'mm':'ss'.'fffffff zzzz";
                    _formats[25] = "ddd',' d MMM yy HH':'mm':'ss'.'ffffff zzzz";
                    _formats[26] = "ddd',' d MMM yy HH':'mm':'ss'.'fffff zzzz";
                    _formats[27] = "ddd',' d MMM yy HH':'mm':'ss'.'ffff zzzz";
                    _formats[28] = "ddd',' d MMM yy HH':'mm':'ss'.'fff zzzz";
                    _formats[29] = "ddd',' d MMM yy HH':'mm':'ss'.'ff zzzz";
                    _formats[30] = "ddd',' d MMM yy HH':'mm':'ss'.'f zzzz";
                    _formats[31] = "ddd',' d MMM yy HH':'mm':'ss zzzz";

                    // without seconds

                    // two-digit day, four-digit year patterns
                    _formats[32] = "ddd',' dd MMM yyyy HH':'mm'.'fffffff zzzz";
                    _formats[33] = "ddd',' dd MMM yyyy HH':'mm'.'ffffff zzzz";
                    _formats[34] = "ddd',' dd MMM yyyy HH':'mm'.'fffff zzzz";
                    _formats[35] = "ddd',' dd MMM yyyy HH':'mm'.'ffff zzzz";
                    _formats[36] = "ddd',' dd MMM yyyy HH':'mm'.'fff zzzz";
                    _formats[37] = "ddd',' dd MMM yyyy HH':'mm'.'ff zzzz";
                    _formats[38] = "ddd',' dd MMM yyyy HH':'mm'.'f zzzz";
                    _formats[39] = "ddd',' dd MMM yyyy HH':'mm zzzz";

                    // two-digit day, two-digit year patterns
                    _formats[40] = "ddd',' dd MMM yy HH':'mm'.'fffffff zzzz";
                    _formats[41] = "ddd',' dd MMM yy HH':'mm'.'ffffff zzzz";
                    _formats[42] = "ddd',' dd MMM yy HH':'mm'.'fffff zzzz";
                    _formats[43] = "ddd',' dd MMM yy HH':'mm'.'ffff zzzz";
                    _formats[44] = "ddd',' dd MMM yy HH':'mm'.'fff zzzz";
                    _formats[45] = "ddd',' dd MMM yy HH':'mm'.'ff zzzz";
                    _formats[46] = "ddd',' dd MMM yy HH':'mm'.'f zzzz";
                    _formats[47] = "ddd',' dd MMM yy HH':'mm zzzz";

                    // one-digit day, four-digit year patterns
                    _formats[48] = "ddd',' d MMM yyyy HH':'mm'.'fffffff zzzz";
                    _formats[49] = "ddd',' d MMM yyyy HH':'mm'.'ffffff zzzz";
                    _formats[50] = "ddd',' d MMM yyyy HH':'mm'.'fffff zzzz";
                    _formats[51] = "ddd',' d MMM yyyy HH':'mm'.'ffff zzzz";
                    _formats[52] = "ddd',' d MMM yyyy HH':'mm'.'fff zzzz";
                    _formats[53] = "ddd',' d MMM yyyy HH':'mm'.'ff zzzz";
                    _formats[54] = "ddd',' d MMM yyyy HH':'mm'.'f zzzz";
                    _formats[55] = "ddd',' d MMM yyyy HH':'mm zzzz";

                    // two-digit day, two-digit year patterns
                    _formats[56] = "ddd',' d MMM yy HH':'mm'.'fffffff zzzz";
                    _formats[57] = "ddd',' d MMM yy HH':'mm'.'ffffff zzzz";
                    _formats[58] = "ddd',' d MMM yy HH':'mm'.'fffff zzzz";
                    _formats[59] = "ddd',' d MMM yy HH':'mm'.'ffff zzzz";
                    _formats[60] = "ddd',' d MMM yy HH':'mm'.'fff zzzz";
                    _formats[61] = "ddd',' d MMM yy HH':'mm'.'ff zzzz";
                    _formats[62] = "ddd',' d MMM yy HH':'mm'.'f zzzz";
                    _formats[63] = "ddd',' d MMM yy HH':'mm zzzz";

                    // Fall back patterns
                    _formats[64] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK"; // RoundtripDateTimePattern
                    _formats[65] = DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern;
                    _formats[66] = DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern;

                    return _formats;
                }
            }
        }
        #endregion

        //============================================================
        //  Public Methods
        //============================================================
        #region Parse(string s)
        /// <summary>
        /// Converts the specified string representation of a date and time to its <see cref="DateTime"/> equivalent.
        /// </summary>
        /// <param name="dateTime">A string containing a date and time to convert.</param>
        /// <returns>
        /// A <see cref="DateTime"/> equivalent to the date and time contained in <paramref name="dateTime"/>, 
        /// expressed as <i>Coordinated Universal Time (UTC)</i>.
        /// </returns>
        /// <remarks>
        /// The string <paramref name="dateTime"/> is parsed using formatting information in the <see cref="DateTimeFormatInfo.InvariantInfo"/> object.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="dateTime"/> is a <b>null</b> reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dateTime"/> is an empty string.</exception>
        /// <exception cref="FormatException"><paramref name="dateTime"/> does not contain a valid RFC 822 string representation of a date and time.</exception>
        public static DateTime Parse(string dateTime)
        {
            //------------------------------------------------------------
            //  Validate parameter
            //------------------------------------------------------------
            if (String.IsNullOrEmpty(dateTime))
            {
                throw new ArgumentNullException("dateTime");
            }

            DateTime result;
            if (Rfc822DateTime.TryParse(dateTime, out result))
            {
                return result;
            }
            else
            {
                throw new FormatException(String.Format(null, "{0} is not a valid RFC 822 string representation of a date and time.", dateTime));
            }
        }
        #endregion

        #region ConvertZoneToLocalDifferential(string s)
        /// <summary>
        /// Converts the time zone component of an RFC 822 date and time string representation to its local differential (time zone offset).
        /// </summary>
        /// <param name="dateTime">A string containing an RFC 822 date and time to convert.</param>
        /// <returns>A date and time string that uses local differential to describe the time zone equivalent to the date and time contained in <paramref name="dateTime"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dateTime"/> is a <b>null</b> reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dateTime"/> is an empty string.</exception>
        public static string ConvertZoneToLocalDifferential(string dateTime)
        {
            string zoneRepresentedAsLocalDifferential = String.Empty;

            //------------------------------------------------------------
            //  Validate parameter
            //------------------------------------------------------------
            if (String.IsNullOrEmpty(dateTime))
            {
                throw new ArgumentNullException("dateTime");
            }

            if (dateTime.EndsWith(" UT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" UT") + 1)), "+00:00");
            }
            else if (dateTime.EndsWith(" GMT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" GMT") + 1)), "+00:00");
            }
            else if (dateTime.EndsWith(" EST", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" EST") + 1)), "-05:00");
            }
            else if (dateTime.EndsWith(" EDT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" EDT") + 1)), "-04:00");
            }
            else if (dateTime.EndsWith(" CST", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" CST") + 1)), "-06:00");
            }
            else if (dateTime.EndsWith(" CDT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" CDT") + 1)), "-05:00");
            }
            else if (dateTime.EndsWith(" MST", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" MST") + 1)), "-07:00");
            }
            else if (dateTime.EndsWith(" MDT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" MDT") + 1)), "-06:00");
            }
            else if (dateTime.EndsWith(" PST", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" PST") + 1)), "-08:00");
            }
            else if (dateTime.EndsWith(" PDT", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" PDT") + 1)), "-07:00");
            }
            else if (dateTime.EndsWith(" Z", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" Z") + 1)), "+00:00");
            }
            else if (dateTime.EndsWith(" A", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" A") + 1)), "-01:00");
            }
            else if (dateTime.EndsWith(" M", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" M") + 1)), "-12:00");
            }
            else if (dateTime.EndsWith(" N", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" N") + 1)), "+01:00");
            }
            else if (dateTime.EndsWith(" Y", StringComparison.OrdinalIgnoreCase))
            {
                zoneRepresentedAsLocalDifferential = String.Concat(dateTime.Substring(0, (dateTime.LastIndexOf(" Y") + 1)), "+12:00");
            }
            else
            {
                zoneRepresentedAsLocalDifferential = dateTime;
            }

            return zoneRepresentedAsLocalDifferential;
        }
        #endregion

        #region ToString(DateTime utcDateTime)
        /// <summary>
        /// Converts the value of the specified <see cref="DateTime"/> object to its equivalent string representation.
        /// </summary>
        /// <param name="utcDateTime">The Coordinated Universal Time (UTC) <see cref="DateTime"/> to convert.</param>
        /// <returns>A RFC 822 string representation of the value of the <paramref name="utcDateTime"/>.</returns>
        /// <exception cref="ArgumentException">The specified <paramref name="utcDateTime"/> object does not represent a <see cref="DateTimeKind.Utc">Coordinated Universal Time (UTC)</see> value.</exception>
        public static string ToString(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("utcDateTime");
            }

            return utcDateTime.ToString(Rfc822DateTime.Rfc822DateTimeFormat, DateTimeFormatInfo.InvariantInfo);
        }
        #endregion

        #region TryParse(string s, out DateTime result)
        /// <summary>
        /// Converts the specified string representation of a date and time to its <see cref="DateTime"/> equivalent.
        /// </summary>
        /// <param name="dateTime">A string containing a date and time to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="DateTime"/> value equivalent to the date and time 
        /// contained in <paramref name="dateTime"/>, expressed as <i>Coordinated Universal Time (UTC)</i>, 
        /// if the conversion succeeded, or <see cref="DateTime.MinValue">MinValue</see> if the conversion failed. 
        /// The conversion fails if the s parameter is a <b>null</b> reference (Nothing in Visual Basic), 
        /// or does not contain a valid string representation of a date and time. 
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns><b>true</b> if the <paramref name="dateTime"/> parameter was converted successfully; otherwise, <b>false</b>.</returns>
        /// <remarks>
        /// The string <paramref name="dateTime"/> is parsed using formatting information in the <see cref="DateTimeFormatInfo.InvariantInfo"/> object. 
        /// </remarks>
        public static bool TryParse(string dateTime, out DateTime result)
        {
            //------------------------------------------------------------
            //  Attempt to convert string representation
            //------------------------------------------------------------
            bool wasConverted = false;
            result = DateTime.MinValue;

            if (!String.IsNullOrEmpty(dateTime))
            {
                DateTime parseResult;
                if (DateTime.TryParseExact(Rfc822DateTime.ConvertZoneToLocalDifferential(dateTime), Rfc822DateTime.Rfc822DateTimePatterns, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out parseResult))
                {
                    result = DateTime.SpecifyKind(parseResult, DateTimeKind.Utc);
                    wasConverted = true;
                }
            }

            return wasConverted;
        }
        #endregion
    }
}
