using System;

namespace MultiTouch.Behaviors
{
	public static class Infrastructure
	{
		private static DateTime initDate;
		public static void Init()
		{
			initDate = DateTime.UtcNow;
		}
	}
}

