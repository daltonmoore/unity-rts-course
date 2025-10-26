using Unity.Behavior;

namespace Units
{
    [BlackboardEnum]
    public enum BuildingEventType
    {
        ArrivedAt,
        Begin,
        Cancel,
        Abort,
        Completed
    }
}