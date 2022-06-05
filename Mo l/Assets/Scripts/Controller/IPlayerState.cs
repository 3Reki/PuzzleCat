namespace PuzzleCat.Controller
{
    public interface IPlayerState
    {
        public void Enter();
        public IPlayerState Handle();
        public void Exit();
    }
}
