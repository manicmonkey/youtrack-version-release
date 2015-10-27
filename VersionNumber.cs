using System;
using System.Collections.Generic;

namespace YoutrackUpdate
{
    public class VersionNumber : IEquatable<VersionNumber>
    {
        private readonly int _major;
        private readonly int? _minor;
        private readonly int? _maintenance;

        public VersionNumber(string version)
        {
            var parts = version.Split('.');
            _major = int.Parse(parts[0]);
            if (parts.Length > 1)
                _minor = int.Parse(parts[1]);
            if (parts.Length > 2)
                _maintenance = int.Parse(parts[2]);
        }

        public VersionNumber(int major, int? minor, int? maintenance)
        {
            _major = major;
            _minor = minor;
            _maintenance = maintenance;
        }

        public override string ToString()
        {
            var version = _major.ToString();
            if (_minor.HasValue)
                version += "." + _minor.Value;
            if (_maintenance.HasValue)
                version += "." + _maintenance.Value;
            return version;
        }

        public List<VersionNumber> GetNextReleaseVersions()
        {
            var releaseVersions = new List<VersionNumber> {NextMajor()};
            if (_minor.HasValue)
                releaseVersions.Add(NextMinor());
            if (_maintenance.HasValue)
                releaseVersions.Add(NextMaintenance());
            return releaseVersions;
        }

        private VersionNumber NextMajor()
        {
            return new VersionNumber(_major + 1, NullableOrZero(_minor), NullableOrZero(_maintenance));
        }

        private VersionNumber NextMinor()
        {
            return new VersionNumber(_major, _minor + 1, NullableOrZero(_maintenance));
        }

        private VersionNumber NextMaintenance()
        {
            return new VersionNumber(_major, _minor, _maintenance + 1);
        }

        private static int? NullableOrZero(int? potentialValue)
        {
            return potentialValue.HasValue ? 0 : (int?)null;
        }

        public bool Equals(VersionNumber other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _major == other._major && _minor == other._minor && _maintenance == other._maintenance;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VersionNumber) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _major;
                hashCode = (hashCode*397) ^ _minor.GetHashCode();
                hashCode = (hashCode*397) ^ _maintenance.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(VersionNumber left, VersionNumber right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VersionNumber left, VersionNumber right)
        {
            return !Equals(left, right);
        }
    }
}
