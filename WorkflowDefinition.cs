// --------------- Models ----------------

public record WorkflowDefinition(
    string Id,
    string Name,
    List<State> States,
    List<ActionDef> Actions
);

public class WorkflowInstance{
    public string Id { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentState { get; set; }
    public List<string> History { get; set; }

    public WorkflowInstance(string id, string workflowId, string currentState, List<string> history)
    {
        Id = id;
        WorkflowId = workflowId;
        CurrentState = currentState;
        History = history;
    }
}

public record ExecuteActionRequest(string ActionId);
