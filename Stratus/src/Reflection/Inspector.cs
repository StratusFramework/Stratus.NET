using Stratus.Extensions;
using Stratus.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stratus.Reflection
{
	public class Node : MemberReference
	{
		private Lazy<Dictionary<string, Node>> _children;

		public Node parent { get; private set; }
		public IReadOnlyDictionary<string, Node> children => _children.Value;
		public bool hasChildren => children.Count > 0;

		private static readonly HashSet<Type> ignored = new HashSet<Type>()
		{
			typeof(string)
		};

		public Node(FieldInfo field, object target) : base(field, target)
		{
			Setup(value);
		}

		public Node(PropertyInfo property, object target) : base(property, target)
		{
			Setup(value);
		}

		protected void Setup(object target)
		{
			_children = new Lazy<Dictionary<string, Node>>(() =>
			{
				if (type.IsPrimitive || ignored.Contains(type) || TypeUtility.IsCollection(type))
				{
					return new Dictionary<string, Node>();
				}

				return From(target)
				.ForEach(n => n.parent = this)
				.ToDictionary(m => m.name);
			});
		}

		public static IEnumerable<Node> From(object target)
		{
			var members = ReflectionUtility.GetAllFieldsOrProperties(target);
			var nodes = members.Select(m => m.memberType == MemberTypes.Property
			? new Node(m.property, m.obj)
			: new Node(m.field, m.obj));
			return nodes;
		}

		public void Visit(Action<Node, int> onNode)
		{
			void visit(Node node, int level)
			{
				onNode(node, level);
				foreach (var child in node.children.Values)
				{
					visit(child, level + 1);
				}
			}

			onNode(this, 0);
			children.Values.ForEach(n => visit(n, 0));
		}
	}

	public class Inspector
	{
		public object target { get; }
		public Inspector parent { get; }
		public bool isRoot => parent == null;

		private List<Node> _nodes;
		public IReadOnlyList<Node> nodes => _nodes;

		public Inspector(object target) : this(target, null)
		{
		}

		private Inspector(object target, Inspector parent)
		{
			this.target = target;
			this.parent = parent;
			this._nodes = Node.From(target).ToList();
		}

		public void Visit(Action<Node, int> onNode)
		{
			nodes.ForEach(n => n.Visit(onNode));
		}
	}


}