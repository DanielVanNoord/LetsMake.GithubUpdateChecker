using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetsMake
{
    public class GithubUpdateChecker
    {
        /// <summary>
        /// The local projects or programs current version
        /// </summary>
        public SemanticVersion CurrentVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the remote repository
        /// </summary>
        public string RemoteRepositoryName
        {
            get;
            private set;
        }

        /// <summary>
        /// The remove repository owner, often this is the user or group name
        /// </summary>
        public string RemoteRepositoryOwner
        {
            get;
            private set;
        }

        /// <summary>
        /// All releases from the repository, will be null until the CheckForUpdates has succeeded 
        /// </summary>
        public IReadOnlyList<Octokit.Release> Releases
        {
            get;
            private set;
        } = null;


        /// <summary>
        /// Creates a GithubUpdateChecker
        /// </summary>
        /// <param name="currentRunningVersion">The local projects or programs current version</param>
        /// <param name="remoteRepositoryOwner">The remove repository owner, often this is the user or group name</param>
        /// <param name="remoteRepositoryName">The name of the remote repository</param>
        public GithubUpdateChecker(SemanticVersion currentRunningVersion, string remoteRepositoryOwner, string remoteRepositoryName)
        {
            CurrentVersion = currentRunningVersion;
            RemoteRepositoryName = remoteRepositoryName;
            RemoteRepositoryOwner = remoteRepositoryOwner;
        }

        /// <summary>
        /// Checks the specified repository for releases. May throw based on failures to get data.
        /// </summary>
        public async Task CheckForUpdates()
        {
            Releases = await GitHubReleases.GetReleases(RemoteRepositoryOwner, RemoteRepositoryName);
        }

        /// <summary>
        /// True if any update is available (must be higher then the CurrentVersion)
        /// </summary>
        public bool UpdateAvailable
        {
            get
            {
                return HasNewerRelease || HasNewerPrerelease;
            }
        }

        /// <summary>
        /// Get the latest release from the repository
        /// </summary>
        public Octokit.Release LatestRelease
        {
            get
            {
                return Releases?.LatestRelease();
            }
        }

        /// <summary>
        /// Gets the latest pre-release from the repository
        /// </summary>
        public Octokit.Release LatestPrerelease
        {
            get
            {
                return Releases?.LatestPrerelease();
            }
        }

        /// <summary>
        /// True if there is a release higher then the CurrentVersion
        /// </summary>
        public bool HasNewerRelease
        {
            get
            {
                return (LatestRelease?.ReleaseSemVersionFromTag() ?? new SemanticVersion(0)) > CurrentVersion;
            }
        }

        /// <summary>
        /// True if there is a pre-release that is higher then the CurrentVersion and the LatestRelease
        /// </summary>
        public bool HasNewerPrerelease
        {
            get
            {
                if ((LatestPrerelease?.ReleaseSemVersionFromTag() ?? new SemanticVersion(0)) > (LatestRelease?.ReleaseSemVersionFromTag() ?? new SemanticVersion(0)))
                {
                    return (LatestPrerelease?.ReleaseSemVersionFromTag() ?? new SemanticVersion(0)) > CurrentVersion;
                }
                return false;
            }
        }
    }
}