using UnityEngine;

namespace BonGames.EasyBuilder
{
    public abstract class BaseEditorWindow : IEditorWindow
    {
        public abstract void DrawGUI(IEasyBuilderEditor parent);

        public virtual void OnFocus(IEasyBuilderEditor parent)
        {
            
        }
    }
}
