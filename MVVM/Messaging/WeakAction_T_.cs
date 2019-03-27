using System;

namespace MVVM.Messaging
{
	/// <summary>
	/// Stores an Action without causing a hard reference
	/// to be created to the Action's owner. The owner can be garbage collected at any time.
	/// </summary>
	/// <typeparam name="T">The type of the Action's parameter.</typeparam>
	public class WeakAction<T> : WeakAction, IExecuteWithObject
	{
		private readonly Action<T> _action;

		/// <summary>
		/// Gets the Action associated to this instance.
		/// </summary>
		public new Action<T> Action
		{
			get
			{
				return this._action;
			}
		}

		/// <summary>
		/// Initializes a new instance of the WeakAction class.
		/// </summary>
		/// <param name="target">The action's owner.</param>
		/// <param name="action">The action that will be associated to this instance.</param>
		public WeakAction(object target, Action<T> action) : base(target, null)
		{
			this._action = action;
		}

		/// <summary>
		/// Executes the action. This only happens if the action's owner
		/// is still alive. The action's parameter is set to default(T).
		/// </summary>
		public new void Execute()
		{
			if ((this._action == null ? false : base.IsAlive))
			{
				this._action(default(T));
			}
		}

		/// <summary>
		/// Executes the action. This only happens if the action's owner
		/// is still alive.
		/// </summary>
		/// <param name="parameter">A parameter to be passed to the action.</param>
		public void Execute(T parameter)
		{
			if ((this._action == null ? false : base.IsAlive))
			{
				this._action(parameter);
			}
		}

		/// <summary>
		/// Executes the action with a parameter of type object. This parameter
		/// will be casted to T. This method implements <see cref="M:MVVM.Messaging.IExecuteWithObject.ExecuteWithObject(System.Object)" />
		/// and can be useful if you store multiple WeakAction{T} instances but don't know in advance
		/// what type T represents.
		/// </summary>
		/// <param name="parameter">The parameter that will be passed to the action after
		/// being casted to T.</param>
		public void ExecuteWithObject(object parameter)
		{
			this.Execute((T)parameter);
		}
	}
}