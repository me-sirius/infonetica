var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory storage
var workflowDefinitions = new List<WorkflowDefinition>();
var workflowInstances = new List<WorkflowInstance>();

// --- Endpoints ---

// Add a workflow definition
app.MapPost("/api/workflows", (WorkflowDefinition def) => {
    workflowDefinitions.Add(def);
    return Results.Created($"/api/workflows/{def.Id}", def);
});

// Get all workflows
app.MapGet("/api/workflows", () => {
    return workflowDefinitions;
});

// Get workflow by id
app.MapGet("/api/workflows/{id}", (string id) => {
    var wf = workflowDefinitions.FirstOrDefault(w => w.Id == id);
    return wf is not null ? Results.Ok(wf) : Results.NotFound();
});

// Create a workflow instance
app.MapPost("/api/workflows/{workflowId}/instances", (string workflowId) => {
    var workflow = workflowDefinitions.FirstOrDefault(w => w.Id == workflowId);
    if (workflow == null)
        return Results.NotFound("Workflow not found.");

    var initialState = workflow.States.FirstOrDefault(s => s.IsInitial);
    if (initialState == null)
        return Results.BadRequest("No initial state defined for workflow.");

    var instanceId = Guid.NewGuid().ToString();
    var instance = new WorkflowInstance(instanceId, workflowId, initialState.Id, new List<string> { initialState.Id });
    workflowInstances.Add(instance);
    return Results.Created($"/api/instances/{instanceId}", instance);
});

// Get a workflow instance by id
app.MapGet("/api/instances/{id}", (string id) => {
    var instance = workflowInstances.FirstOrDefault(i => i.Id == id);
    return instance is not null ? Results.Ok(instance) : Results.NotFound();
});

// Move workflow instance via action
app.MapPost("/api/instances/{instanceId}/execute", (string instanceId, ExecuteActionRequest request) =>
{
    var instance = workflowInstances.FirstOrDefault(i => i.Id == instanceId);
    if (instance == null)
        return Results.NotFound("Instance not found.");

    var workflow = workflowDefinitions.FirstOrDefault(w => w.Id == instance.WorkflowId);
    if (workflow == null)
        return Results.NotFound("Workflow not found.");

    var action = workflow.Actions.FirstOrDefault(a => a.Id == request.ActionId);
    if (action == null)
        return Results.NotFound("Action not found.");

    // Only allow if in one of fromStates
    if (!action.FromStates.Contains(instance.CurrentState))
        return Results.BadRequest("Action not valid from current state.");

    // Only allow transition if not already in a final state
    var state = workflow.States.FirstOrDefault(s => s.Id == instance.CurrentState);
    if (state != null && state.IsFinal)
        return Results.BadRequest("Instance is already in a final state.");

    // Change state
    instance.CurrentState = action.ToState;
    instance.History.Add(action.ToState);

    return Results.Ok(instance);
});

app.Run();
