#if UNITY_EDITOR

namespace BonGames.EasyBuilder
{
    public interface IEasyBuilderEditor { }

    public interface IEditorWindow
    {
        public void DrawGUI(IEasyBuilderEditor parent);
    }
}
#endif