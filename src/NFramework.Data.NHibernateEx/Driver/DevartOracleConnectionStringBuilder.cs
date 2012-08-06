using NSoft.NFramework.Tools;

namespace FluentNHibernate.Cfg.Db {
    /// <summary>
    /// DevartOracle 을 위한 ConnectionStringBuilder입니다. FluentNHibernate로 설정 시에 사용됩니다.
    /// </summary>
    public sealed class DevartOracleConnectionStringBuilder : ConnectionStringBuilder {
        private const string ConnectionFormat = "Server={0};Sid={1};Port={2};User Id={3};Password={4};Direct={5};Pooling={6};{7};";
        private string _server;
        private string _sid;
        private int _port = 1521;

        private string _userId;
        private string _password;

        private bool _direct = true;
        private bool _pool = true;
        private string _otherOptions;

        protected override string Create() {
            string connStr = base.Create();

            if(connStr.IsNotWhiteSpace())
                return connStr;

            return string.Format(ConnectionFormat,
                                 _server,
                                 _sid,
                                 _port,
                                 _userId,
                                 _password,
                                 _direct,
                                 _pool,
                                 _otherOptions);
        }

        public DevartOracleConnectionStringBuilder Server(string server) {
            _server = server;
            IsDirty = true;
            return this;
        }

        public DevartOracleConnectionStringBuilder Sid(string sid) {
            _sid = sid;
            IsDirty = true;
            return this;
        }

        public DevartOracleConnectionStringBuilder Port(int port) {
            _port = port;
            IsDirty = true;
            return this;
        }

        public DevartOracleConnectionStringBuilder UserId(string userId) {
            _userId = userId;
            IsDirty = true;
            return this;
        }

        public DevartOracleConnectionStringBuilder Password(string password) {
            _password = password;
            IsDirty = true;
            return this;
        }

        public DevartOracleConnectionStringBuilder Direct(bool direct) {
            _direct = direct;
            IsDirty = true;
            return this;
        }

        public DevartOracleConnectionStringBuilder Pool(bool pool) {
            _pool = pool;
            IsDirty = true;
            return this;
        }

        public DevartOracleConnectionStringBuilder OtherOptions(string otherOptions) {
            if(otherOptions.IsWhiteSpace())
                return this;

            _otherOptions = otherOptions;
            IsDirty = true;
            return this;
        }
    }
}