using System;
using System.Text.RegularExpressions;

namespace Niflheim.Installer
{
    public class SemanticVersion : IComparable<SemanticVersion>
    {
        private static readonly Regex versionExpression = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)(\.(?<revision>[0-9]*))?$", RegexOptions.Compiled);

        private SemanticVersion(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public int Major { get; init; }
        public int Minor { get; init; }
        public int Revision { get; init; }

        public static SemanticVersion Parse(string v)
        {
            Match matchResult = versionExpression.Match(v);

            if (!matchResult.Success)
            {
                throw new InvalidOperationException($"The provided version string {v} was not a valid semantic version.");
            }

            if (!(matchResult.Groups.ContainsKey("major") && matchResult.Groups.ContainsKey("minor")))
            {
                throw new InvalidOperationException($"The provided version string {v} was not a valid semantic version. No major and minor version part were found.");
            }
            if (!int.TryParse(matchResult.Groups["major"].Value, out int major))
            {
                throw new InvalidOperationException($"The provided version string {v} was not a valid semantic version. The major part {matchResult.Groups["major"].Value} could not be parsed as an integer");
            }
            if (!int.TryParse(matchResult.Groups["minor"].Value, out int minor))
            {
                throw new InvalidOperationException($"The provided version string {v} was not a valid semantic version. The minor part {matchResult.Groups["minor"].Value} could not be parsed as an integer");
            }
            int revision = 0;
            if (matchResult.Groups.ContainsKey("revision"))
            {
                int.TryParse(matchResult.Groups["revision"].Value, out revision);
            }

            return new SemanticVersion(major, minor, revision);
        }

        public int CompareTo(SemanticVersion y)
        {
            var x = this;
            if (x.Major < y.Major)
            {
                return -1;
            }
            if (x.Major > y.Major)
            {
                return 1;
            }

            if (x.Minor < y.Minor)
            {
                return -1;
            }
            if (x.Minor > y.Minor)
            {
                return 1;
            }

            if (x.Revision < y.Revision)
            {
                return -1;
            }
            if (x.Revision > y.Revision)
            {
                return 1;
            }

            return 0;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            SemanticVersion version = obj as SemanticVersion;
            return (Major == version.Major && Minor == version.Minor && Revision == version.Revision);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SemanticVersion left, SemanticVersion right)
        {
            return (left.Equals(right));
        }

        public static bool operator !=(SemanticVersion left, SemanticVersion right)
        {
            return (!left.Equals(right));
        }

        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) == -1;
        }

        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) == 1;
        }

        public static bool operator <=(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}