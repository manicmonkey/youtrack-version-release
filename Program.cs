using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using YouTrackSharp.Infrastructure;
using YouTrackSharp.Projects;

namespace YoutrackUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine("YouTrack Release helper, marks a version as released and creates version numbers for follow up releases. Works with version numbers up to 3 digits");
                Console.WriteLine("");
                Console.WriteLine("Usage:");
                Console.WriteLine("  YouTrackRelease --server=<server> --username=<username> --password=<password> --project=<project> --version=<version>");
                Console.WriteLine("");
                Console.WriteLine("Options:");
                Console.WriteLine("  --server=<server>          Server address, ie http://youtrack.domain.com/youtrack");
                Console.WriteLine("  --username=<username>      User with admin role");
                Console.WriteLine("  --password=<password>      ");
                Console.WriteLine("  --project=<project>        ID of project being released");
                Console.WriteLine("  --version=<version>        The version number to be released (i.e 1.2.3)");
                return;
            }
            
            var server = GetArgNotNull(args, "--server");
            var username = GetArgNotNull(args, "--username");
            var password = GetArgNotNull(args, "--password");
            var projectId = GetArgNotNull(args, "--project");
            var version = GetArgNotNull(args, "--version");

            var uri = new Uri(server);
            Console.WriteLine("Connecting to server: {0}, {1}, {2}", uri, uri.Port, uri.AbsolutePath);
            
            var conn = new Connection(uri.Host, uri.Port, uri.Scheme == "https", uri.AbsolutePath);
            conn.Authenticate(username, password);

            var projectManagement = new ProjectManagement(conn);
            var project = projectManagement.GetProject(projectId);
            if (project == null)
                throw new Exception("Project does not exist");

            var existingVersions = projectManagement.GetVersions(project).Select((v) => v.Name);

            if (!existingVersions.Contains(version))
                throw new Exception("Version does not exist");

            Console.Out.WriteLine("Releasing version: {0}", version);
            projectManagement.UpdateVersion(project, new ProjectVersion { IsReleased = true, Name = version, ReleaseDate = JavaTimeNow() });

            var currentVersion = new VersionNumber(version);
            var nextVersions = currentVersion.GetNextReleaseVersions().Select((v) => v.GetVersion());

            var missingVersions = nextVersions.Where((v) => !existingVersions.Contains(v));
            foreach (var newVersion in missingVersions)
            {
                Console.Out.WriteLine("Adding version: {0}", newVersion);
                projectManagement.AddVersion(project, new ProjectVersion {IsReleased = false, Name = newVersion});
            }

            if (Debugger.IsAttached)
                Console.ReadLine();
        }

        private static long JavaTimeNow()
        {
            return (long) (TimeZoneInfo.ConvertTimeToUtc(DateTime.Now) - new DateTime(1970, 1, 1)).TotalSeconds * 1000;
        }

        private static string GetArgNotNull(string[] args, string argName)
        {
            var arg = GetArg(args, argName);
            if (arg == null)
                throw new Exception("Argument '" + argName + "' not supplied");
            return arg;
        }

        private static string GetArg(string[] args, string argName, string defaultValue = null)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith(argName))
                {
                    return arg.Substring(arg.IndexOf('=') + 1);
                }
            }
            return defaultValue;
        }
    }
}
