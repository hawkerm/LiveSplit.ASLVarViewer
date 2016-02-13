using System.Reflection;
using LiveSplit.ASLVarViewer.UI;
using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

[assembly: ComponentFactory(typeof(ASLVarViewerUIFactory))]

namespace LiveSplit.ASLVarViewer.UI
{
    public class ASLVarViewerUIFactory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "ASL Var Viewer"; }
        }

        public string Description
        {
            get { return "Configurable component to display any values from an ASL script."; }
        }

        public ComponentCategory Category
        {
            get { return ComponentCategory.Information; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new ASLVarViewerUIComponent(state);
        }

        public string UpdateName
        {
            get { return this.ComponentName; }
        }

        public string UpdateURL
        {
            get { return ""; } // http://fatalis.pw/livesplit/update/
        }

        public Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public string XMLURL
        {
            get { return ""; } // this.UpdateURL + "Components/update.LiveSplit.HaloSplit.UI.xml"
        }
    }
}
