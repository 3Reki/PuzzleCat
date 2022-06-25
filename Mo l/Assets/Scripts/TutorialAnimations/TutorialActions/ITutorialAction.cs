namespace PuzzleCat.TutorialAnimations.TutorialActions
{
    public interface ITutorialAction
    {
        public bool CanPerform();
        public void OnPerformed();
    }
}
