using System;

namespace MultiTouch.Behaviors.Helpers
{
    /// <summary>
    /// Infrastructure helper class needed for initialising behaviors when deploying different assemplies to iOS devices
    /// </summary>
    internal static class Infrastructure
	{
		private static DateTime _initDate;
		public static void Init()
		{
			_initDate = DateTime.UtcNow;
		}
	}
}

