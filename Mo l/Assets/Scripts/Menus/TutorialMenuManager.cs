using PuzzleCat.TutorialAnimations;
using UnityEngine;

namespace PuzzleCat.Menus
{
    public class TutorialMenuManager : MenuManager
    {
        [SerializeField] private Tutorial tutorial;
        
        public new void SwitchPortalMode()
        {
            if (!tutorial.CanChangePortalMode())
                return;
            
            base.SwitchPortalMode();
            tutorial.OnPortalModeChanged();
        }
        
        public new void UpdateSelectedPortalGroup(int index)
        {
            if (!tutorial.CanSelectPortal(index))
                return;
            
            base.UpdateSelectedPortalGroup(index);
            tutorial.OnPortalSelected();
        }
    }
}
