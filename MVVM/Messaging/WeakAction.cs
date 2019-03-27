using System;

namespace MVVM.Messaging
{
	/// <summary>
	/// Stores an <see cref="P:MVVM.Messaging.WeakAction.Action" /> without causing a hard reference
	/// to be created to the Action's owner. The owner can be garbage collected at any time.
	/// </summary>
	public class WeakAction
	{
		private readonly System.Action _action;

		private WeakReference _reference;

		/// <summary>
		/// Gets the Action associated to this instance.
		/// </summary>
		public System.Action Action
		{
			get
			{
				return this._action;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Action's owner is still alive, or if it was collected
		/// by the Garbage Collector already.
		/// </summary>
		public bool IsAlive
		{
			get
			{
				bool flag;
				flag = (this._reference != null ? this._reference.IsAlive : false);
				return flag;
			}
		}

		/// <summary>
		/// Gets the Action's owner. This object is stored as a <see cref="T:System.WeakReference" />.
		/// </summary>
		public object Target
		{
			get
			{
				object obj;
				obj = (_reference != null ? this._reference.Target : null);
				return obj;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MVVM.Messaging.WeakAction" /> class.
		/// </summary>
		/// <param name="target">The action's owner.</param>
		/// <param name="action">The action that will be associated to this instance.</param>
		public WeakAction(object target, System.Action action)
		{
			this._reference = new WeakReference(target);
			this._action = action;
		}

		/// <summary>
		/// Executes the action. This only happens if the action's owner
		/// is still alive.
		/// </summary>
		public void Execute()
		{
			if ((this._action == null ? false : this.IsAlive))
			{
				this._action();
			}
		}

		/// <summary>
		/// Sets the reference that this instance stores to null.
		/// </summary>
		public void MarkForDeletion()
		{
			this._reference = null;
		}
	}
}