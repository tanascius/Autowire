using System;
using System.Globalization;

namespace Autowire.Utils.Extensions
{
	/// <summary>Stellt Extensions für <see cref="string"/>s zur Verfügung</summary>
	public static class StringExtensions
	{
		/// <summary>Initializes static members of the <see cref="StringExtensions" /> class.</summary>
		static StringExtensions()
		{
			UiCulture = CultureInfo.CurrentCulture;
		}

		/// <summary>Gets or sets the userinterface culture.</summary>
		public static IFormatProvider UiCulture { get; set; }

		#region FormatUi()
		/// <summary>Replaces one or more format items in a specified string with the string representation of a specified object by using <see cref="UiCulture"/>.</summary>
		/// <param name="format">A composite format string</param>
		/// <param name="arg0">An object to format.</param>
		/// <returns>A copy of format in which any format items are replaced by the string representation of arg0.</returns>
		public static string FormatUi( this string format, object arg0 )
		{
			return string.Format( UiCulture, format, arg0 );
		}

		/// <summary>Replaces the format items in a specified string with the string representations of two specified objects by using <see cref="UiCulture"/>.</summary>
		/// <param name="format">A composite format string</param>
		/// <param name="arg0">The first object to format.</param>
		/// <param name="arg1">The second object to format.</param>
		/// <returns>A copy of format in which format items have been replaced by the string equivalents of arg0 and arg1.</returns>
		public static string FormatUi( this string format, object arg0, object arg1 )
		{
			return string.Format( UiCulture, format, arg0, arg1 );
		}

		/// <summary>Replaces the format items in a specified string with the string representations of three specified objects by using <see cref="UiCulture"/>.</summary>
		/// <param name="format">A composite format string</param>
		/// <param name="arg0">The first object to format.</param>
		/// <param name="arg1">The second object to format.</param>
		/// <param name="arg2">The third object to format.</param>
		/// <returns>A copy of format in which the format items have been replaced by the string representations of arg0, arg1, and arg2.</returns>
		public static string FormatUi( this string format, object arg0, object arg1, object arg2 )
		{
			return string.Format( UiCulture, format, arg0, arg1, arg2 );
		}

		/*
		/// <summary>Replaces the format item in a specified string with the string representation of a corresponding object in a specified array by using <see cref="UiCulture"/>.</summary>
		/// <param name="format">A composite format string</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
		public static string FormatUi( this string format, params object[] args )
		{
			return string.Format( UiCulture, format, args );
		}
		*/
		#endregion
	}
}
