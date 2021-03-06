﻿using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using SQF.ClassParser;
using ArmA_UI_Editor.Code;
using ArmA_UI_Editor.Code.AddInUtil.PropertyUtil;

namespace ArmA_UI_Editor.UI.Snaps
{
    /// <summary>
    /// Interaction logic for PropertyWindow.xaml
    /// </summary>
    public partial class PropertySnap : Page, Code.Interface.ISnapWindow
    {
        public ConfigField CurrentField { get; private set; }
        public List<Code.AddInUtil.Group> CurrentGroups { get; private set; }
        public EditingSnap CurrentEditingSnap { get; private set; }

        public int AllowedCount { get { return 1; } }
        public Dock DefaultDock { get { return Dock.Right; } }

        public PropertySnap()
        {
            InitializeComponent();
        }

        private void PType_ValueChanged(object sender, PTypeDataTag e)
        {
            this.CurrentEditingSnap.UpdateConfigKey(string.Concat(e.Key, e.Path));
            (ArmA_UI_Editor.UI.MainWindow.TryGet()).SetStatusbarText("", false);
            //Tmp "fix" for outdated bindings
            //ToDo: Update AddIn Bindings
            this.CurrentEditingSnap.RegenerateDisplay();
        }
        private void PType_OnError(object sender, string e)
        {
            (ArmA_UI_Editor.UI.MainWindow.TryGet()).SetStatusbarText(e, true);
        }

        private void AddDefaultProperties()
        {
            var group = new Group();
            group.Header = "Default";
            this.PropertyStack.Children.Add(group);
            Property p;
            TextBox tb;

            p = new Property();
            tb = new TextBox();
            tb.PreviewTextInput += TextBox_ClassName_PreviewTextInput;
            tb.LostFocus += TextBox_ClassName_LostFocus;
            tb.Text = this.CurrentField.Name;
            p.Children.Add(tb);
            p.Header = "Class Name";
            group.Children.Add(p);
        }

        private void TextBox_ClassName_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.CurrentEditingSnap.RenameConfigKey(this.CurrentField.Key, (sender as TextBox).Text);
            this.CurrentField.Name = (sender as TextBox).Text;
            this.CurrentEditingSnap.RegenerateDisplay();
        }
        private void TextBox_ClassName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Utility.tb_PreviewTextInput_Ident_DoHandle(sender, e, () =>
            {
                this.CurrentEditingSnap.RenameConfigKey(this.CurrentField.Key, (sender as TextBox).Text);
                this.CurrentField.Name = (sender as TextBox).Text;
                this.CurrentEditingSnap.RegenerateDisplay();
            });
        }


        public void LoadProperties(List<Code.AddInUtil.Group> groups, string Key)
        {
            this.CurrentGroups = groups;
            this.CurrentField = AddInManager.Instance.MainFile[Key];
            List<Group> groupList = new List<Group>();
            this.PropertyStack.Children.Clear();
            this.AddDefaultProperties();

            foreach (var groupIt in groups)
            {
                var group = new Group();
                group.IsExpaned = true;
                group.Header = groupIt.Name;
                groupList.Add(group);
                foreach (var property in groupIt.Items)
                {
                    var el = new Property();
                    el.Header = property.DisplayName;
                    var fEl = property.PropertyType.GenerateUiElement(string.Concat(this.CurrentField.Key, property.FieldPath), CurrentEditingSnap, new Code.AddInUtil.PropertyUtil.PTypeDataTag { PropertyObject = property, Key = this.CurrentField.Key, Path = property.FieldPath });
                    el.Children.Add(fEl);
                    group.ItemsPanel.Children.Add(el);
                }
                foreach (var sqf in groupIt.SqfProperties)
                {
                    var el = new Property();
                    group.ItemsPanel.Children.Add(el);
                    el.Header = sqf.Name;
                    var field = this.CurrentField.GetKey("onLoad", ConfigField.KeyMode.NullOnNotFound);
                    int argCount = 0;
                    foreach(var it in sqf.Arguments)
                    {
                        if (it.Property != null)
                            argCount++;
                    }
                    if(argCount > 1)
                    {
                        MainWindow.TryGet().SetStatusbarText("SQF-Properties currently only support one non-control arg", true);
                    }
                    else
                    {
                        foreach (var it in sqf.Arguments)
                        {
                            if (it.Property != null)
                            {
                                var fEl = it.Property.GenerateUiElement(string.Concat(this.CurrentField.Key, "/onLoad"), CurrentEditingSnap, new Code.AddInUtil.PropertyUtil.PTypeDataTag { PropertyObject = sqf, Key = this.CurrentField.Key, Path = "/onLoad", Extra = it.Index });
                                el.Children.Add(fEl);
                                break;
                            }
                        }
                    }
                }
            }
            groupList.Sort((a, b) => a.Header.CompareTo(b.Header));
            foreach(var it in groupList)
            {
                this.PropertyStack.Children.Add(it);
            }
        }

        public void UnloadSnap()
        {
            Code.AddInUtil.PropertyUtil.PType.ValueChanged -= PType_ValueChanged;
            Code.AddInUtil.PropertyUtil.PType.OnError -= PType_OnError;
            (ArmA_UI_Editor.UI.MainWindow.TryGet()).Docker.OnSnapFocusChange -= Docker_OnSnapFocusChange;
            if(CurrentEditingSnap != null)
                CurrentEditingSnap.OnSelectedFocusChanged -= CurrentWindow_OnSelectedFocusChanged;

        }
        public void LoadSnap()
        {
            Code.AddInUtil.PropertyUtil.PType.ValueChanged += PType_ValueChanged;
            Code.AddInUtil.PropertyUtil.PType.OnError += PType_OnError;
            (ArmA_UI_Editor.UI.MainWindow.TryGet()).Docker.OnSnapFocusChange += Docker_OnSnapFocusChange;
            var EditingSnaps = (ArmA_UI_Editor.UI.MainWindow.TryGet()).Docker.FindSnaps<EditingSnap>(true);
            if (EditingSnaps.Count > 0)
            {
                CurrentEditingSnap = EditingSnaps[0];
                CurrentEditingSnap.OnSelectedFocusChanged += CurrentWindow_OnSelectedFocusChanged;
            }
        }

        private void CurrentWindow_OnSelectedFocusChanged(object sender, EditingSnap.OnSelectedFocusChangedEventArgs e)
        {
            if(e.Tags.Length != 1)
            {
                this.PropertyStack.Children.Clear();
            }
            else
            {
                LoadProperties(e.Tags[0].file.Properties, e.Tags[0].Key);
            }
        }

        private void Docker_OnSnapFocusChange(object sender, SnapDocker.OnSnapFocusChangeEventArgs e)
        {
            if (e.SnapWindowNew != null && e.SnapWindowNew.Window is EditingSnap)
            {
                this.PropertyStack.Children.Clear();
                if(CurrentEditingSnap != null)
                    CurrentEditingSnap.OnSelectedFocusChanged -= CurrentWindow_OnSelectedFocusChanged;
                CurrentEditingSnap = e.SnapWindowNew.Window as EditingSnap;
                CurrentEditingSnap.OnSelectedFocusChanged += CurrentWindow_OnSelectedFocusChanged;
            }
            else if (e.SnapWindowLast != null && e.SnapWindowLast.Window is EditingSnap)
            {
                if (e.SnapWindowLast.Window != CurrentEditingSnap)
                    return;
                if (CurrentEditingSnap != null)
                    CurrentEditingSnap.OnSelectedFocusChanged -= CurrentWindow_OnSelectedFocusChanged;
                this.PropertyStack.Children.Clear();
                CurrentEditingSnap = null;
            }
        }
    }
}
