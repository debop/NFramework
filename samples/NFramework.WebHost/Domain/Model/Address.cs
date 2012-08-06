using System;

namespace NSoft.NFramework.WebHost.Domain.Model
{
	/// <summary>
	/// 사용자의 주소 정보
	/// </summary>
	[Serializable]
	public class Address : ValueObjectBase, IEquatable<Address>
	{
		public string Addr { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }

		public bool Equals(Address other)
		{
			return (other != null) && GetHashCode().Equals(other.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			return (obj != null) && (obj is Address) && Equals((Address) obj);
		}

		public override int GetHashCode()
		{
			return HashTool.Compute(Addr, City, State, ZipCode);
		}

		public override string ToString()
		{
			return string.Format("Address# Addr=[{0}], City=[{1}], State=[{2}], ZipCode=[{3}]", Addr, City, State, ZipCode);
		}
	}
}