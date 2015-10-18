using System.Collections.Generic;

namespace YoutrackUpdate
{
    class VersionNumber
    {
        private readonly string _version;

        public VersionNumber(string version)
        {
            _version = version;
        }

        public string GetVersion()
        {
            return _version;
        }

        public List<VersionNumber> GetNextReleaseVersions()
        {
            var releaseVersions = new List<VersionNumber> {NextMajor()};
            if (SplitVersion(_version).Length > 1)
                releaseVersions.Add(NextMinor());
            if (SplitVersion(_version).Length > 2)
                releaseVersions.Add(NextMaintenance());
            return releaseVersions;
        }

        private VersionNumber NextMajor()
        {
            return new VersionNumber(IncrementPart(_version, 1));
        }

        private VersionNumber NextMinor()
        {
            return new VersionNumber(IncrementPart(_version, 2));
        }

        private VersionNumber NextMaintenance()
        {
            return new VersionNumber(IncrementPart(_version, 3));
        }

        /// <summary>
        /// Increments part of a version number and zeroes the remainder. 1-based.
        /// </summary>
        private static string IncrementPart(string version, int part)
        {
            var currentPart = GetVersionPart(version, part) + 1;
            var updatedVersion = UpdateVersionPart(version, part, currentPart);
            part++;
            while (part - 1 < SplitVersion(version).Length)
            {
                updatedVersion = UpdateVersionPart(updatedVersion, part, 0);
                part++;
            }
            return updatedVersion;
        }

        private static int GetVersionPart(string version, int part)
        {
            return int.Parse(SplitVersion(version)[part - 1]);
        }

        private static string UpdateVersionPart(string version, int part, int number)
        {
            var versionParts = SplitVersion(version);
            versionParts[part - 1] = number.ToString();
            return string.Join(".", versionParts);
        }

        private static string[] SplitVersion(string version)
        {
            return version.Split('.');
        }
    }
}
