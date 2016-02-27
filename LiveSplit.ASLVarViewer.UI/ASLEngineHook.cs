using LiveSplit.ASL;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.ASLVarViewer.UI
{
    // LiveSplitState
    // ASLEngine (Logic Component) [only after Layout loaded]
    // ASLScript
    // State.Data/Vars ExpandoObjects

    /*
     * Component to handle integration with ASLEngine component and retrieving values from an ASL Script component.
     * 
     * Values returned will be blank if ASL not loaded.
     */
    public class ASLEngineHook
    {
        private LiveSplitState State { get; set; }

        private LogicComponent aslEngine;
        public LogicComponent ASLEngine
        {
            get
            {
                if (aslEngine == null)
                {
                    // If we have an active auto-downloaded autosplitter, use that, otherwise look for a layout component
                    if (State.Run != null &&
                        State.Run.AutoSplitter != null &&
                        State.Run.AutoSplitter.Type == AutoSplitterType.Script &&
                        State.Run.IsAutoSplitterActive())
                    {
                        aslEngine = State.Run.AutoSplitter.Component as LogicComponent; // TODO: For 1.7 compatible only release, case to ASLComponent
                    }
                    else
                    {
                        aslEngine = State.Layout.Components.FirstOrDefault(c => c.ComponentName == "Scriptable Auto Splitter") as LogicComponent;
                    }
                }

                return aslEngine;
            }
        }

        private ASLScript aslScript;
        public ASLScript ASLScript
        {
            get
            {
                // TODO: Handle case where Script objects gets recreated, compare HashCode?
                if (ASLEngine != null && aslScript == null)
                {
                    aslScript = (ASLScript)aslEngine.GetType().GetProperty("Script").GetValue(aslEngine, null);
                }
                
                return aslScript; 
            }
        }

        /// <summary>
        /// Will not load components, but will return false if not hooked in.
        /// 
        /// Call <see cref="AttemptLoad"/> to try loading components.
        /// </summary>
        public bool IsLoaded
        {
            get { return aslEngine != null && aslScript != null; }
        }

        public ASLEngineHook(LiveSplitState state)
        {
            this.State = state;
        }

        public bool AttemptLoad()
        {
            // TODO: Maybe make this piece check for a reload instead?
            bool loaded = ASLEngine != null;
            loaded &= ASLScript != null;

            return loaded;
        }

        public void Unload()
        {
            this.aslEngine = null;
            this.aslScript = null;
        }

        public string GetStateValue(string key)
        {
            if (ASLScript != null && ASLScript.State != null && key != null &&
                ((IDictionary<string, object>)ASLScript.State.Data).ContainsKey(key))
            {
                return "" + ((IDictionary<string, object>)ASLScript.State.Data)[key];
            }
            
            return string.Empty;
        }

        public string GetVariableValue(string key)
        {
            if (ASLScript != null && key != null &&
                ((IDictionary<string, object>)ASLScript.Vars).ContainsKey(key))
            {
                return "" + ((IDictionary<string, object>)ASLScript.Vars)[key];
            }

            return string.Empty;
        }

        public string[] GetStateKeys()
        {
            if (ASLScript == null || ASLScript.State == null)
            {
                return new string[] {};
            }

            return ((IDictionary<string, object>)ASLScript.State.Data).Keys.ToArray();
        }

        public string[] GetVariableKeys()
        {
            if (ASLScript == null)
            {
                return new string[] { };
            }

            return ((IDictionary<string, object>)ASLScript.Vars).Keys.ToArray();
        }
    }
}
