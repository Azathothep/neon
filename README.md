https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax

# Neon

Neon is a simple Entity Component System framework in C#.
It features:

- Entities, components and systems
- Parent-child relationships
- Entities and component activation / desactivation
- Entity copies
- Fully customizable Component queries 
- Systems ordering
- Hooks for specitic entity and component events

You can find an implementation of neon in the monogame-based engine [neongine](https://github.com/Azathothep/neongine)!

## Getting started

Clone this repository and add `neon` as a reference to your C# project.

Before calling any neon code, make sure to call `Neon.Initialize()` to initialize the base storage architecture.

## Entities

Entities are base objects in your application. They are represented by the class EntityID.
To create a new Entity, simply use:

```c#
new EntityID().
```

### Activation

By default, a new entity is set active, but you can disable it setting its `active` property to `false`. This will impact component query results (see Component Queries).

### Parenting

An entity can be set as parent of another one (which will in turn become its child).
To set an entity's parent, use
```c#
public void SetParent(EntityID parent)
```

Entities can have only one parent, but any number of children.

Disabling an entity will automatically disable all of its children.

### Depth

The entity depth represent how far the entity is in the hierarchy tree. It increments by one for each parent above it.

## Components

Components are objects attached to entities. Entities can hold any number of components, but **only one component per type**.

All components derive from the `Component` abstract class.
As a requirement, they must implement the following method that must return a copy of itself:

```c#
public Component Clone();
```

### Adding and removing components

To add a component to an entity, simply use
```c#
public Component Add<T>(T component) where T : Component;
```

Be careful, **the added component will be a copy** of the one you provided. It allows you to create a "model" component that can be added to multiple entities.

Components also have an `Add<T>(Component)` method, which will redirect the call to their owner entity. It gives you the ability to chain multiple `Add` together:

```c#
entity.Add<ComponentA>(ComponentA).Add<ComponentB>().Add<Component3>()...
```

Note that for any component that can be created with an empty constructor, it can be added using the template expression without the requiring a model component as argument.

To query for a specific component on an entity, you can use on of the following methods
```c#
public T Get<T>();
public bool TryGet<T>(out T component);
public (T1, T2) Get<T1, T2>();
public (T1, T2, T3) Get<T1, T2, T3>();
public (T1, T2, T3, T4) Get<T1, T2, T3, T4>();
public Component[] GetAll();
```

You can also query for components in parents or children

```c#
public T[] GetInChildren<T>(bool propagate = false); // By default, it will only search the components in its own children. If propagate is true, it will search the entire children hierarchy. 
public T[] GetInParents<T>();

```

To remove any component from an entity, simply use

```c#
public void Remove<T>() where T : Component
```

And finally, you can access the entity a component is attached to using the `Owner` property.

```c#
EntityID entity = myComponent.Owner;
```

### Components are also entities

Components are actually also considered as entities in neon. When a component is added to an entity, it automatically sets it as child of that entity. As a result, they can also be disabled and will automatically be if their owner entity is disabled.

### IAwakable

Components can implement the `IAwakable` interface. The implemented `Awake()` method will be called when the component is added to the entity.

## Systems

Systems are objects providing a method expected to be called periodically.

Neon provides to basic forms of systems:
- `IUpdateSystem` provide the `Update(TimeSpan timeSpan)` method, expected to be called each frame
- `IDrawSystem` provide the `Draw()` method, expected to be called before each render

### Adding and removing systems

Neon provides the `Systems` static class to work with `IUpdateSystem` and `IDrawSystem` systems.
They can be added, removed and triggered to the underlying storage using the following methods:
```c#
public static void Add(ISystem system);
public static void Remove(ISystem system);
public static void Update(TimeSpan timeSpan); // Calls the Update method on each registered system
public static void Draw(); // Calls the Draw method on each registered draw
```

Thus, your application only need to call `Systems.Update(...)` and `Systems.Draw()` at the right time to trigger the systems.

By default, a system type can only be added once, but you can allow to store multiple systems of the same type using the `AllowMultiple` attribute.
```c#
[AllowMultiple]
public class MySystem: IUpdateSystem { ... }
```

### System storage

If you need to implement other system sub-interfaces, you can use the `SystemStorage<T>` class to declare a container for those systems.

A SystemStorage give you access to an `Add` and `Remove` method, but they main interest is their support to the `Order` attribute.

### Order attribute

Sometimes, you may want to declare an order between systems without needing to add and remove them in the desired order.
Neon provides the `Order` class attribute, that allows you to specify one or more order constraints:
```c#
public OrderAttribute(OrderType type, Type targetSystem);
```

Use the `OrderType` to specify if the `targetSystem` should be triggered before or after this one.

```c#
[Order(OrderType.Before, typeof(OtherSystem))]
public class MySystem: IUpdateSystem { ... }
```

 ### Starting and Stopping systems

When using `SystemStorage` to store systems, you can also implement the `IStartable` and `IStoppable` interfaces:
```c#
public void Start(); // Called when the system is added to a SystemStorage
public void Stop(); // Called when the system is removed from a SystemStorage
```

## Queries
 
Queries are your main way to access components in batch, mainly during system updates.

A query is represented in a `Query` object. You can specify the type of components you want to request as type parameters:
```c#
Query query = new Query<ComponentA, ComponentB>(); 
```

And then, call the `QueryBuild` to get **all entities that have all of the specified components** and a reference to these components.
```c#
IEnumerable<EntityID, ComponentA, ComponentB> queryResult = QueryBuilder.Get(query, QueryType.Cached);

foreach ((EntityID id, ComponentA c1, ComponentB c2) in queryResult)
{
    // do something with the components and / or the entity
}
```

You may noticed a `QueryType` argument passed to the `QueryBuilder`, after your `Query` object.
Queries can be specified to be cached or uncached.

`QueryType.Cached` will store the query result for a much faster access later, but to the expand of memory space.

`QueryType.Uncached` will not keep the query result in-memory, but it will need to rebuild it entirely the next time it is request.

As a rule of thumb, if you plan to call your query each frame (in an Update system, for example), you better cache the query.
However if you know it won't be queried again soon, it may be better to save up some memory!

### Query result mode

The `QueryBuilder.Get` let you optionally specify the way it stores your result, using `QueryResultMode` as third parameters.

By default, the result you get from the `QueryBuilder` is a copy of the underlying storage. This is to avoid messing up the result if you add or remove components *during iteration*. As you can imagine, it is a time and memory-expensive process.

Thus, you can specify the QueryResultMode to `QueryResultMode.Unsafe` to let you iterate directly on the storage. However, only do this if you know with certainty you're not going to add or remove a component during iteration!

### Query filters

Sometimes, you may want your query to optionally include other components, or to exclude entities that contain some other components in addition to the ones requested.

This is why queries can include filters to fine-tune your request, using a `QueryFilter` object.
A `QueryFilter` take a component type as type parameter, and a term to specify the type of filter to apply:
```c#
QueryFilter filter1 = new QueryFilter<OptionalComponent>(FilterTerm.MightHave);
QueryFilter filter2 = new QueryFilter<ComponentToExclude>(FilterTerm.HasNot);
```

You can then pass filters to a query in its constructor:
```c#
Query query = new Query<ComponentA>([ filter1, filter2 ]);
```

The `FilterTerm.MightHave` is used to specify one of the type parameters to be optional. Thus, in the enumerable result of your query, the component reference might be null.

The `FilterTerm.HasNot` will specify to skip any entity that have the target component. However, if the component is also present in the type parameters of the `Query`, **it won't take the filter into account**.

```c#
Query query = new Query<ComponentA, ComponentB>( [ new QueryFilter<ComponentB>(FilterTerm.MightHave) ]); // the returned ComponentB references might be null
Query query = new Query<ComponentA>( [ new QueryFilter<ComponentA>(FilterTerm.HasNot) ]); // the filter won't be taken into account, because ComponentA is already specified as a query type parameter
```

You also have the `FilterTerm.Has`, which could be useful when requesting the entity to have a specific component without returning it in the result.
```c#
Query query = new Query<ComponentA>( [ new QueryFilter<ComponentB>(FilterTerm.Has) ]); // all returned entities are assured to also have a ComponentB
```

## Hooks

Hooks are a way to trigger and react to events that are targetted toward specific object.
When you want to react to a hook, you can use the **Hooks** static class and register the action to trigger:
```c#
public static void Add<HookID>(HookID hook, Action<object> action); // global reaction
public static void Add<HookID>(HookID hook, Action action, object target); // targetted reaction
```

### HookID

The `HookID` must be an `enum` type that contain a list of events.

Two types of Hooks are already implemented in neon:

`ComponentHook`, to react to component addition and removal
`EntityHook`, to react to entity enabling / disabling and reparenting

```c#
Hooks.Add<EntityHook>(EntityHook.OnNewChild, () => { ... }, targetEntity); // action will be triggered when the specified entity gets a new child
```

### Hook target

You can subscribe to hooks in two ways.

When specifying a third `object` argument, you target your `Action` to only be triggered when the event is called on this specific object (for example, when a specific entity is reparented).

When not specifying any target `object`, your `Action` will be triggered for any target `object`, passed as parameter to your action (for exemple, when any component is removed).

### Hook type constraint

Suppose you can to create a hook that is only scoped to a certain component *type*. To achieve this, `Hooks` also provide type-constrainted hooks:
```c#
// Replace T with your type constraint
public static void Add<HookID, T>(HookID hook, Action action, object target);
public static void Add<HookID, T>(HookID hook, Action<object> action);
```

For example, the following action will be triggered when any `ComponentA` component is added to an entity
```c#
Hooks.Add<ComponentHook, ComponentA>(ComponentHook.OnAdded, (component) => { ... });
```

### Hook triggers

If you want to create your own hooks, you can use `Hooks.Create`
```c#
public static HookTrigger<HookID> Create<HookID>(Type additionalType = null); // optionally specify a type constraint as parameter
public static HookTrigger<HookID> Create<HookID, T>();
```

Hooks cannot be triggered from anywhere. This is why you get a `HookTrigger`. You can then use the following method to raise the event:
```c#
public void Raise(HookID hook, object target);
```