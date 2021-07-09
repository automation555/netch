﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Netch.Controllers;
using Netch.Enums;
using Netch.Models;
using Netch.Properties;
using Netch.Services;
using Netch.Utils;

namespace Netch.Forms.Mode
{
    public partial class Process : Form
    {
        public Models.Mode? Mode { get; }

        public Process(Models.Mode? mode = null)
        {
            if (mode is { Type: not ModeType.Process })
                throw new ArgumentOutOfRangeException();

            Mode = mode;

            InitializeComponent();
            Icon = Resources.icon;
        }

        public void ModeForm_Load(object sender, EventArgs e)
        {
            if (Mode != null)
            {
                Text = "Edit Process Mode";

                RemarkTextBox.TextChanged -= RemarkTextBox_TextChanged;
                RemarkTextBox.Text = Mode.Remark;
                FilenameTextBox.Text = Mode.RelativePath;
                RuleAddRange(Mode.Content);
            }

            i18N.TranslateForm(this);
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true,
                Title = i18N.Translate("Select a folder"),
                AddToMostRecentlyUsedList = false,
                EnsurePathExists = true,
                NavigateToShortcut = true
            };

            if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
                foreach (string p in dialog.FileNames)
                {
                    string path = p;
                    if (!path.EndsWith(@"\"))
                        path += @"\";

                    RuleAdd($"^{path.ToRegexString()}");
                }
        }

        public void ControlButton_Click(object sender, EventArgs e)
        {
            if (!RuleRichTextBox.Lines.Any())
            {
                MessageBoxX.Show(i18N.Translate("Unable to add empty rule"));
                return;
            }

            if (string.IsNullOrWhiteSpace(RemarkTextBox.Text))
            {
                MessageBoxX.Show(i18N.Translate("Please enter a mode remark"));
                return;
            }

            if (string.IsNullOrWhiteSpace(FilenameTextBox.Text))
            {
                MessageBoxX.Show(i18N.Translate("Please enter a mode filename"));
                return;
            }

            var modeService = DI.GetRequiredService<ModeService>();

            if (Mode != null)
            {
                Mode.Remark = RemarkTextBox.Text;
                Mode.Content.Clear();
                Mode.Content.AddRange(RuleRichTextBox.Lines);

                modeService.UpdateMode(Mode);
                MessageBoxX.Show(i18N.Translate("Mode updated successfully"));
            }
            else
            {
                var relativePath = FilenameTextBox.Text;
                var fullName = ModeService.GetFullPath(relativePath);
                if (File.Exists(fullName))
                {
                    MessageBoxX.Show(i18N.Translate("File already exists.\n Please Change the filename"));
                    return;
                }

                var mode = new Models.Mode(fullName)
                {
                    Type = ModeType.Process,
                    Remark = RemarkTextBox.Text
                };

                mode.Content.AddRange(RuleRichTextBox.Lines);

                modeService.CreateMode(mode);

                MessageBoxX.Show(i18N.Translate("Mode added successfully"));
            }

            Close();
        }

        private void RemarkTextBox_TextChanged(object? sender, EventArgs? e)
        {
            BeginInvoke(new Action(() =>
            {
                FilenameTextBox.Text = FilenameTextBox.Text = ModeEditorUtils.GetCustomModeRelativePath(RemarkTextBox.Text);
            }));
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false,
                Title = i18N.Translate("Select a folder"),
                AddToMostRecentlyUsedList = false,
                EnsurePathExists = true,
                NavigateToShortcut = true
            };

            if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName;
                var list = new List<string>();
                const uint maxCount = 50;
                try
                {
                    ScanDirectory(path, list);
                }
                catch
                {
                    MessageBoxX.Show(i18N.Translate($"The number of executable files in the \"{path}\" directory is greater than {maxCount}"),
                        LogLevel.WARNING);

                    return;
                }

                RuleAddRange(list);
            }
        }

        private void ScanDirectory(string directory, List<string> list, uint maxCount = 30)
        {
            foreach (string dir in Directory.GetDirectories(directory))
                ScanDirectory(dir, list, maxCount);

            list.AddRange(
                Directory.GetFiles(directory).Select(s => Path.GetFileName(s)).Where(s => s.EndsWith(".exe")).Select(s => s.ToRegexString()));

            if (maxCount != 0 && list.Count > maxCount)
                throw new Exception("The number of results is greater than maxCount");
        }

        private void ValidationButton_Click(object sender, EventArgs e)
        {
            if (!NFController.CheckRules(Rules, out var results))
                MessageBoxX.Show(NFController.GenerateInvalidRulesMessage(results), LogLevel.WARNING);
            else
                MessageBoxX.Show("Fine");
        }

        #region Model

        public IEnumerable<string> Rules => RuleRichTextBox.Lines;

        private void RuleAdd(string value)
        {
            RuleRichTextBox.AppendText($"{value}\n");
        }

        private void RuleAddRange(IEnumerable<string> value)
        {
            foreach (string s in value)
                RuleAdd(s);
        }

        #endregion
    }
}