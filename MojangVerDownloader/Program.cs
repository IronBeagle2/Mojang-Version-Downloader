using MojangVerDownloader.json.mojang;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MojangVerDownloader
{
    internal class Program
    {
        public static String version = "v2.1";
        public static String mojangData = "http://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
        public static WebClient client;

        static void Main(string[] args)
        {
            Console.Title = $"Mojang Version Downloader {version} | (c) DEJVOSS Productions 2024";
            Console.WriteLine($"Mojang Version Downloader {version}");
            Console.WriteLine("------------------------------");

            Process.Start("downloads\\");

            client = new WebClient();
            Start();
        }

        static void Start()
        {
            //dl version list
            string versionList = client.DownloadString(mojangData);

            //parse verlist
            VersionListJson verList = JsonConvert.DeserializeObject<VersionListJson>(versionList);
            Console.WriteLine($"Latest release = {verList.latest.release}");
            Console.WriteLine($"Latest snapshot = {verList.latest.snapshot}");
            Console.WriteLine("------------------------------");

            //go through all versions
            foreach (VersionListJsonVersion ver in verList.versions)
            {
                Console.WriteLine($"{ver.id} ({ver.type})");

                versionWorker(ver.id, ver.type, ver.url);
            }

            Console.ReadLine();
        }

        //gets and works with each VersionJson
        static void versionWorker(String id, String type, String url)
        {
            //get VersionJson
            String versionJson = client.DownloadString(url);
            VersionJson verJson = JsonConvert.DeserializeObject<VersionJson>(versionJson);

            //download client + mappings
            Console.WriteLine("getting client...");
            downloader($"{id}.jar", verJson.downloads.client.url, $"downloads\\{type}\\client", verJson.downloads.client.sha1);
            Console.WriteLine("getting client_mappings...");
            downloader($"{id}.txt", verJson.downloads.client_mappings.url, $"downloads\\{type}\\client", verJson.downloads.client_mappings.sha1);

            //download server + mappings
            Console.WriteLine("getting server...");
            downloader($"{id}.jar", verJson.downloads.server.url, $"downloads\\{type}\\server", verJson.downloads.server.sha1);
            Console.WriteLine("getting server_mappings...");
            downloader($"{id}.txt", verJson.downloads.server_mappings.url, $"downloads\\{type}\\server", verJson.downloads.server_mappings.sha1);
            if(verJson.downloads.windows_server !=  null)
            {
                Console.WriteLine("getting windows_server...");
                downloader($"{id}.exe", verJson.downloads.windows_server.url, $"downloads\\{type}\\server", verJson.downloads.windows_server.sha1);
            }

            //finish
            Console.WriteLine();
        }

        static void downloader(String file, String url, String path, String sha1)
        {
            //get full path
            String filePath = $"{path}\\{file}";

            //create dir
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //download file
            if (!File.Exists(filePath))
            {
                client.DownloadFile(url, $"{path}\\{file}");
            }

            //check if hashes match => if yes continue, if no download again
            if (getHash(filePath) == sha1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Dowloaded!");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error! Attempting again!");
                Console.ForegroundColor = ConsoleColor.Gray;

                File.Delete(filePath);
                downloader(file, url, path, sha1);
            }

        }

        //gets file hash, courtesy of some stackoverflow user
        static String getHash(String filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
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

                    return formatted.ToString();
                }
            }
        }
    }
}
