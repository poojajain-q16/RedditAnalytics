using System;
namespace Models
{
	public class UserPostCount : IComparable<UserPostCount>
    {

        public string User { get; set; }
        public int PostCount { get; set; }

        public int CompareTo(UserPostCount other)
        {
            return this.PostCount.CompareTo(other.PostCount);
        }

        public override bool Equals(object obj)
        {
            if (obj is UserPostCount)
            {
                return this.User == ((UserPostCount)obj).User;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.User.GetHashCode();
        }
    }
}

