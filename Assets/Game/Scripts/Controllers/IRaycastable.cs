
namespace RPG.Control
{
    public interface IRaycastable {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController controller);
    }
}