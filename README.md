# WorkflowEngine

An in-memory, minimal API-based workflow engine backend in ASP.NET Core.

---

## 🚀 Quick Start

**Prerequisites:**  
- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/en-us/download)

**1. Clone the repository**
git clone https://github.com/me-sirius/infonetica.git
cd infonetica/WorkflowEngine

**2. Build the project**
dotnet build

**3. Run the application**
dotnet run

By default, the API will run on `http://localhost:5116` (or another port as displayed in terminal).

---

## 🧑‍💻 Workflow Example (using [Postman](https://www.postman.com/) or `curl`)

### 1. Define a workflow
curl -X POST http://localhost:5116/api/workflows
-H "Content-Type: application/json"
-d '{
"id": "document-approval",
"name": "Document Approval Workflow",
"states": [
{ "id": "draft", "name": "Draft", "isInitial": true, "isFinal": false },
{ "id": "review", "name": "Under Review", "isInitial": false, "isFinal": false },
{ "id": "approved", "name": "Approved", "isInitial": false, "isFinal": true }
],
"actions": [
{ "id": "submit", "name": "Submit", "fromStates": ["draft"], "toState": "review" },
{ "id": "approve", "name": "Approve", "fromStates": ["review"], "toState": "approved" }
]
}'

### 2. Start a workflow instance
curl -X POST http://localhost:5116/api/workflows/document-approval/instances

_Response will include your instance id._

### 3. Transition state for the instance
curl -X POST http://localhost:5116/api/instances/{instanceId}/execute
-H "Content-Type: application/json"
-d '{"actionId": "submit"}'

(_Replace `{instanceId}` with the id from the previous step._)

---

## 🛠️ API Endpoints

| HTTP Method | URL                                                 | Purpose                            |
|-------------|-----------------------------------------------------|------------------------------------|
| POST        | /api/workflows                                      | Create a workflow definition       |
| GET         | /api/workflows                                      | List all workflows                 |
| GET         | /api/workflows/{id}                                 | Get workflow by id                 |
| POST        | /api/workflows/{workflowId}/instances               | Create an instance of a workflow   |
| GET         | /api/instances/{instanceId}                         | Get an instance by id              |
| POST        | /api/instances/{instanceId}/execute                 | Execute an action on instance      |

---

## 📄 Assumptions & Known Limitations

- **In-memory storage:** All workflow definitions and instances are stored in process memory.  
  **Data will be lost if the application is stopped or restarted.**
- **No concurrency control:** Not thread-safe for multiple users at once.
- **No authentication or authorization.**
- **API only**: No frontend or OpenAPI/Swagger UI.
- **Basic validation** is included, but not production-grade.

---

## 🏗 Environment and Build Notes

- Tested with .NET 6.0/7.0 SDK on macOS and Windows.
- To change the port, update `launchSettings.json` or pass `--urls` parameter to `dotnet run`.
- **CORS:** If you add a frontend, configure CORS as needed.

---

## 🧩 Project Structure

WorkflowEngine/
├── Program.cs
├── State.cs
├── ActionDef.cs
├── WorkflowDefinition.cs
├── WorkflowInstance.cs
└── ExecuteActionRequest.cs

---

## ✏️ Shortcuts

- All business logic is implemented in `Program.cs` for simplicity.
- Models defined as C# `record` or `class` types.
- No persistent database integration.
- No automated test suite included.

---

## 🔗 License

MIT or as per your requirements.

---

## 💬 Feedback

For bugs or suggestions, open an issue or create a pull request!
