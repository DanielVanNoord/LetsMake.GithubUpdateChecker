using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsMake
{
    public static class GitHubReleases
    {
        public static Task<IReadOnlyList<Release>> GetReleases(string owner, string name)
        {
            if (string.IsNullOrEmpty(owner))
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var Github = new GitHubClient(new ProductHeaderValue(name + @"-UpdateCheck"));

            return Github.Repository.Release.GetAll(owner, name);
        }

        public static Release LatestRelease(this IEnumerable<Release> releases, bool OnlySemVerTags = true)
        {
            if (releases == null)
            {
                throw new ArgumentNullException(nameof(releases));
            }

            return releases.Where(rp => !rp.Prerelease).Latest(OnlySemVerTags);
        }

        public static Release LatestPrerelease(this IEnumerable<Release> releases, bool OnlySemVerTags = true)
        {
            if (releases == null)
            {
                throw new ArgumentNullException(nameof(releases));
            }
            return releases.Where(rp => rp.Prerelease).Latest(OnlySemVerTags);
        }

        public static Release Latest(this IEnumerable<Release> releases, bool OnlySemVerTags = true)
        {
            if (releases == null)
            {
                throw new ArgumentNullException(nameof(releases));
            }
            if (releases.Any())
            {
                if (OnlySemVerTags)
                {
                    return releases.Where(r=>r.TagCanBeSemanticVersion()).OrderBy(rp => rp.ReleaseSemVersionFromTag()).LastOrDefault();
                }
                else
                {
                    return releases.OrderBy(rp => rp.ReleaseSemVersionFromTag()).LastOrDefault();
                }
            }
            return null;
        }

        public static SemanticVersion ReleaseSemVersionFromTag(this Release release)
        {
            if (release == null)
            {
                throw new ArgumentNullException(nameof(release));
            }
            if (!string.IsNullOrEmpty(release.TagName) && SemanticVersion.TryParse(release.TagName, out SemanticVersion _latest_release_version))
            {
                return _latest_release_version;
            }
            return new SemanticVersion(0, 0, 0, release.TagName ?? "No Tag");
        }

        public static bool TagCanBeSemanticVersion(this Release release)
        {
            if (release != null && !string.IsNullOrEmpty(release.TagName))
            {
                return SemanticVersion.TryParse(release.TagName, out SemanticVersion _);
            }
            return false;
        }
    }
}