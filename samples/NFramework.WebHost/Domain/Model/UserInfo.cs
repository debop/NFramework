using System;
using System.Collections.Generic;

namespace NSoft.NFramework.WebHost.Domain.Model
{
	/// <summary>
	/// Summary description for Class1
	/// </summary>
	[Serializable]
	public class UserInfo : ValueObjectBase, IEquatable<UserInfo>
	{
		public UserInfo()
		{
			Address = new Address();
			// this.FavoriteMovies = new Dictionary<string, string>();
			FavoriteMovies = new List<string>();
		}

		public UserInfo(string firstName, string lastName, string address,
		                string city, string state, string zipCode, string email,
		                string userName, string passWord)
			: this()
		{
			FirstName = firstName;
			LastName = lastName;
			Address.Addr = address;
			Address.City = city;
			Address.State = state;
			Address.ZipCode = zipCode;
			Email = email;
			UserName = userName;
			Password = passWord;
		}

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public Address Address { get; set; }

		public string Email { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }

		// public IDictionary<string, string> FavoriteMovies { get; set; }
		public IList<string> FavoriteMovies { get; set; }

		public bool Equals(UserInfo other)
		{
			return (other != null) && GetHashCode().Equals(other.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			return (obj != null) && (obj is UserInfo) && Equals((UserInfo) obj);
		}

		public override int GetHashCode()
		{
			return HashTool.Compute(FirstName, LastName, Email);
		}

		public override string ToString()
		{
			return string.Format("UserInfo# FirstName=[{0}], LastName=[{1}], Email=[{2}]", FirstName, LastName, Email);
		}
	}
}