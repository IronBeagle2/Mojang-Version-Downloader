using System;

namespace MojangVerDownloader.json.mojang
{
    public class VersionJson
    {
        public VersionJsonDownloads downloads;
    }

    public class VersionJsonDownloads
    {
        public VersionJsonDownloadsObj client { get; set; }
        public VersionJsonDownloadsObj client_mappings { get; set; }
        public VersionJsonDownloadsObj server { get; set; }
        public VersionJsonDownloadsObj windows_server { get; set; }
        public VersionJsonDownloadsObj server_mappings { get; set; }
    }

    public class VersionJsonDownloadsObj
    {
        public String sha1 { get; set; }
        public String url { get; set; }
    }
}
