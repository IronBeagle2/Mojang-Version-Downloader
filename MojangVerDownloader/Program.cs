using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MojangVerDownloader
{
    internal class Program
    {
        public static string mojangData = "http://piston-meta.mojang.com/mc/game/version_manifest_v2.json";

        public static string fileSha1;
        public static string versionUrl;


        static void Main(string[] args)
        {
            Start();
        }

        public static void Start()
        {
            Console.Title = "Mojang Version Downloader v1.1";
            Console.WriteLine("Mojang Version Downloader v1.1");
            Console.WriteLine("by DEJVOSS Productions");
            Console.WriteLine("------------------------------");


            WebClient client = new WebClient();
            string origJson = client.DownloadString(mojangData);

            JObject origObj = JsonConvert.DeserializeObject<JObject>(origJson);
            var origProps = origObj.Properties();

            foreach (var oProp in origProps)
            {
                string oKey = oProp.Name;
                string oVal = oProp.Value.ToString();
                if (oKey == "latest")
                {
                    JObject verObj = JsonConvert.DeserializeObject<JObject>(oVal);
                    var verProps = verObj.Properties();
                    foreach (var vProp in verProps)
                    {
                        string vKey = vProp.Name;
                        string vVal = vProp.Value.ToString();
                        Console.WriteLine($"Latest {vKey} is {vVal}");
                    }
                    Console.Write("------------------------------");
                }
                else if (oKey == "versions")
                {
                    List<jsonObject> data = JsonConvert.DeserializeObject<List<jsonObject>>(oVal);

                    foreach (var vers in data)
                    {
                        string verName = vers.url;
                        int index = verName.LastIndexOf("/");
                        if (index >= 0)
                            verName = verName.Substring(verName.LastIndexOf("/") + 1);

                        string verName2 = verName.Substring(0, verName.Length - 5);

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"\n{vers.id} ({vers.type})");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        versionJson(vers.url, vers.type, verName2);
                    }

                    Console.WriteLine("------------------------------");
                    Console.WriteLine("Finished!");
                    Console.WriteLine("------------------------------");
                }
            }
            Console.ReadLine();
        }

        public static void versionJson(string url, string category, string verName)
        {
            WebClient client = new WebClient();
            string origJson = client.DownloadString(url);

            JObject origObj = JsonConvert.DeserializeObject<JObject>(origJson);
            var origProps = origObj.Properties();

            foreach (var oProp in origProps)
            {
                string oKey = oProp.Name;
                string oVal = oProp.Value.ToString();

                if(oKey == "downloads")
                {
                    JObject dlObj = JsonConvert.DeserializeObject<JObject>(oVal);
                    var dlProps = dlObj.Properties();

                    foreach (var dlProp in dlProps)
                    {
                        string dlKey = dlProp.Name;
                        string dlVal = dlProp.Value.ToString();

                        JObject dl2Obj = JsonConvert.DeserializeObject<JObject>(dlVal);
                        var dl2Props = dl2Obj.Properties();

                        foreach (var dl2Prop in dl2Props)
                        {
                            string dl2Key = dl2Prop.Name;
                            string dl2Val = dl2Prop.Value.ToString();

                            if (dl2Key == "url")
                            {
                                versionUrl = dl2Prop.Value.ToString();
                            }
                            else if (dl2Key == "sha1")
                            {
                                fileSha1 = dl2Val;
                            }
                        }
                        Console.WriteLine($"{versionUrl}");

                        if (dlKey == "client")
                        {
                            downloadFile($"downloads2\\{category}\\client", versionUrl, $"{verName}.jar", fileSha1, verName);
                        }
                        else if (dlKey == "server")
                        {
                            downloadFile($"downloads2\\{category}\\server", versionUrl, $"{verName}.jar", fileSha1, verName);
                        }
                        else if (dlKey == "windows_server")
                        {
                            downloadFile($"downloads2\\{category}\\server", versionUrl, $"{verName}.exe", fileSha1, verName);
                        }
                        else if (dlKey == "client_mappings")
                        {
                            downloadFile($"downloads2\\{category}\\client", versionUrl, $"{verName}.txt", fileSha1, verName);
                        }
                        else if (dlKey == "server_mappings")
                        {
                            downloadFile($"downloads2\\{category}\\server", versionUrl, $"{verName}.txt", fileSha1, verName);
                        }
                        else
                        {
                            Console.WriteLine($"Unknown type!");
                        }
                    }
                }
            }
        }

        public static void downloadFile(string dir, string url, string fileName, string fileHash, string verName)
        {
            if(!File.Exists($"{dir}\\{fileName}"))
            {
                Directory.CreateDirectory(dir);

                using (var client2 = new WebClient())
                {
                    client2.DownloadFile(url, $"{dir}\\{fileName}");
                }
                downloadFile(dir, url, fileName, fileHash, verName);
            }
            else
            {
                string fullFileHash = "";
                using (FileStream fs = new FileStream($"{dir}\\{fileName}", FileMode.Open))
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (SHA1Managed sha1 = new SHA1Managed())
                    {
                        byte[] hash = sha1.ComputeHash(bs);
                        StringBuilder formatted = new StringBuilder(2 * hash.Length);
                        foreach (byte b in hash)
                        {
                            formatted.AppendFormat("{0:x2}", b);
                        }
                        fullFileHash = formatted.ToString();
                    }
                }
                if (fullFileHash == fileHash)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Dowloaded {fileName} at {fileHash}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error! Attempting again!");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    File.Delete($"{dir}\\{fileName}");
                    downloadFile(dir, url, fileName, fileHash, verName);
                }

            }
        }
    }

    public class jsonObject
    {
        public string id { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }
}
