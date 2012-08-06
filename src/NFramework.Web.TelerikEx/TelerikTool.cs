using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSoft.NFramework.Web.TelerikEx
{
	public static partial class TelerikTool
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion
	}
}
