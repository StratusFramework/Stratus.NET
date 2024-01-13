namespace Stratus.Interpolation
{
	/// <summary>
	/// Action is the base class from which all other actions derive from.
	/// </summary>
	public abstract class ActionBase
	{
		#region Properties
		/// <summary>
		/// An optional name for this action
		/// </summary>
		/// <remarks>Primarily given for debugging purposes</remarks>
		public string? name { get; set; }
		/// <summary>
		/// A private identifier for this action.
		/// </summary>
		public int id { get; private set; }
		/// <summary>
		/// How much time has elapsed since the action started running
		/// </summary>
		public float elapsed { get; protected set; }
		/// <summary>
		/// The total amount of time the action will run for
		/// </summary>
		public float duration { get; protected set; }
		/// <summary>
		/// Whether the action is currently active. If not active it may end up
		/// blocking others behind it (if its on a sequence).
		/// </summary>
		public bool isActive { get; private set; }
		/// <summary>
		/// Whether the action has finished running.
		/// </summary>
		/// <remarks>This usually acts as a marker for it to be removed.</remarks>
		public bool isFinished = false;
		/// <summary>
		/// Whether we are logging actions
		/// </summary>
		protected static bool logging = false;
		#endregion

		#region Static
		/// <summary>
		/// How many actions have been created so far
		/// </summary>
		private static int created = 0;

		/// <summary>
		/// How many actions have been destroyed so far
		/// </summary>
		private static int destroyed = 0;
		#endregion

		#region Virtual
		public abstract float Update(float dt);
		#endregion

		#region Constructors
		public ActionBase()
		{
			this.id = created++;
			name = GetType().Name;
		}

		~ActionBase()
		{
			destroyed++;
		}

		public override string ToString()
		{
			return $"{name}({id})";
		}
		#endregion

		#region Interface
		/// <summary>
		/// Resumes running the action. It will no longer block any actions beyond it in a sequence.
		/// </summary>
		public void Resume()
		{
			this.isActive = true;
		}

		/// <summary>
		/// Pauses the update of this action. This will block a sequence 
		/// if there's other actions behind it.
		/// </summary>
		public void Pause()
		{
			this.isActive = false;
		}

		/// <summary>
		/// Cancels execution of this action. It will be cleaned up at the next opportunity.
		/// </summary>
		public void Cancel()
		{
			this.isActive = false;
			this.isFinished = true;
		} 
		#endregion
	}
}
