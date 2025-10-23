#if UNITY_EDITOR

namespace BonGames.EasyBuilder
{
    public interface IEasyBuilderEditor { }

    public interface IEditorWindow
    {
        public void OnFocus(IEasyBuilderEditor parent);
        public void DrawGUI(IEasyBuilderEditor parent);
    }
}
#endif