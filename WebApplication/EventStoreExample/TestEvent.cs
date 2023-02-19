public class TestEvent
{
    public string EntityId { get; set; }
    public string ImportantData { get; set; }

    public override string ToString()
    {
        return $"EntityId: {EntityId}, ImportantData: {ImportantData}";
    }
}