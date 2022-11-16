using System.Collections.Generic;

namespace LetsMake
{
    public class GithubUpdateChecker
    {
        public SemanticVersion CurrentVersion
        {
            get;
            private set;
        }

        public string RemoteRepositoryName
        {
            get;
            private set;
        }

        public string RemoteRepositoryOwner
        {
            get;
            private set;
        }

        public IReadOnlyList<Octokit.Release> Releases
        {
            get;
            private set;
        } = null;

        public GithubUpdateChecker(SemanticVersion currentRunningVersion, string remoteRepositoryOwner, string remoteRepositoryName)
        {
            CurrentVersion = currentRunningVersion;
            RemoteRepositoryName = remoteRepositoryName;
            RemoteRepositoryOwner = remoteRepositoryOwner;
        }

        public async void CheckForUpdates()
        {
            Releases = await GitHubReleases.GetReleases(RemoteRepositoryOwner, RemoteRepositoryName);
        }

        public bool UpdateAvailable
        {
            get
            {
                return HasNewerRelease || HasNewerPrerelease;
            }
        }

        public Octokit.Release LatestRelease
        {
            get
            {
                return Releases?.LatestRelease();
            }
        }

        public Octokit.Release LatestPrerelease
        {
            get
            {
                return Releases?.LatestPrerelease();
            }
        }

        public bool HasNewerRelease
        {
            get
            {
                return (LatestRelease?.ReleaseSemVersionFromTag() ?? new SemanticVersion(0)) > CurrentVersion;
            }
        }

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