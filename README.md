https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax

# Neon

A simple ECS framework in C#

Pitch

It features:
- Entities, components and systems
- Entity copies
- Parent-child relationships
- Entities and component activation / desactivation
- Fully customizable Component queries 
- Systems ordering
- Hooks for specitic entity and component events

You can find an implementation of neon in the monogame-based engine [neongine]("https://github.com/Azathothep/neongine")

## Getting started

Use Neon.Initialize() to set the base architecture.

## Entities

Entities are base objects in your application. They are represented by the class EntityID.
To create a new Entity, simply use 

```c#
new EntityID().
```

### Activation

By default, a new entity is set active, but you can disable it using its `active` property. This will impact component query results (see Component Queries).

### Parenting

An entity can be set as parent of another one (which will become its child).
To set an entity's parent, use `public void SetParent(EntityID parent)`.

Entities can have only one parent, but any number of children.
Disabling an entity will automatically disable all of its children.

### Depth

The entity depth represent how far the entity is in the hierarchy tree. It increments by one for each parent above it.

## Components

- Component are entities (children), width flag isComponent
- Enabling
- IAwakable
- Owner

- List of functions
- Overide IComponentStorage

- Archetypes ?

## Queries

- Query
- QueryBuilder
- QueryFilters
- QueryResult

- Override IQueryStorage

## Systems

- Update & Draw
- IStartable, IStoppable
- Systems order & SystemStorage
- AllowMultipleAttribute

## Hooks

- EntityHooks
- ComponentHooks
- IHookStorage