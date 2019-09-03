namespace RPG.Control
{
    public interface IRayCastable
    {
        bool HandleRaycast(PlayerController callingController);
        CursorType GetCursorType();
    }
}
