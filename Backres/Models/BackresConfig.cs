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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Diagnostics;

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

            config.LoadCommonItems();

            config.LoadItems();

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

        #region Storage & Items

        #region Storage

        private string DefaultStorage()
        {
            var machineName = Environment.MachineName ?? Dns.GetHostName() ?? Environment.GetEnvironmentVariable("COMPUTERNAME");
            var fingerPrint = PcFingerPrint.Value();
            var defaultStorageFolder = System.IO.Path.Combine(AppContext.BaseDirectory, @$"{machineName} ({fingerPrint})");
            return defaultStorageFolder;

        }

        private string _storage = String.Empty;

        public string Storage
        {
            get
            {
                return String.IsNullOrWhiteSpace(_storage) ? DefaultStorage() : _storage;
            }
            set
            {
                _storage = value;
            }
        }

        private string CommonItemsFolder = System.IO.Path.Combine(AppContext.BaseDirectory, @"Items");

        private string StorageItemsFolder
        {
            get
            {
                return System.IO.Path.Combine(Storage, @"_Items");
            }
        }

        #endregion

        #region Items

        public ObservableDictionary<string, BackresItem> Items;

        private void LoadItems(bool forceCreate = true)
        {

            if (!System.IO.Directory.Exists(StorageItemsFolder))
            {
                if (forceCreate)
                {
                    Directory.CreateDirectory(StorageItemsFolder);
                }
                else
                {
                    throw new Exception("Directory with Items doesn't exest");
                }
            }

            var directoryInfo = new DirectoryInfo(StorageItemsFolder);

            Items = new ObservableDictionary<string, BackresItem>();

            foreach (var file in directoryInfo.GetFiles())
            {
                using var stream = file.OpenRead();
                var item = JsonSerializer.Deserialize<BackresItem>(stream);
                if (Items.ContainsKey(item.Name))
                {
                    Trace.WriteLine(@$"Item with Name(key) ""{item.Name}"" already exist in collection.");
                }
                else
                {
                    Items.Add(item.Name, item);
                }
            }
        }

        public void SaveItems()
        {
            foreach (var item in Items)
            {
                var filePath = @$"{StorageItemsFolder}\{item.Value.Name}.json";
                using var file = System.IO.File.Create(filePath);
                //using var stream = file.OpenRead();
                var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
                JsonSerializer.Serialize(file, item.Value, jsonSerializerOptions);
            }
        }

        //public void AddItems(List<BackresItem> items)
        //{
        //    foreach (var item in items)
        //    {
        //        try
        //        {
        //            Items.Add(item.Name, item);
        //        }
        //        catch (ArgumentException ex)
        //        {
        //            if (ex.Message == "The dictionary already contains the key")
        //            {

        //            }
        //        }
        //    }
        //}

        public void AddItems(ObservableDictionary<string, BackresItem> items)
        {
            foreach (var item in items)
            {
                if (Items.ContainsKey(item.Key))
                {
                    Trace.WriteLine(@$"Item with Name(key) ""{item.Key}"" already exist in collection.");
                }
                else
                {
                    Items.Add(item.Key, item.Value);
                }
            }
        }

        public void DeleteItem(ObservableKeyValuePair<string, BackresItem> item)
        {
            DeleteItem(item.Value);
        }

        public void DeleteItem(BackresItem item)
        {
            Items.Remove(item.Name);
        }

        public void DeleteItems(List<BackresItem> items)
        {
            foreach (var item in items)
            {
                DeleteItem(item);
            }
        }

        #endregion

        #endregion

        #region CommonItems

        private ObservableDictionary<string, BackresItem> _commonItems = new ObservableDictionary<string, BackresItem>();

        public ObservableDictionary<string, BackresItem> CommonItems
        {
            get
            {
                return _commonItems;
            }
        }

        private void LoadCommonItems()
        {

            if (!System.IO.Directory.Exists(CommonItemsFolder))
            {
                System.IO.Directory.CreateDirectory(CommonItemsFolder);
            }

            #region Fill common items folder
            // check content of common items folder and if folder empty read items form resources 
            if (System.IO.Directory.GetFiles(CommonItemsFolder).Where(path => System.IO.Path.GetExtension(path) == "json").Count() == 0)
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
                        string filePath = System.IO.Path.Combine(CommonItemsFolder, $@"{item}.json");
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
            #endregion

            if (_commonItems is null)
            {
                _commonItems = new ObservableDictionary<string, BackresItem>();
            }

            _commonItems.Clear();

            var files = System.IO.Directory.GetFiles(CommonItemsFolder).Where(path => System.IO.Path.GetExtension(path) == ".json");

            foreach (var file in files)
            {
                var content = System.IO.File.ReadAllText(file);
                var item = JsonSerializer.Deserialize<BackresItem>(content);
                if (_commonItems.ContainsKey(item.Name))
                {
                    Trace.WriteLine(@$"Item with name(key) ""{item.Name}"" already exist.");
                }
                else
                {
                    _commonItems.Add(item.Name, item);
                }
            }

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
                    BackresItem Item = Items[name];
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

