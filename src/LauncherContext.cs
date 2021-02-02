using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

namespace NanoLauncher
{
    public class LauncherContext
    {
        public void Close() => Environment.Exit(1);

        private readonly IConfiguration _config;
        private WebClient _client;
        private IList<UpdatableFile> _updatableFiles;

        public LauncherContext(IConfiguration config)
        {
            _config = config;
            _client = new WebClient();
            _updatableFiles = new List<UpdatableFile>();
        }

        public async Task CheckUpdates()
        {
            if(!_updatableFiles.Any())
            {
                _updatableFiles = await GetUpdatableFilesAsync("");
            }
        }

        public void CompareLocalFiles()
        {
            _updatableFiles.AsParallel().ForAll(f =>
            {
                if (File.Exists(f.Name))
                {
                    using (var sha256 = SHA256.Create())
                    {
                        var file = File.Open(f.Name, FileMode.Open);
                        var hash = sha256.ComputeHash(file);
                        var hashString = BitConverter.ToString(hash).Replace("-", string.Empty);

                        if (!string.Equals(f.Hash, hashString, StringComparison.OrdinalIgnoreCase))
                        {
                            //Download file from backend
                        }
                    }
                }
            });
        }

        public async Task<IList<UpdatableFile>> GetUpdatableFilesAsync(string route)
        {
            var dict = await _client.DownloadStringTaskAsync($"{_config["ServerUrl"]}/{route}");
            return JsonSerializer.Deserialize<IList<UpdatableFile>>(dict);
        }
    }
}
