using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YoutrackUpdate.Tests
{
    [TestClass]
    public class VersionNumberTest
    {
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FailsOnChar()
        {
            new VersionNumber("a");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FailsOnEmpty()
        {
            new VersionNumber("");
        }

        [TestMethod]
        public void MajorVersionOnlyToString()
        {
            var versionNumber = new VersionNumber("1");
            Assert.AreEqual("1", versionNumber.ToString());
        }

        [TestMethod]
        public void MajorMinorVersionToString()
        {
            var versionNumber = new VersionNumber("1.2");
            Assert.AreEqual("1.2", versionNumber.ToString());
        }

        [TestMethod]
        public void MajorMinorMaintenanceVersionToString()
        {
            var versionNumber = new VersionNumber("1.2.3");
            Assert.AreEqual("1.2.3", versionNumber.ToString());
        }

        [TestMethod]
        public void MajorVersionNextRelease()
        {
            var versionNumber = new VersionNumber("1");
            Assert.IsTrue(new List<VersionNumber> { new VersionNumber("2") }.SequenceEqual(versionNumber.GetNextReleaseVersions()));
        }

        [TestMethod]
        public void MajorMinorVersionNextReleases()
        {
            var versionNumber = new VersionNumber("1.2");
            var nextReleaseVersions = versionNumber.GetNextReleaseVersions();
            Assert.IsTrue(new List<VersionNumber> { new VersionNumber("2.0"), new VersionNumber("1.3")}.SequenceEqual(nextReleaseVersions), "Was not equal to: {0}", string.Join(", ", nextReleaseVersions));
        }

        [TestMethod]
        public void MajorMinorMaintenanceVersionNextReleases()
        {
            var versionNumber = new VersionNumber("1.2.3");
            var nextReleaseVersions = versionNumber.GetNextReleaseVersions();
            Assert.IsTrue(new List<VersionNumber> { new VersionNumber("2.0.0"), new VersionNumber("1.3.0"), new VersionNumber("1.2.4") }.SequenceEqual(nextReleaseVersions), "Was not equal to: {0}", string.Join(", ", nextReleaseVersions));
        }
    }
}
