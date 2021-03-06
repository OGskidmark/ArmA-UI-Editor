﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ArmA_UI_Editor.Code
{
    public class AddInManager
    {
        public List<AddIn> AddIns { get; private set; }

        static AddInManager _instance = new AddInManager();
        public static AddInManager Instance { get { return _instance; } }
        public SQF.ClassParser.ConfigField MainFile { get; set; }
        private Dictionary<string, AddInUtil.UIElement> ConfigNameFileDictionary;
        private AddInManager()
        {
            this.AddIns = new List<AddIn>();
        }

        /// <summary>
        /// Will Reload all AddIns !AND the settings!
        /// Has to be callen at least once during startup
        /// </summary>
        /// <param name="progress">progress "report" function</param>
        public void ReloadAddIns(IProgress<Tuple<double, string>> progress)
        {
            this.MainFile = new SQF.ClassParser.ConfigField();
            this.MainFile.ToClass();
            this.ConfigNameFileDictionary = new Dictionary<string, AddInUtil.UIElement>();
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(basePath + @"AddIns\"))
            {
                var dirs = Directory.EnumerateDirectories(basePath + @"AddIns\");
                List<string> AddInInfoPaths = new List<string>();
                for (int i = 0; i < dirs.Count(); i++)
                {
                    progress.Report(new Tuple<double, string>((double)i / dirs.Count(), "Checking AddIn Directory"));
                    var it = dirs.ElementAt(i);
                    string curPath = it + @"\info.xml";
                    if (File.Exists(curPath))
                    {
                        AddInInfoPaths.Add(curPath);
                    }
                }

                for (int i = 0; i < AddInInfoPaths.Count(); i++)
                {
                    progress.Report(new Tuple<double, string>((double)i / AddInInfoPaths.Count(), "Reading in AddIns"));
                    var it = AddInInfoPaths.ElementAt(i);
                    this.AddIns.Add(AddIn.LoadAddIn(it));
                }

                for (int i = 0; i < this.AddIns.Count(); i++)
                {
                    var it = this.AddIns.ElementAt(i);
                    it.Initialize(new Progress<double>((d) => {
                        progress.Report(new Tuple<double, string>((i - 1 + d) / AddInInfoPaths.Count(), string.Format("Initializing AddIn '{0}'", it.Info.Name)));
                    }), this.MainFile);

                    for (int j = 0; j < it.UIElements.Count(); j++)
                    {
                        var file = it.UIElements.ElementAt(j);
                        progress.Report(new Tuple<double, string>((j + 1) / it.UIElements.Count(), string.Format("Linking '{0}'s files", it.Info.Name)));
                        this.ConfigNameFileDictionary.Add(file.ConfigKey, file);
                    }
                }
            }
            Settings.Deserialize();
        }
        public AddInUtil.UIElement GetElement(string s)
        {
            if (this.ConfigNameFileDictionary.ContainsKey(s))
                return this.ConfigNameFileDictionary[s];
            else
                return null;
        }
    }
}
