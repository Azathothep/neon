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
entity.Add<Component1>(component1).Add<Component2>().Add<Component3>()...
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

However, as long as your object implements the empty `ISystem` interface, it can be added to the System storage. Don't hesitate to create your own sub-interfaces if you need to. 

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

Queries are your main way to access components in groups, with conditions.

- Query
- QueryBuilder
- QueryFilters
- QueryResult

- Override IQueryStorage

## Hooks

- EntityHooks
- ComponentHooks
- IHookStorage