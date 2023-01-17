using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net;
using System.IO.Packaging;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Backres.Models
{
    internal class BackresConfig
    {

        #region Instance

        private static readonly Lazy<BackresConfig> _instance = new Lazy<BackresConfig>(Initialize);

        internal static BackresConfig Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private static BackresConfig Initialize()
        {

            var config = new BackresConfig()
            {

            };

            config.Load();

            return config;
        }

        public static string ReadResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            //if (!name.StartsWith(nameof(SignificantDrawerCompiler)))
            //{
            resourcePath = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));
            //}

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath)!)
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion

        #region Properties 

        public IList<BackresItem> Items;

        private string DefaultStorage
        {
            get
            {
                var machineName = Environment.MachineName ?? Dns.GetHostName() ?? Environment.GetEnvironmentVariable("COMPUTERNAME");
                var fingerPrint = PcFingerPrint.Value();
                var defaultStorageFolder = Path.Combine(AppContext.BaseDirectory, @$"{machineName} ({fingerPrint})");
                return defaultStorageFolder;
            }
        }

        private string _storage = String.Empty;

        public string Storage
        {
            get
            {
                return String.IsNullOrWhiteSpace(_storage) ? DefaultStorage : _storage;
            }
            set
            {
                _storage = value;
            }
        }

        private Dictionary<string, string> _actions;

        public Dictionary<string, string> Actions
        {
            get
            {
                if (_actions is null)
                {
                    _actions = new Dictionary<string, string>();
                }

                var folderPath = Path.Combine(AppContext.BaseDirectory, @"Actions");

                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

                if (System.IO.Directory.GetFiles(folderPath).Where(path => System.IO.Path.GetExtension(path) == "json").Count() == 0)
                {
                    var assembly = Assembly.GetExecutingAssembly();

                    var regexTemplate = @"Backres\.Actions\.(?<name>.*)\.json";

                    var items = assembly.GetManifestResourceNames()
                        .Where(str => Regex.IsMatch(str, regexTemplate))
                        .Select(str => Regex.Match(str, regexTemplate).Groups["name"].Value);

                    var writerOptions = new JsonWriterOptions
                    {
                        Indented = true
                    };

                    var documentOptions = new JsonDocumentOptions
                    {
                        CommentHandling = JsonCommentHandling.Skip
                    };

                    foreach (var item in items)
                    {
                        var content = ReadResource(@$"Backres.Actions.{item}.json");
                        try
                        {
                            string filePath = System.IO.Path.Combine(folderPath, $@"{item}.json");
                            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                            using FileStream fs = File.Create(filePath);
                            using var writer = new Utf8JsonWriter(fs, options: writerOptions);
                            using JsonDocument document = JsonDocument.Parse(content, documentOptions);
                            document.WriteTo(writer);
                            writer.Flush();
                        }
                        catch
                        {
                        }

                    }
                }

                var files = System.IO.Directory.GetFiles(folderPath).Where(path => System.IO.Path.GetExtension(path) == ".json");
                
                foreach(var file in files)
                {
                    var name = System.IO.Path.GetFileNameWithoutExtension(file);
                    var content = System.IO.File.ReadAllText(file);
                    JsonDocument.Parse(content);
                    _actions.Add(name, content);
                }


                return _actions;
            }
        }

        #endregion

        #region Load

        private void LoadActions(string folderPath)
        {

        }

        private void Load(bool forceCreate = true)
        {

            var filePath = Path.Combine(Storage, @"backres.json");

            if (!File.Exists(filePath))
            {
                if (forceCreate)
                {
                    string jsonString = Create();

                    var writerOptions = new JsonWriterOptions
                    {
                        Indented = true
                    };

                    var documentOptions = new JsonDocumentOptions
                    {
                        CommentHandling = JsonCommentHandling.Skip
                    };

                    System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                    file.Directory!.Create();

                    using FileStream fs = File.Create(filePath);
                    using var writer = new Utf8JsonWriter(fs, options: writerOptions);
                    using JsonDocument document = JsonDocument.Parse(jsonString, documentOptions);
                    document.WriteTo(writer);
                    writer.Flush();
                }
                else
                {
                    throw new Exception("File *.json dosn't exists in the current location.");
                }
            }

            var options = new JsonSerializerOptions { MaxDepth = 5 };
            Items = JsonSerializer.Deserialize<List<BackresItem>>(File.ReadAllText(filePath), options)!;
            var t = Actions;
        }

        private string Create()
        {
            System.Text.Json.JsonDocumentOptions options = default;

            var json = JsonNode.Parse(@"[]");

            var array = json.Root.AsArray();

            foreach (var item in Actions)
            {
                var node = JsonNode.Parse(item.Value);
                array.Add(node);
            }

            return json.ToString();
        }

        private JsonDocument AddItem(JsonDocument jsonDocument, string value)
        {

            //var jsd = JsonSerializer.Deserialize<List<BackresItem>>(File.ReadAllText(filePath), options);

            return jsonDocument;
        }

        #endregion

        #region Run Action

        public Task<bool> RunAction(string name, ActionDirection aDirection)
        {
            var tcs = new TaskCompletionSource<bool>();

            Task.Factory.StartNew(/*async*/ () =>
            {
                try
                {
                    BackresItem Item = Items.FirstOrDefault(item => item.Name == name);
                    var ActionItems = aDirection == ActionDirection.Backup ? Item.BackupActions.OrderBy(i => i.Order) : Item.RestoreActions.OrderBy(i => i.Order);
                    foreach (var action in ActionItems)
                    {
                        var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == "Action" + action.ActionName);

                        if (type == null)
                            continue;
                        action.ItemName = name;
                        IAction iaction = (IAction)Activator.CreateInstance(type, action, aDirection);

                        iaction.Run();
                    }
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                tcs.SetResult(true);
            }, TaskCreationOptions.None);
            return tcs.Task;
        }

        #endregion

    }

}

