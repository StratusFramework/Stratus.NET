using System.Collections.Generic;
using System;
using System.Numerics;

namespace Stratus
{
	/// <summary>
	///  A blackboard system is an artificial intelligence approach based on the blackboard architectural model, 
	///  where a common knowledge base, the "blackboard", is iteratively updated by a diverse group of specialist knowledge sources, 
	///  starting with a problem specification and ending with a solution. Each knowledge source updates the blackboard with a partial solution 
	///  when its internal constraints match the blackboard state. In this way, the specialists work together to solve the problem.
	/// </summary>
	public interface IBlackboard
	{
	}

	public class Blackboard : IBlackboard
	{
		#region Declarations
		/// <summary>
		/// The scope of a table on a given blackboard
		/// </summary>
		public enum Scope
		{
			Local,
			Global
		}

		/// <summary>
		/// A field that allows the selection of a given blackboard and specific keys within it
		/// from an inspector window
		/// </summary>
		[Serializable]
		public class Selector
		{
			public Blackboard blackboard;
			public Scope scope;
			public string key;

			/// <summary>
			/// Sets the value of the symbol with the selected key
			/// </summary>
			/// <param name="owner"></param>
			/// <param name="value"></param>
			public void Set(object owner, object value)
			{
				if (blackboard == null)
				{
					throw new NullReferenceException($"No blackboard has been set!");
				}

				switch (scope)
				{
					case Scope.Local:
						blackboard.SetLocal(owner, key, value);
						break;
					case Scope.Global:
						blackboard.SetGlobal(key, value);
						break;
				}
			}

			/// <summary>
			/// Gets the value of the symbol with the selected key
			/// </summary>
			/// <param name="owner"></param>
			/// <param name="value"></param>
			public object Get(object owner)
			{
				if (blackboard == null)
				{
					throw new NullReferenceException($"No blackboard has been set!");
				}

				object value = null;
				switch (scope)
				{
					case Scope.Local:
						value = blackboard.GetLocal(owner, key);
						break;
					case Scope.Global:
						value = blackboard.GetGlobal(key);
						break;
				}
				return value;
			}
		}

		/// <summary>
		/// A reference of a symbol within the blackboard
		/// </summary>
		[Serializable]
		public class SymbolReference
		{
			public string key;
			public Scope scope;
			public StratusVariant.VariantType type;

			public object GetValue(Blackboard blackboard, object gameObject)
			{
				if (scope == Scope.Local)
					return blackboard.GetLocal(gameObject, key);
				return blackboard.GetGlobal(key);
			}

			public void SetValue(Blackboard blackboard, object gameObject, object value)
			{
				if (scope == Scope.Local)
					blackboard.SetLocal(gameObject, key, value);
				else
					blackboard.SetGlobal(key, value);
			}
		}

		/// <summary>
		/// A reference of a symbol within the blackboard
		/// </summary>
		[Serializable]
		public class Reference<T>// where T : struct
		{
			public string key;
			public Scope scope;
			public StratusVariant.VariantType type { get; } = VariantUtilities.Convert(typeof(T));

			public T GetValue(Blackboard blackboard, object gameObject)
			{
				if (scope == Scope.Local)
					return blackboard.GetLocal<T>(gameObject, key);
				return blackboard.GetGlobal<T>(key);
			}

			public void SetValue(Blackboard blackboard, object gameObject, T value)
			{
				if (scope == Scope.Local)
					blackboard.SetLocal<T>(gameObject, key, value);
				else
					blackboard.SetGlobal<T>(key, value);
			}
		}

		/// <summary>
		/// A reference for a Vector3 within a blackboard
		/// </summary>
		public class Vector3Reference : Reference<Vector3>
		{
		}

		/// <summary>
		/// A reference for an integer within a blackboard
		/// </summary>
		public class IntegerReference : Reference<int>
		{
		}

		public delegate void OnGlobalSymbolChanged(StratusSymbol symbol);
		public delegate void OnLocalSymbolChanged(object gameObject, StratusSymbol symbol);
		#endregion

		#region Fields
		public string name;
		#endregion

		#region Events
		public event OnLocalSymbolChanged onLocalSymbolChanged;
		public event OnGlobalSymbolChanged onGlobalSymbolChanged;
		#endregion

		//----------------------------------------------------------------------/
		// Properties: Static
		//----------------------------------------------------------------------/
		/// <summary>
		/// Identifier for this particular blackboard at runtime
		/// </summary>
		public int id { get; set; }
		/// <summary>
		/// Runtime instantiated globals for a given blackboard
		/// </summary>
		private static Dictionary<Blackboard, StratusSymbolTable> instancedGlobals = new();
		/// <summary>
		/// Runtime instantiated locals (symbol tables) for a given blackboard, where we use a gameobject as the key
		/// </summary>
		private static Dictionary<Blackboard, Dictionary<object, StratusSymbolTable>> instancedLocals = new();

		//----------------------------------------------------------------------/
		// Fields
		//----------------------------------------------------------------------/
		/// <summary>
		/// Symbols which are available to all agents using this blackboard
		/// </summary>
		public StratusSymbolTable globals = new StratusSymbolTable();
		/// <summary>
		/// Symbols specific for each agent of this blackboard
		/// </summary>
		public StratusSymbolTable locals = new StratusSymbolTable();


		//----------------------------------------------------------------------/
		// Messages
		//----------------------------------------------------------------------/    
		private void OnValidate()
		{
		}

		//----------------------------------------------------------------------/
		// Methods
		//----------------------------------------------------------------------/
		/// <summary>
		/// Adds a local symbol to the blackboard
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="symbol"></param>
		public void Add(StratusSymbol symbol, Scope scope)
		{
			switch (scope)
			{
				case Scope.Local:
					this.AddLocal(symbol);
					break;
				case Scope.Global:
					this.AddGlobal(symbol);
					break;
			}
		}

		/// <summary>
		/// Adds a local symbol to the blackboard
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="symbol"></param>
		public void AddGlobal(StratusSymbol symbol)
		{
			this.globals.Add(symbol);
		}

		/// <summary>
		/// Adds a global symbol to the blackboard
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="symbol"></param>
		public void AddLocal(StratusSymbol symbol)
		{
			this.locals.Add(symbol);
		}

		/// <summary>
		/// Returns all the global symbols for this blackboard at runtime
		/// </summary>
		/// <returns></returns>
		public StratusSymbolTable GetGlobals()
		{
			if (!instancedGlobals.ContainsKey(this))
				instancedGlobals.Add(this, new StratusSymbolTable(this.globals));
			return instancedGlobals[this];
		}

		/// <summary>
		/// Returns all the local symbols for this blackboard at runtime
		/// </summary>
		/// <param name="local"></param>
		/// <returns></returns>
		public StratusSymbolTable GetLocals(object owner)
		{
			if (!instancedLocals.ContainsKey(this))
			{
				instancedLocals.Add(this, new Dictionary<object, StratusSymbolTable>());
			}

			if (!instancedLocals[this].ContainsKey(owner))
				instancedLocals[this].Add(owner, new StratusSymbolTable(this.locals));

			return instancedLocals[this][owner];
		}

		// Get

		/// <summary>
		/// Gets the value of a local symbol
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="owner"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetLocal<T>(object owner, string key) => GetLocals(owner).GetValue<T>(key);

		/// <summary>
		/// Gets the value of a local symbol
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="owner"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public object GetLocal(object owner, string key) => GetLocals(owner).GetValue(key);

		/// <summary>
		/// Gets the value of a global symbol
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetGlobal<T>(string key) => GetGlobals().GetValue<T>(key);

		/// <summary>
		/// Gets the value of a global symbol
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public object GetGlobal(string key) => GetGlobals().GetValue(key);

		/// <summary>
		/// Sets the value of a local symbol
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetLocal<T>(object owner, string key, T value)
		{
			StratusSymbol symbol = GetLocals(owner).Find(key);
			symbol.SetValue(value);
			onLocalSymbolChanged?.Invoke(owner, symbol);
		}

		/// <summary>
		/// Sets the value of a local symbol
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetLocal(object owner, string key, object value)
		{
			StratusSymbol symbol = GetLocals(owner).Find(key);
			symbol.SetValue(value);
			onLocalSymbolChanged?.Invoke(owner, symbol);
		}

		/// <summary>
		/// Sets the value of a global symbol
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetGlobal<T>(string key, T value)
		{
			StratusSymbol symbol = GetGlobals().Find(key);
			symbol.SetValue(value);
			onGlobalSymbolChanged?.Invoke(symbol);

			//GetGlobals().SetValue<T>(key, value);
		}

		/// <summary>
		/// Sets the value of a global symbol
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetGlobal(string key, object value)
		{
			StratusSymbol symbol = GetGlobals().Find(key);
			symbol.SetValue(value);
			onGlobalSymbolChanged?.Invoke(symbol);
			//GetGlobals().SetValue(key, value);
		}

		//----------------------------------------------------------------------/
		// Methods
		//----------------------------------------------------------------------/    
		/// <summary>
		/// Gets the value of a symbol from the table
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="scope"></param>
		/// <param name="table"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private T Get<T>(Scope scope, StratusSymbolTable table, string key)
		{
			try
			{
				T value = table.GetValue<T>(key);
				return value;
			}
			catch (KeyNotFoundException e)
			{
				throw new KeyNotFoundException($"{name} : {e.Message}");
			}
		}


		/// <summary>
		/// Gets the value of a symbol from the table
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="scope"></param>
		/// <param name="table"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private void Set<T>(Scope scope, StratusSymbolTable table, string key, T value)
		{
			try
			{
				table.SetValue(key, value);
			}
			catch (KeyNotFoundException e)
			{
				throw new KeyNotFoundException($"{name} : {e.Message}");
			}
		}
	}
}