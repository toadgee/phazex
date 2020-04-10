namespace PhazeX.Helpers
{
    using System;
    using PhazeX.Network;

    public static class Updates
    {
        public static UpdateResult CheckForUpdates(string url, int major, int minor, int build)
        {
            string text = HTTPLib.getPage(url);
            if (text.Length == 0)
            {
                return UpdateResult.CouldNotLoadPage;
            }

            text = text.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "");
            string[] vs = text.Split(new char[] { '.' });
            if (vs.Length != 3)
            {
                return UpdateResult.InvalidData;
            }

            // comparison code
            int serverMajor = Int32.Parse(vs[0]);
            int serverMinor = Int32.Parse(vs[1]);
            int serverBuild = Int32.Parse(vs[2]);
            if ((serverMajor > major)
            || ((serverMajor == major) && (serverMinor > minor))
            || ((serverMajor == major) && (serverMinor == minor) && (serverBuild > build)))
            {
                return UpdateResult.NewVersionAvailable;
            }
            else
            {
                return UpdateResult.UpToDate;
            }
        }
    }
}

    