
namespace AmusedGroupAssignment
{
    public class ScenarioDetails
    {
        public string? Parameters { get; set; }
        public Dictionary<string, object>? Body { get; set; }
    }

    public class Scenarios
    {
        public ScenarioDetails? SingleItem { get; set; }
        public ScenarioDetails? MultipleItems { get; set; }
        public ScenarioDetails? AddItem { get; set; }
        public ScenarioDetails? UpdateItem { get; set; }
        public ScenarioDetails? DeleteItem { get; set; }
    }

    public class RootObject
    {
        public required string Endpoint { get; set; }
        public required Scenarios Scenarios { get; set; }
    }
}
