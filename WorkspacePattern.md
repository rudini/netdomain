# The Workspace pattern #

The IWorkspace is an abstraction of the used persistence framework and an implementation of the Unit of Work pattern.
The concept is to let your Repositories (see Repositories page) only talk to the Workspace abstraction, making it possible:
  * To swap the underlying persistence framework.
  * To use the persistence framework in a unified way.

Here's a code sample of a small unit of work implementation:

```
using (ObjectContext ctx = new MyObjectContext())
using (IWorkspace ws = new LinqToEntitiesWorkspace(ctx))
{
  var person = ws.CreateQuery<Person>().Where(p => p.Id == id).Single();
  person.Name = "changedName";
  ws.SubmitChanges();
}
```

To access the underlying persistence framework you can use the generic IWorkspace interface e.g.

```
IWorkspace ws = new LinqToEntitiesWorkspace(new MyObjectContext());
IWorkspace<ObjectContext> typedWs = IWorkspace<ObjectContext>() ws;
ObjectContext ctx = typedWs.WrappedInstance;
```


# The WorkspaceScope #

When it comes to refactoring of duplicated code, the problem is often how to pass the current Workspace to a method used by many consumers.

The simplest way to do that is as follows:

```
public Customer GetCustomer(int id)
{
  using (var scope = new WorkspaceScope(WorkspaceScopeOption.Required))
  {
    return scope.CurrentWorkspace.CreateQuery<Customer>().Where(c => c.Id == id);
  }
}
```

| **WorkspaceOption** | **Description** |
|:--------------------|:----------------|
| Required | A workspace is required by the scope. It uses an ambient workspace if one already exists. <br /> Otherwise, it creates a new workspace before entering the scope. This is the default value. |
| RequiresNew |  A new workspace is always created for the scope. <br /> This is usefull if you don't want to cache the result of the query in your id map / tracking manager |

| The WorkspaceScope provides different constructor overloads. For more information see the API documentation. |
|:-------------------------------------------------------------------------------------------------------------|

To use the WorkspaceScope, you have to register a WorkspaceFactory before.
You can do that in the following ways:

```
// registers a workspace factory as an instance ! Not thread safe
WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory(workspaceFactoryMock.Object);
```

```
// registers a workspace factory by a type.
WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory<MyWorkspaceFactory>();
```