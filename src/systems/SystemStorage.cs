using System.Diagnostics;
using System.Reflection;

namespace neon {
    public class SystemStorage<T> where T : IGameSystem
    {
        private class OrderTree {
            private class Node {
                public Type System;
                public Node Parent;
                public HashSet<Node> Children = new();

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
                HashSet<Node> dependencies;
                Node node;
                Node? precedingNode = m_Root;
                
                if (precedingSystem != null)
                    precedingNode = m_TypeToNode.GetValueOrDefault(precedingSystem);

                if (m_TypeToNode.TryGetValue(system, out node)) {                        
                    ProcessNewDependance(node, precedingNode);
                } else {
                    CreateNewNode(system, precedingNode);
                }
            }

            public void AddBefore(Type system, Type followingSystem) {
                if (Contains(followingSystem) == false)
                    return;

                AddAfter(followingSystem, system);
            }

            private void ProcessNewDependance(Node node, Node precedingNode) {
                if (precedingNode == null) // preceding system isn't in hierarchy
                        return;
                    
                if (precedingNode != m_Root) {
                    HashSet<Node> dependencies = m_Dependencies.GetOrCreateValue(node);
                    dependencies.Add(precedingNode);
                }

                if (IsChildOf(node, precedingNode)) { // Is already a child of
                    return;
                }

                if (IsChildOf(precedingNode, node)) // target system to go after is a child of current
                    throw new Exception($"Error : {node.System} cannot go after its child {precedingNode.System}");

                Node nodeToCutTo = FindNodeToCut(node, precedingNode);
                MoveAfter(nodeToCutTo, precedingNode);
            }

            private void CreateNewNode(Type system, Node precedingNode) {
                Node node = new Node(system);
                m_TypeToNode.Add(system, node);

                if (precedingNode == null)
                    precedingNode = m_Root;
                else if (precedingNode != m_Root) {
                    HashSet<Node> dependencies = m_Dependencies.GetOrCreateValue(node);
                    dependencies.Add(precedingNode);
                }

                SetRelation(precedingNode, node);

                int insertPosition = m_TypeOrder.FindIndex((t) => t == precedingNode.System) + 1;
                m_TypeOrder.InsertOrAppend(insertPosition, system);
            }

            public void Remove(Type systemType) {
                Node node = m_TypeToNode[systemType];

                HashSet<Node> children = node.Children;

                foreach (var child in children) {
                    // check their dependencies
                    // if they have others : move to the closest one

                    RemoveDependency(child, node);
                }

                node.Parent.Children.Remove(node);
                node.Parent = null;
                node.Children = null;
                m_Dependencies.Remove(node);
                m_TypeToNode.Remove(systemType);
                m_TypeOrder.Remove(systemType);
            }

            private void RemoveDependency(Node node, Node dependency) {
                HashSet<Node> dependencies = RemoveDependencyRecursive(node, dependency);

                if (dependencies.Count == 0) {
                    MoveAfter(node, m_Root);
                    return;
                }

                int minDistance = int.MaxValue;
                Node closestDependency = null;
                foreach (var d in dependencies) {
                    int distance = GetFamilyDistance(node, d);
                    if (distance > 0 && distance < minDistance) {
                        minDistance = distance;
                        closestDependency = d;
                    }
                }

                if (closestDependency != null) {
                    MoveAfter(node, closestDependency);
                    return;
                }

                MoveAfter(node, m_Root);
            }

            private HashSet<Node>? RemoveDependencyRecursive(Node from, Node dependency) {
                if (m_Dependencies.TryGetValue(from, out HashSet<Node>? dependencies)) {
                    dependencies.Remove(dependency);
                }

                foreach (var child in from.Children)
                    RemoveDependencyRecursive(child, dependency);

                return dependencies;
            }

            public bool Contains(Type system) => m_TypeToNode.ContainsKey(system);

            public List<Type> ToList() => m_TypeOrder;

            private bool IsChildOf(Node evaluatedNode, Node targetParentNode) {
                if (evaluatedNode == null)
                    return false;
                
                if (evaluatedNode.Parent == targetParentNode)
                    return true;

                return IsChildOf(evaluatedNode.Parent, targetParentNode);
            }

            private int GetFamilyDistance(Node node, Node parentNode) {
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

            private void MoveAfter(Node node, Node precedingNode) {
                SetRelation(precedingNode, node);
                
                List<Type> range = ExtractBranch(node);

                int insertPosition = 0;
                if (precedingNode != m_Root)
                    insertPosition = m_TypeOrder.FindIndex((t) => t == precedingNode.System) + 1;

                m_TypeOrder.InsertOrAppendRange(insertPosition, range);
            }

            private Node FindNodeToCut(Node node, Node dependency) {
                Node nodeToCutTo = node;
                Node nodeParent = node.Parent;

                while (nodeParent != null) {
                    if (IsChildOf(dependency, nodeParent))
                        break;

                    nodeToCutTo = nodeParent;
                    nodeParent = nodeParent.Parent;
                }

                return nodeToCutTo;
            }

            private void SetRelation(Node parent, Node child) {
                parent.Children.Add(child);
                child.Parent = parent;
            }

            private List<Type> ExtractBranch(Node node) {
                int start = m_TypeOrder.FindIndex((t) => t == node.System);
                int count = DescendanceCount(node) + 1;
                List<Type> range = m_TypeOrder.GetRange(start, count);
                m_TypeOrder.RemoveRange(start, count);

                return range;
            }

            private int DescendanceCount(Node node) {
                int count = node.Children.Count;

                foreach (var child in node.Children) {
                    count += DescendanceCount(child);
                }

                return count;
            }
        }

        private List<T> m_Systems = new();
        public List<T> Systems => m_Systems;

        private Dictionary<Type, HashSet<Type>> m_SystemsComingBefore = new();
        private Dictionary<Type, HashSet<Type>> m_SystemsComingAfter = new();

        private Dictionary<Type, List<T>> m_SystemsByType = new();

        private OrderTree m_OrderTree = new();

        public void Add(T system) {
            Type type = system.GetType();
        
            if (m_OrderTree.Contains(type)) {
                var systems = m_SystemsByType[type];

                systems.Add(system);

                int index = m_Systems.FindLastIndex((s) => s.GetType() == type);

                m_Systems.InsertOrAppend(index + 1, system);
            } else {
                IEnumerable<OrderAttribute> predicates = system.GetType().GetCustomAttributes<OrderAttribute>();
                if (predicates.Count() > 0)
                {
                    RegisterPredicates(type, predicates);
                }

                AddToTree(system);

                List<T> systems = m_SystemsByType.GetOrCreateValue(type);
                systems.Add(system);

                ReorderStorage();
            }
        }

        public void Remove(T system) {
            m_Systems.Remove(system);
            
            Type type = system.GetType();
            
            if (m_SystemsByType.TryGetValue(type, out List<T>? systems)) {
                systems.Remove(system);

                if (systems.Count == 0) {
                    m_OrderTree.Remove(type);
                    m_SystemsByType.Remove(type);

                    ReorderStorage();
                }
            }
        }

        private void RegisterPredicates(Type type, IEnumerable<OrderAttribute> predicates) {
            Type systemCategory = typeof(T);
            
            foreach (OrderAttribute predicate in predicates) {             

                if (!systemCategory.IsAssignableFrom(predicate.TargetSystem))
                    continue;

                if (predicate.Type == OrderType.After) {
                    HashSet<Type> systemsComingBefore = m_SystemsComingBefore.GetOrCreateValue(type);
                    systemsComingBefore.Add(predicate.TargetSystem);

                    HashSet<Type> systemsComingAfter = m_SystemsComingAfter.GetOrCreateValue(predicate.TargetSystem);
                    systemsComingAfter.Add(type);
                } else { // Before
                    HashSet<Type> systemsComingAfter = m_SystemsComingAfter.GetOrCreateValue(type);
                    systemsComingAfter.Add(predicate.TargetSystem);
                    
                    HashSet<Type> systemsComingBefore = m_SystemsComingBefore.GetOrCreateValue(predicate.TargetSystem);
                    systemsComingBefore.Add(type);
                }
            }
        }

        private void AddToTree(T system) {                
            Type type = system.GetType();

            if (m_SystemsComingBefore.TryGetValue(type, out HashSet<Type>? systemsComingBefore)) {

                foreach (var precedingSystem in systemsComingBefore)
                    m_OrderTree.AddAfter(type, precedingSystem);

            } else {
                // If not systems coming before, it won't be added to the tree
                // so we must add it to root
                m_OrderTree.AddAfter(type, null);
            }

            if (m_SystemsComingAfter.TryGetValue(type, out HashSet<Type>? systemsComingAfter)) {

                foreach (var followingSystem in systemsComingAfter)
                    m_OrderTree.AddBefore(type, followingSystem);
                
            }
        }

        public void ReorderStorage() {
            m_Systems.Clear();

            List<Type> order = m_OrderTree.ToList();

            foreach (var type in order) {
                if (m_SystemsByType.TryGetValue(type, out List<T>? systems)) {
                    m_Systems.AddRange(systems);
                }
            }
        }
    }
}

