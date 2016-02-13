using System.Globalization;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.UI;
using LiveSplit.ASL;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;

namespace LiveSplit.ASLVarViewer.UI
{
    public class ASLVarViewerUIComponent : IComponent
    {
        public string ComponentName
        {
            get { return "ASL Var Viewer"; }
        }

        private LogicComponent ASLEngine
        {
            get
            {
                return _state.Layout.Components.FirstOrDefault(
                    c => c.ComponentName == "Scriptable Auto Splitter") as LogicComponent;
            }
        }

        private ASLScript ScriptExecuting { get; set; }

        public IDictionary<string, Action> ContextMenuControls { get; protected set; }
        protected InfoTextComponent InternalComponent;

        private LiveSplitState _state;

        public ASLVarViewerUIComponent(LiveSplitState state)
        {
            this.ContextMenuControls = new Dictionary<String, Action>();
            this.InternalComponent = new InfoTextComponent("Position", "0");

            _state = state;
            _state.OnStart += _state_OnStart;
            _state.OnReset += state_OnReset;
        }

        void _state_OnStart(object sender, EventArgs e)
        {
            this.ScriptExecuting = (ASLScript)ASLEngine.GetType().GetProperty("Script").GetValue(ASLEngine, null);
        }

        public void Dispose()
        {
            _state.OnReset -= state_OnReset;
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            //string deaths = _deaths.ToString(CultureInfo.InvariantCulture);
            if (ScriptExecuting != null)
            {
                string lives = ((IDictionary<string, object>)ScriptExecuting.State.Data)["PlayerPosition"].ToString();

                if (invalidator != null && this.InternalComponent.InformationValue != lives)
                {
                    this.InternalComponent.InformationValue = lives;
                    invalidator.Invalidate(0f, 0f, width, height);
                }
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region region)
        {
            this.PrepareDraw(state);
            this.InternalComponent.DrawVertical(g, state, width, region);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region region)
        {
            this.PrepareDraw(state);
            this.InternalComponent.DrawHorizontal(g, state, height, region);
        }

        void PrepareDraw(LiveSplitState state)
        {
            this.InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            this.InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;
            this.InternalComponent.NameLabel.HasShadow
                = this.InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;
        }

        void state_OnReset(object sender, TimerPhase t)
        {
            //_deaths = 0;
            ScriptExecuting = null;
        }

        public XmlNode GetSettings(XmlDocument document) { return document.CreateElement("Settings"); }
        public Control GetSettingsControl(LayoutMode mode) { return null; }
        public void SetSettings(XmlNode settings) { }
        public void RenameComparison(string oldName, string newName) { }
        public float MinimumWidth { get { return this.InternalComponent.MinimumWidth; } }
        public float MinimumHeight { get { return this.InternalComponent.MinimumHeight; } }
        public float VerticalHeight { get { return this.InternalComponent.VerticalHeight; } }
        public float HorizontalWidth { get { return this.InternalComponent.HorizontalWidth; } }
        public float PaddingLeft { get { return this.InternalComponent.PaddingLeft; } }
        public float PaddingRight { get { return this.InternalComponent.PaddingRight; } }
        public float PaddingTop { get { return this.InternalComponent.PaddingTop; } }
        public float PaddingBottom { get { return this.InternalComponent.PaddingBottom; } }
    }
}
