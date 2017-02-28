﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Document;
using VirtualRealityEngine.Config.Parser;
using System.IO;
using ArmA.Studio.DataContext.TextEditorUtil;

namespace ArmA.Studio.DataContext
{
    public class ConfigEditorDocument : TextEditorDocument
    {
        private static IHighlightingDefinition ThisSyntaxName { get; set; }
        static ConfigEditorDocument()
        {
            ThisSyntaxName = LoadAvalonEditSyntaxFiles(System.IO.Path.Combine(App.SyntaxFilesPath, "armaconfig.xshd"));
        }
        public override string[] SupportedFileExtensions { get { return new string[] { ".hpp", ".cpp", ".ext" }; } }
        public override IHighlightingDefinition SyntaxDefinition { get { return ThisSyntaxName; } }

        public ConfigEditorDocument() : base()
        {
            
        }
        protected override IEnumerable<SyntaxError> GetSyntaxErrors()
        {
            using (var memstream = new MemoryStream())
            {
                { //Load content into MemoryStream
                    var writer = new StreamWriter(memstream);
                    writer.Write(this.Document.Text);
                    writer.Flush();
                    memstream.Seek(0, SeekOrigin.Begin);
                }
                //Setup base requirements for the parser
                var parser = new Parser(new Scanner(memstream));
                parser.Root = null;
                parser.doc = null;
                parser.Parse();
                return parser.errors.ErrorList.Select((it) => new SyntaxError() { StartOffset = it.Item1, Length = it.Item2, Message = it.Item3 });
            }
        }
    }
}