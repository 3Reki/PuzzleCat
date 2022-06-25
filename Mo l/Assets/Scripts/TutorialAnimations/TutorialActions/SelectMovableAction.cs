using System;

namespace PuzzleCat.TutorialAnimations.TutorialActions
{
    [Serializable]
    public class SelectMovableAction : ITutorialAction
    {
        public int intField;
        
        public bool CanPerform()
        {
            throw new System.NotImplementedException();
        }

        public void OnPerformed()
        {
            throw new System.NotImplementedException();
        }
    }
}
