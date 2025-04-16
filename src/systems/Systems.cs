using System.Diagnostics;
using System.Reflection;

namespace neon
{
    public class Systems
    {
        private class SystemsStorage<T> where T : IGameSystem
        {
            public class OrderTree {
                private class Node {
                    public Type System;
                    public Node Parent;
                    public List<Node> Children = new();

                    public Node(Type system) {
                        System = system;
                    }
                }

                private Node m_Root;

                private Dictionary<Type, Node> m_TypeToNode = new();
                private Dictionary<Node, HashSet<Node>> m_Dependencies = new();
                
                private List<Type> m_TypeOrder = new();

                public OrderTree() {
                    m_Root = new Node(null);
                }

                public void AddAfter(Type system, Type precedingSystem) {
                    Node precedingNode = precedingSystem != null ? m_TypeToNode.GetValueOrDefault(precedingSystem) : m_Root;
                    HashSet<Node> dependencies;
                    Node node;

                    if (m_TypeToNode.TryGetValue(system, out node)) {                        
                        if (precedingNode == null) // system isn't in hierarchy
                            return;
                        
                        if (precedingNode != m_Root) {
                            dependencies = GetOrCreateValue(node, m_Dependencies);
                            if (dependencies.Contains(precedingNode) == false)
                                dependencies.Add(precedingNode);
                        }

                        if (IsChildOf(node, precedingNode)) { // Is already a child of
                            return;
                        }

                        if (IsChildOf(precedingNode, node)) // target system to go after is a child of current
                            throw new Exception($"Error : {node.System} cannot go after its child {precedingNode.System}");

                        MoveAfter(node, precedingNode);
                        return;
                    }

                    node = new Node(system);
                    m_TypeToNode.Add(system, node);

                    if (precedingNode == null)
                        precedingNode = m_Root;
                    else if (precedingNode != m_Root) {
                        dependencies = GetOrCreateValue(node, m_Dependencies);
                        dependencies.Add(precedingNode);
                    }

                    precedingNode.Children.Add(node);
                    node.Parent = precedingNode;

                    int precedingNodeIndex = m_TypeOrder.FindIndex((t) => t == precedingSystem);
                    
                    if (precedingNodeIndex == -1)
                        m_TypeOrder.Add(system);
                    else
                        AddOrInsert(system, m_TypeOrder, precedingNodeIndex + 1);

                    return;
                }

                public void AddBefore(Type system, Type followingSystem) {
                    if (Contains(followingSystem) == false)
                        return;

                    AddAfter(followingSystem, system);
                }

                public void Remove(Type systemType) {
                    Node node = m_TypeToNode[systemType];

                    List<Node> children = node.Children;

                    foreach (var child in children) {
                        // check their dependencies
                        // if they have others : move to the closest one

                        HashSet<Node> dependencies = RemoveDependenciesOnNode(child, node);

                        if (dependencies.Count == 0) {
                            Retrograde(child, m_Root);
                            continue;
                        }

                        int minDistance = int.MaxValue;
                        Node closestDependency = null;
                        foreach (var dependency in dependencies) {
                            int distance = GetParentDistance(child, dependency);
                            if (distance > 0 && distance < minDistance) {
                                minDistance = distance;
                                closestDependency = dependency;
                            }
                        }

                        if (closestDependency != null) {
                            Retrograde(child, closestDependency);
                            continue;
                        }

                        Retrograde(child, m_Root);
                    }

                    node.Parent.Children.Remove(node);
                    node.Parent = null;
                    node.Children = new();
                    m_TypeToNode.Remove(systemType);
                    m_TypeOrder.Remove(systemType);
                    m_Dependencies.Remove(node);
                }

                private HashSet<Node> RemoveDependenciesOnNode(Node from, Node dependency) {
                    if (m_Dependencies.TryGetValue(from, out HashSet<Node> dependencies)) {
                        dependencies.Remove(dependency);
                    }

                    foreach (var child in from.Children)
                        RemoveDependenciesOnNode(child, dependency);

                    return dependencies;
                }

                public bool Contains(Type system) => m_TypeToNode.ContainsKey(system);

                public List<Type> ToList() => m_TypeOrder;

                private void AddToList(Node node, List<Type> list) {
                    list.Add(node.System);

                    foreach (var child in node.Children)
                        AddToList(child, list);
                }

                private bool IsChildOf(Node evaluatedNode, Node targetParentNode) {
                    if (evaluatedNode == null)
                        return false;
                    
                    if (evaluatedNode.Parent == targetParentNode)
                        return true;

                    return IsChildOf(evaluatedNode.Parent, targetParentNode);
                }

                private int GetParentDistance(Node node, Node parentNode) {
                    int i = 1;
                    Node currentNode = node;

                    while (currentNode != m_Root) {
                        if (currentNode.Parent == parentNode)
                            return i;

                        currentNode = currentNode.Parent;
                        i++;
                    }

                    return -1;
                }

                private void Retrograde(Node node, Node precedingNode) {
                    precedingNode.Children.Add(node);
                    node.Parent = precedingNode;

                    List<Type> range = ExtractBranch(node);

                    int append = 0;
                    if (precedingNode != m_Root)
                        append = m_TypeOrder.FindIndex((t) => t == precedingNode.System) + 1;

                    if (append == m_TypeOrder.Count)
                        m_TypeOrder.AddRange(range);
                    else
                        m_TypeOrder.InsertRange(append, range);
                }

                private void MoveAfter(Node node, Node precedingNode) {
                    Node nodeToCutTo = GetChildOfCommonAncestor(node, precedingNode);

                    precedingNode.Children.Add(nodeToCutTo);
                    nodeToCutTo.Parent = precedingNode;
                    
                    List<Type> range = ExtractBranch(node);

                    int precedingNodePosition = m_TypeOrder.FindIndex((t) => t == precedingNode.System);
                    
                    if (precedingNodePosition + 1 == m_TypeOrder.Count)
                        m_TypeOrder.AddRange(range);
                    else
                        m_TypeOrder.InsertRange(precedingNodePosition + 1, range);
                }

                private List<Type> ExtractBranch(Node node) {
                    int start = m_TypeOrder.FindIndex((t) => t == node.System);
                    int count = ChildCount(node) + 1;
                    List<Type> range = m_TypeOrder.GetRange(start, count);
                    m_TypeOrder.RemoveRange(start, count);

                    return range;
                }

                public Type GetPrecedingType(Type type) {
                    if (m_TypeToNode.TryGetValue(type, out Node node) == false) {
                        throw new Exception($"Type {type} not found in tree, ensure it has been added before calling GetPrecedingType");
                    }

                    int index = m_TypeOrder.FindIndex((i) => i == type);
                    if (index - 1 < 0)
                        return null;
                    
                    return m_TypeOrder[index - 1];
                }

                private Node GetChildOfCommonAncestor(Node node1, Node node2) {
                    Node currentChild = node1;
                    Node parent = node1.Parent;

                    while (parent != null) {
                        if (IsChildOf(node2, parent))
                            break;

                        currentChild = parent;
                        parent = parent.Parent;
                    }

                    return currentChild;
                }

                private int ChildCount(Node node) {
                    Debug.WriteLine($"Checking node count of {node.System}");
                    int count = node.Children.Count;

                    foreach (var child in node.Children) {
                        count += ChildCount(child);
                    }

                    return count;
                }

                private U GetOrCreateValue<V, U>(V key, Dictionary<V, U> dict) where U : new() {
                if (dict.TryGetValue(key, out U item) == false) {
                    item = new();
                    dict.Add(key, item);
                }

                return item;
            }
            }

            private List<T> m_Systems = new();
            public List<T> Systems => m_Systems;

            private Dictionary<Type, HashSet<Type>> m_SystemsComingBefore = new();
            private Dictionary<Type, HashSet<Type>> m_SystemsComingAfter = new();

            private Dictionary<Type, List<T>> m_SystemsByType = new();

            public OrderTree m_OrderTree = new();

            public void Add(T system) {
                Type type = system.GetType();
            
                if (m_OrderTree.Contains(type)) {
                    var systems = m_SystemsByType[type];

                    systems.Add(system);

                    int index = m_Systems.FindLastIndex((s) => s.GetType() == type);

                    AddOrInsert(system, m_Systems, index + 1);

                    return;
                }
                
                IEnumerable<OrderAttribute> predicates = system.GetType().GetCustomAttributes<OrderAttribute>();
                if (predicates.Count() > 0)
                {
                    RegisterPredicates(type, predicates);
                }

                AddToStorage(system);
            }

            public void Remove(T system) {
                m_Systems.Remove(system);
                
                Type type = system.GetType();
                
                if (m_SystemsByType.TryGetValue(type, out List<T> systems)) {
                    systems.Remove(system);

                    if (systems.Count == 0) {
                        m_SystemsByType.Remove(type);
                        m_OrderTree.Remove(type);

                        foreach (var item in m_OrderTree.ToList())
                            Debug.WriteLine($"Remove reorder : {item}");

                        ReorderStorage();
                    }
                }
            }

            private static void AddOrInsert<U>(U item, List<U> list, int index) {
                if (index >= list.Count) {
                    list.Add(item);
                    return;
                }
                
                list.Insert(index, item);
            }

            private void RegisterPredicates(Type type, IEnumerable<OrderAttribute> predicates) {
                Type templateType = typeof(T);
                
                foreach (OrderAttribute predicate in predicates) {                    
                    if (!templateType.IsAssignableFrom(predicate.TargetSystem)) {
                        continue;
                    }

                    if (predicate.Type == OrderType.After) {
                        HashSet<Type> systemsComingBefore = GetOrCreateValue(type, m_SystemsComingBefore);
                        systemsComingBefore.Add(predicate.TargetSystem);

                        HashSet<Type> systemsComingAfter = GetOrCreateValue(predicate.TargetSystem, m_SystemsComingAfter);
                        systemsComingAfter.Add(type);
                    } else { // Before
                        HashSet<Type> systemsComingAfter = GetOrCreateValue(type, m_SystemsComingAfter);
                        systemsComingAfter.Add(predicate.TargetSystem);
                        
                        HashSet<Type> systemsComingBefore = GetOrCreateValue(predicate.TargetSystem, m_SystemsComingBefore);
                        systemsComingBefore.Add(type);
                    }
                }
            }

            private void AddToStorage(T system) {                
                Type type = system.GetType();

                if (m_SystemsComingBefore.TryGetValue(type, out HashSet<Type> systemsComingBefore)) {

                    foreach (var precedingSystem in systemsComingBefore)
                        m_OrderTree.AddAfter(type, precedingSystem);

                } else {
                    // If not systems coming before, it won't be added to the tree
                    // so we must add it to root
                    m_OrderTree.AddAfter(type, null);
                }

                if (m_SystemsComingAfter.TryGetValue(type, out HashSet<Type> systemsComingAfter)) {

                    foreach (var followingSystem in systemsComingAfter)
                        m_OrderTree.AddBefore(type, followingSystem);
                    
                }

                List<T> systems = GetOrCreateValue(type, m_SystemsByType);
                systems.Add(system);

                ReorderStorage();
            }

            public void ReorderStorage() {
                m_Systems.Clear();

                List<Type> order = m_OrderTree.ToList();

                foreach (var type in order) {
                    if (m_SystemsByType.TryGetValue(type, out List<T> systems)) {
                        m_Systems.AddRange(systems);
                    }
                }
            }

            private U GetOrCreateValue<U>(Type key, Dictionary<Type, U> dict) where U : new() {
                if (dict.TryGetValue(key, out U item) == false) {
                    item = new();
                    dict.Add(key, item);
                }

                return item;
            }
        }

        private class Storage {
            public SystemsStorage<IUpdateSystem> Update = new();
            public SystemsStorage<IDrawSystem> Draw = new();
        }

        private static Storage storage;

        private Systems() { }

        public static void Initialize() {
            storage = new();
        }

        public static void Add(IGameSystem gameSystem)
        {
            Type type = gameSystem.GetType();
            bool unique = type.GetCustomAttribute<AllowMultipleAttribute>() == null;

            if (unique && (storage.Update.Systems.FirstOrDefault(s => s.GetType() == type) != null
                || storage.Draw.Systems.FirstOrDefault(s => s.GetType() == type) != null))
            {
                Debug.WriteLine($"{type} has been added multiple times but is a unique system. Skipping.");
                return;
            }

            if (gameSystem is IUpdateSystem updateSystem)
                storage.Update.Add(updateSystem);

            if (gameSystem is IDrawSystem drawSystem)
                storage.Draw.Add(drawSystem);
        
        }

        public static void Remove(IGameSystem gameSystem)
        {
            if (gameSystem is IUpdateSystem updateSystem)
                storage.Update.Remove(updateSystem);

            if (gameSystem is IDrawSystem drawSystem)
                storage.Draw.Remove(drawSystem);
        }

        public static IGameSystem[] GetLoadedSystems()
        {
            List<IGameSystem> gameSystems = [.. storage.Update.Systems];

            foreach (IGameSystem system in storage.Draw.Systems)
            {
                if (gameSystems.Contains(system))
                    continue;

                gameSystems.Add(system);
            }

            return gameSystems.ToArray();
        }

        public static void Update(TimeSpan timeSpan)
        {
            for (int i = 0; i < storage.Update.Systems.Count; i++)
            {
                storage.Update.Systems[i].Update(timeSpan);
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < storage.Draw.Systems.Count; i++)
            {
                storage.Draw.Systems[i].Draw();
            }
        }
    }
}
