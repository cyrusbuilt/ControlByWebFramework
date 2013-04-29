using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
	/// <summary>
	/// Humidity history readings.
	/// </summary>
	public enum HumidityReadings
	{
		/// <summary>
		/// The highest reading today.
		/// </summary>
		HighToday,

		/// <summary>
		/// The highest reading for yesterday.
		/// </summary>
		HighYesterday,

		/// <summary>
		/// The lowest reading today.
		/// </summary>
		LowToday,

		/// <summary>
		/// The lowest reading for yesterday.
		/// </summary>
		LowYesterday,

        /// <summary>
        /// No history.
        /// </summary>
        None
	}
}

