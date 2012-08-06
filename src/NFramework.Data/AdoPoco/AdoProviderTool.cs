using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NSoft.NFramework.Data.AdoPoco {
    /// <summary>
    /// <see cref="IAdoProvider"/>에 대한 Extension Method 입니다.
    /// </summary>
    public static class AdoProviderTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        // Helper to handle named parameters from object properties
        internal static readonly Regex RxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);

        internal static readonly Regex RxParamsPrefix = new Regex(@"((?<!@)@\w+", RegexOptions.Compiled);

        public static string ProcessParams(string sql, object[] args_src, IList<object> args_dest) {
            return
                RxParams.Replace(sql,
                                 m => {
                                     var param = m.Value.Substring(1);

                                     object arg_val;

                                     int paramIndex;
                                     if(int.TryParse(param, out paramIndex)) {
                                         // Numbered parameter
                                         if(paramIndex < 0 || paramIndex >= args_src.Length)
                                             throw new ArgumentOutOfRangeException(
                                                 string.Format(
                                                     "Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)",
                                                     paramIndex, args_src.Length, sql));
                                         arg_val = args_src[paramIndex];
                                     }
                                     else {
                                         // Look for a property on one of the arguments with this name
                                         bool found = false;
                                         arg_val = null;
                                         foreach(var o in args_src) {
                                             var pi = o.GetType().GetProperty(param);
                                             if(pi != null) {
                                                 arg_val = pi.GetValue(o, null);
                                                 found = true;
                                                 break;
                                             }
                                         }

                                         if(!found)
                                             throw new ArgumentException(
                                                 string.Format(
                                                     "Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')",
                                                     param, sql));
                                     }

                                     // Expand collections to parameter lists
                                     if((arg_val as System.Collections.IEnumerable) != null &&
                                        (arg_val as string) == null &&
                                        (arg_val as byte[]) == null) {
                                         var sb = new StringBuilder();
                                         foreach(var i in arg_val as System.Collections.IEnumerable) {
                                             sb.Append((sb.Length == 0 ? "@" : ",@") + args_dest.Count.ToString());
                                             args_dest.Add(i);
                                         }
                                         return sb.ToString();
                                     }

                                     args_dest.Add(arg_val);
                                     return "@" + (args_dest.Count - 1).ToString();
                                 }
                    );
        }
    }
}