using System;
using System.Collections.Generic;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// Serialize 수행을 위한 Sample class
    /// </summary>
    [Serializable]
    public class UserInfo : ValueObjectBase {
        public static UserInfo GetSample() {
            return GetSample(999);
        }

        public static UserInfo GetSample(int favoriteMovieCount) {
            var user = new UserInfo("Peter",
                                    "Bromberg",
                                    "101 Park Avenue West",
                                    "New York", "NY", "10021",
                                    "pbromberg@nospammin.yahoo.com",
                                    "petey",
                                    "whodunnit")
                       {
                           HomeAddr = new AddressInfo
                                      {
                                          Phone = "999-9999",
                                          Street = "정릉동",
                                          Proeprties = new List<string>()
                                                       {
                                                           "home",
                                                           "addr"
                                                       }
                                      },
                           OfficeAddr = new AddressInfo
                                        {
                                            Phone = "555-5555",
                                            Street = "논현동",
                                            Proeprties = new List<string>()
                                                         {
                                                             "office",
                                                             "addr"
                                                         }
                                        }
                       };

            for(int i = 0; i < favoriteMovieCount; i++) {
                user.FavoriteMovies.Add("Favorite Movie Number-" + i);
            }

            return user;
        }

        public List<string> FavoriteMovies = new List<string>();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public double? Age { get; set; }
        public DateTime? UpdateTime { get; set; }

        public byte[] ByteArray { get; set; }

        public string Description { get; set; }

        public AddressInfo HomeAddr { get; set; }
        public AddressInfo OfficeAddr { get; set; }

        public UserInfo() {
            HomeAddr = new AddressInfo();
            OfficeAddr = new AddressInfo();
        }

        public UserInfo(string firstName, string lastName, string address,
                        string city, string state, string zipCode, string email,
                        string userName, string passWord) {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            City = city;
            State = state;
            ZipCode = zipCode;
            Email = email;
            UserName = userName;
            Password = passWord;

            Age = 0;
            UpdateTime = DateTime.Now;

            ByteArray = ArrayTool.GetRandomBytes(1024 * 16);
            Description = "동해물과 백두산이 마르고 닳도록 하느님이 보우하사 우리나라 만세".Replicate(5000);

            HomeAddr = new AddressInfo();
            OfficeAddr = new AddressInfo();
        }

        public override int GetHashCode() {
            return HashTool.Compute(FirstName, LastName, Email);
        }

        [Serializable]
        public struct AddressInfo {
            public string Street { get; set; }
            public string Phone { get; set; }

            private IList<string> _properties;

            public IList<string> Proeprties {
                get { return _properties ?? (_properties = new List<string>()); }
                set { _properties = value; }
            }
        }
    }
}