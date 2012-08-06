using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Application의 Command Line 인자에 대한 파서입니다.
    /// </summary>
    /// <example>
    /// <code>
    /// // 유효 Parameter 형식:
    /// // {-, /, --}param{ ,=,:}((",')value(",'))
    /// 
    /// SampleApp.exe -param1 value1 --param2 /params3:"Test-:-work" /param4=happy -param5 '--=nice==='
    /// </code>
    /// </example>
    public class CommandLineParser {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly Regex Spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex Remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly IDictionary<string, string> _parameters = new Dictionary<string, string>();

        public CommandLineParser(string[] args) {
            if(args == null || args.Length == 0)
                return;

            string parameter = null;
            foreach(var arg in args) {
                var parts = Spliter.Split(arg, 3);

                switch(parts.Length) {
                        // Found a value (for the last parameter found (space separator))
                    case 1:
                        if(parameter != null) {
                            if(_parameters.ContainsKey(parameter) == false) {
                                parts[0] = Remover.Replace(parts[0], "$1");
                                _parameters.AddValue(parameter, parts[0]);
                            }
                            parameter = null;
                        }
                        break;

                        // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. With no value, set it to true.
                        if(parameter != null) {
                            if(_parameters.ContainsKey(parameter) == false)
                                _parameters.AddValue(parameter, "true");
                        }
                        parameter = parts[1];
                        break;

                        // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. With no value, set it to true.
                        if(parameter != null) {
                            if(_parameters.ContainsKey(parameter) == false)
                                _parameters.AddValue(parameter, "true");
                        }
                        parameter = parts[1];
                        // Remove possible enclosing characters (", ')
                        if(_parameters.ContainsKey(parameter) == false) {
                            parts[2] = Remover.Replace(parts[2], "$1");
                            _parameters.AddValue(parameter, parts[2]);
                        }
                        parameter = null;
                        break;
                }
            }

            // In case a parameter is still waiting
            if(parameter != null)
                if(_parameters.ContainsKey(parameter) == false)
                    _parameters.Add(parameter, "true");
        }

        public string this[string parameterName] {
            get { return _parameters.GetValueOrDefault(parameterName, string.Empty); }
        }
    }
}