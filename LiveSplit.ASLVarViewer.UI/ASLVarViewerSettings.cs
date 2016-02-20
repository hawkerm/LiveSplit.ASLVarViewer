using LiveSplit.ASL;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.ASLVarViewer.UI
{
    public partial class ASLVarViewerSettings : UserControl
    {
        public enum ValueBucket
        {
            CurrentState,
            Variables
        }

        public Color TextColor { get; set; }
        public bool OverrideTextColor { get; set; }
        public string TextLabel { get; set; }

        public ValueBucket ValueLocation { get; set; }
        public string ValueSource { get; set; }
        public Color ValueColor { get; set; }
        public bool OverrideValueColor { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public bool Display2Rows { get; set; }

        public LayoutMode Mode { get; set; }

        private ASLEngineHook EngineHook { get; set; }

        public event EventHandler ASLVarViewerLayoutChanged;

        public ASLVarViewerSettings(ASLEngineHook ASLEngine)
        {
            this.EngineHook = ASLEngine;

            InitializeComponent();

            TextColor = Color.FromArgb(255, 255, 255);
            OverrideTextColor = false;
            ValueColor = Color.FromArgb(255, 255, 255);
            OverrideValueColor = false;
            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = GradientType.Plain;
            Display2Rows = false;

            txtLabel.DataBindings.Add("Text", this, "TextLabel", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);

            chkOverrideTimeColor.DataBindings.Add("Checked", this, "OverrideValueColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTimeColor.DataBindings.Add("BackColor", this, "ValueColor", false, DataSourceUpdateMode.OnPropertyChanged);
            comboValue.DataBindings.Add("Text", this, "ValueSource", false, DataSourceUpdateMode.OnPropertyChanged);

            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void chkOverrideTimeColor_CheckedChanged(object sender, EventArgs e)
        {
            label2.Enabled = btnTimeColor.Enabled = chkOverrideTimeColor.Checked;
        }

        void chkOverrideTextColor_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = btnTextColor.Enabled = chkOverrideTextColor.Checked;
        }

        void ASLVarViewerSettings_Load(object sender, EventArgs e)
        {
            if (!this.EngineHook.AttemptLoad())
            {
                MessageBox.Show("Cannot connect to AutoSplit Engine.\n\nPlease add a 'Scriptable Auto Splitter' component first with an Active Script before configuring this component.", "AutoSplit Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Enabled = false;
                return;
            }

            this.radioState.Checked = (ValueLocation == ValueBucket.CurrentState);
            this.radioVariables.Checked = (ValueLocation == ValueBucket.Variables);
            this.UpdateLocation();

            chkOverrideTextColor_CheckedChanged(null, null);
            chkOverrideTimeColor_CheckedChanged(null, null);
            if (Mode == LayoutMode.Horizontal)
            {
                chkTwoRows.Enabled = false;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.Checked = true;
            }
            else
            {
                chkTwoRows.Enabled = true;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            TextLabel = SettingsHelper.ParseString(element["TextLabel"]);
            TextColor = SettingsHelper.ParseColor(element["TextColor"]);
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);

            ValueColor = SettingsHelper.ParseColor(element["ValueColor"]);
            OverrideValueColor = SettingsHelper.ParseBool(element["OverrideValueColor"]);
            ValueLocation = SettingsHelper.ParseEnum<ValueBucket>(element["ValueLocation"]);
            ValueSource = SettingsHelper.ParseString(element["ValueSource"]);

            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"]);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"]);
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"]);
            Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);

            if (ASLVarViewerLayoutChanged != null)
            {
                ASLVarViewerLayoutChanged(this, null);
            }
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.0") ^
            SettingsHelper.CreateSetting(document, parent, "TextLabel", TextLabel) ^
            SettingsHelper.CreateSetting(document, parent, "TextColor", TextColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^

            SettingsHelper.CreateSetting(document, parent, "ValueColor", ValueColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideValueColor", OverrideValueColor) ^
            SettingsHelper.CreateSetting(document, parent, "ValueLocation", ValueLocation) ^
            SettingsHelper.CreateSetting(document, parent, "ValueSource", ValueSource) ^

            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
            SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateLocation();
        }

        private void UpdateLocation()
        {
            if (radioState.Checked)
            {
                ValueLocation = ValueBucket.CurrentState;
            }
            else
            {
                ValueLocation = ValueBucket.Variables;
            }

            if (this.EngineHook.IsLoaded)
            {
                comboValue.Items.Clear();
                if (ValueLocation == ValueBucket.CurrentState)
                {
                    comboValue.Items.AddRange(this.EngineHook.GetStateKeys());
                }
                else
                {
                    comboValue.Items.AddRange(this.EngineHook.GetVariableKeys());
                }

                if (comboValue.Items.Count == 0)
                {
                    this.radioState.Enabled = false;
                    this.radioVariables.Enabled = false;
                    this.comboValue.Enabled = false;
                    this.groupBoxValue.Text = "Value [Please Load ASL Script or Start Game]";
                }
                else
                {
                    this.groupBoxValue.Text = "Value";
                    this.radioState.Enabled = true;
                    this.radioVariables.Enabled = true;
                    this.comboValue.Enabled = true;
                }
            }
        }

        private void txtLabel_Validated(object sender, EventArgs e)
        {
            if (ASLVarViewerLayoutChanged != null)
            {
                ASLVarViewerLayoutChanged(this, null);
            }
        }

        private void chkTwoRows_CheckedChanged(object sender, EventArgs e)
        {
            if (ASLVarViewerLayoutChanged != null)
            {
                ASLVarViewerLayoutChanged(this, null);
            }
        }
    }
}
