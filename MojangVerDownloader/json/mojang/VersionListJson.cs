using System;

namespace MojangVerDownloader.json.mojang
{
    public class VersionListJson
    {
        public VersionListJsonLatest latest { get; set; }
        public VersionListJsonVersion[] versions { get; set; }
    }

    public class VersionListJsonLatest
    {
        public String release { get; set; }
        public String snapshot { get; set; }
    }

    public class VersionListJsonVersion
    {
        public String id { get; set; }
        public String type { get; set; }
        public String url { get; set; }
    }
}
