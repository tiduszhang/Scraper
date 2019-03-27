using System;
using System.Collections.Generic;
using System.Linq;

namespace MVVM.Messaging
{
	/// <summary>
	/// The Messenger is a class allowing objects to exchange messages.
	/// </summary>
	public class Messenger : IMessenger
	{
		private static Messenger _defaultInstance;

		private Dictionary<Type, List<Messenger.WeakActionAndToken>> _recipientsOfSubclassesAction;

		private Dictionary<Type, List<Messenger.WeakActionAndToken>> _recipientsStrictAction;

		/// <summary>
		/// Gets the Messenger's default instance, allowing
		/// to register and send messages in a static manner.
		/// </summary>
		public static Messenger Default
		{
			get
			{
				if (Messenger._defaultInstance == null)
				{
					Messenger._defaultInstance = new Messenger();
				}
				return Messenger._defaultInstance;
			}
		}
        /// <summary>
        /// 
        /// </summary>
		public Messenger()
		{
		}

		private void Cleanup()
		{
			Messenger.CleanupList(this._recipientsOfSubclassesAction);
			Messenger.CleanupList(this._recipientsStrictAction);
		}

		private static void CleanupList(IDictionary<Type, List<Messenger.WeakActionAndToken>> lists)
		{
			if (lists != null)
			{
				List<Type> types = new List<Type>();
				foreach (KeyValuePair<Type, List<Messenger.WeakActionAndToken>> list in lists)
				{
					List<Messenger.WeakActionAndToken> weakActionAndTokens = new List<Messenger.WeakActionAndToken>();
					foreach (Messenger.WeakActionAndToken value in list.Value)
					{
						if ((value.Action == null ? true : !value.Action.IsAlive))
						{
							weakActionAndTokens.Add(value);
						}
					}
					foreach (Messenger.WeakActionAndToken weakActionAndToken in weakActionAndTokens)
					{
						list.Value.Remove(weakActionAndToken);
					}
					if (list.Value.Count == 0)
					{
						types.Add(list.Key);
					}
				}
				foreach (Type type in types)
				{
					lists.Remove(type);
				}
			}
		}

		private static bool Implements(Type instanceType, Type interfaceType)
		{
			bool flag;
			if ((interfaceType == null ? false : instanceType != null))
			{
				Type[] interfaces = instanceType.GetInterfaces();
				int num = 0;
				while (num < (int)interfaces.Length)
				{
					if (interfaces[num] != interfaceType)
					{
						num++;
					}
					else
					{
						flag = true;
						return flag;
					}
				}
				flag = false;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		/// <summary>
		/// Provides a way to override the Messenger.Default instance with
		/// a custom instance, for example for unit testing purposes.
		/// </summary>
		/// <param name="newMessenger">The instance that will be used as Messenger.Default.</param>
		public static void OverrideDefault(Messenger newMessenger)
		{
			Messenger._defaultInstance = newMessenger;
		}

		/// <summary>
		/// Registers a recipient for a type of message TMessage. The action
		/// parameter will be executed when a corresponding message is sent.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		public virtual void Register<TMessage>(object recipient, Action<TMessage> action)
		{
			this.Register<TMessage>(recipient, null, false, action);
		}

		/// <summary>
		/// Registers a recipient for a type of message TMessage.
		/// The action parameter will be executed when a corresponding 
		/// message is sent. See the receiveDerivedMessagesToo parameter
		/// for details on how messages deriving from TMessage (or, if TMessage is an interface,
		/// messages implementing TMessage) can be received too.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="receiveDerivedMessagesToo">If true, message types deriving from
		/// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.
		/// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
		/// transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.</para>
		/// </param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		public virtual void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action)
		{
			this.Register<TMessage>(recipient, null, receiveDerivedMessagesToo, action);
		}

		/// <summary>
		/// Registers a recipient for a type of message TMessage.
		/// The action parameter will be executed when a corresponding 
		/// message is sent.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="token">A token for a messaging channel. If a recipient registers
		/// using a token, and a sender sends a message using the same token, then this
		/// message will be delivered to the recipient. Other recipients who did not
		/// use a token when registering (or who used a different token) will not
		/// get the message. Similarly, messages sent without any token, or with a different
		/// token, will not be delivered to that recipient.</param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		public virtual void Register<TMessage>(object recipient, object token, Action<TMessage> action)
		{
			this.Register<TMessage>(recipient, token, false, action);
		}

		/// <summary>
		/// Registers a recipient for a type of message TMessage.
		/// The action parameter will be executed when a corresponding 
		/// message is sent. See the receiveDerivedMessagesToo parameter
		/// for details on how messages deriving from TMessage (or, if TMessage is an interface,
		/// messages implementing TMessage) can be received too.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="token">A token for a messaging channel. If a recipient registers
		/// using a token, and a sender sends a message using the same token, then this
		/// message will be delivered to the recipient. Other recipients who did not
		/// use a token when registering (or who used a different token) will not
		/// get the message. Similarly, messages sent without any token, or with a different
		/// token, will not be delivered to that recipient.</param>
		/// <param name="receiveDerivedMessagesToo">If true, message types deriving from
		/// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.
		/// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
		/// transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.</para>
		/// </param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		public virtual void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action)
		{
			Dictionary<Type, List<Messenger.WeakActionAndToken>> types;
			List<Messenger.WeakActionAndToken> item;
			Type type = typeof(TMessage);
			if (!receiveDerivedMessagesToo)
			{
				if (this._recipientsStrictAction == null)
				{
					this._recipientsStrictAction = new Dictionary<Type, List<Messenger.WeakActionAndToken>>();
				}
				types = this._recipientsStrictAction;
			}
			else
			{
				if (this._recipientsOfSubclassesAction == null)
				{
					this._recipientsOfSubclassesAction = new Dictionary<Type, List<Messenger.WeakActionAndToken>>();
				}
				types = this._recipientsOfSubclassesAction;
			}
			if (types.ContainsKey(type))
			{
				item = types[type];
			}
			else
			{
				item = new List<Messenger.WeakActionAndToken>();
				types.Add(type, item);
			}
			WeakAction<TMessage> weakAction = new WeakAction<TMessage>(recipient, action);
			Messenger.WeakActionAndToken weakActionAndToken = new Messenger.WeakActionAndToken()
			{
				Action = weakAction,
				Token = token
			};
			item.Add(weakActionAndToken);
			this.Cleanup();
		}

		/// <summary>
		/// Sets the Messenger's default (static) instance to null.
		/// </summary>
		public static void Reset()
		{
			Messenger._defaultInstance = null;
		}

		/// <summary>
		/// Sends a message to registered recipients. The message will
		/// reach all recipients that registered for this message type
		/// using one of the Register methods.
		/// </summary>
		/// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
		/// <param name="message">The message to send to registered recipients.</param>
		public virtual void Send<TMessage>(TMessage message)
		{
			this.SendToTargetOrType<TMessage>(message, null, null);
		}

		/// <summary>
		/// Sends a message to registered recipients. The message will
		/// reach only recipients that registered for this message type
		/// using one of the Register methods, and that are
		/// of the targetType.
		/// </summary>
		/// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
		/// <typeparam name="TTarget">The type of recipients that will receive
		/// the message. The message won't be sent to recipients of another type.</typeparam>
		/// <param name="message">The message to send to registered recipients.</param>
		public virtual void Send<TMessage, TTarget>(TMessage message)
		{
			this.SendToTargetOrType<TMessage>(message, typeof(TTarget), null);
		}

		/// <summary>
		/// Sends a message to registered recipients. The message will
		/// reach only recipients that registered for this message type
		/// using one of the Register methods, and that are
		/// of the targetType.
		/// </summary>
		/// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
		/// <param name="message">The message to send to registered recipients.</param>
		/// <param name="token">A token for a messaging channel. If a recipient registers
		/// using a token, and a sender sends a message using the same token, then this
		/// message will be delivered to the recipient. Other recipients who did not
		/// use a token when registering (or who used a different token) will not
		/// get the message. Similarly, messages sent without any token, or with a different
		/// token, will not be delivered to that recipient.</param>
		public virtual void Send<TMessage>(TMessage message, object token)
		{
			this.SendToTargetOrType<TMessage>(message, null, token);
		}

		private static void SendToList<TMessage>(TMessage message, IEnumerable<Messenger.WeakActionAndToken> list, Type messageTargetType, object token)
		{
			bool flag;
			if (list != null)
			{
				List<Messenger.WeakActionAndToken> weakActionAndTokens = list.Take<Messenger.WeakActionAndToken>(list.Count<Messenger.WeakActionAndToken>()).ToList<Messenger.WeakActionAndToken>();
				foreach (Messenger.WeakActionAndToken weakActionAndToken in weakActionAndTokens)
				{
					IExecuteWithObject action = weakActionAndToken.Action as IExecuteWithObject;
					if (action == null || !weakActionAndToken.Action.IsAlive || weakActionAndToken.Action.Target == null || !(messageTargetType == null) && !(weakActionAndToken.Action.Target.GetType() == messageTargetType) && !Messenger.Implements(weakActionAndToken.Action.Target.GetType(), messageTargetType))
					{
						flag = false;
					}
					else if (weakActionAndToken.Token != null || token != null)
					{
						flag = (weakActionAndToken.Token == null ? false : weakActionAndToken.Token.Equals(token));
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						action.ExecuteWithObject(message);
					}
				}
			}
		}

		private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
		{
			Type type = typeof(TMessage);
			if (this._recipientsOfSubclassesAction != null)
			{
				List<Type> list = this._recipientsOfSubclassesAction.Keys.Take<Type>(this._recipientsOfSubclassesAction.Count<KeyValuePair<Type, List<Messenger.WeakActionAndToken>>>()).ToList<Type>();
				foreach (Type type1 in list)
				{
					List<Messenger.WeakActionAndToken> item = null;
					if ((type == type1 || type.IsSubclassOf(type1) ? true : Messenger.Implements(type, type1)))
					{
						item = this._recipientsOfSubclassesAction[type1];
					}
					Messenger.SendToList<TMessage>(message, item, messageTargetType, token);
				}
			}
			if (this._recipientsStrictAction != null)
			{
				if (this._recipientsStrictAction.ContainsKey(type))
				{
					List<Messenger.WeakActionAndToken> weakActionAndTokens = this._recipientsStrictAction[type];
					Messenger.SendToList<TMessage>(message, weakActionAndTokens, messageTargetType, token);
				}
			}
			this.Cleanup();
		}

		/// <summary>
		/// Unregisters a messager recipient completely. After this method
		/// is executed, the recipient will not receive any messages anymore.
		/// </summary>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		public virtual void Unregister(object recipient)
		{
			Messenger.UnregisterFromLists(recipient, this._recipientsOfSubclassesAction);
			Messenger.UnregisterFromLists(recipient, this._recipientsStrictAction);
		}

		/// <summary>
		/// Unregisters a message recipient for a given type of messages only. 
		/// After this method is executed, the recipient will not receive messages
		/// of type TMessage anymore, but will still receive other message types (if it
		/// registered for them previously).
		/// </summary>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		/// <typeparam name="TMessage">The type of messages that the recipient wants
		/// to unregister from.</typeparam>
		public virtual void Unregister<TMessage>(object recipient)
		{
			this.Unregister<TMessage>(recipient, null);
		}

		/// <summary>
		/// Unregisters a message recipient for a given type of messages only and for a given token. 
		/// After this method is executed, the recipient will not receive messages
		/// of type TMessage anymore with the given token, but will still receive other message types
		/// or messages with other tokens (if it registered for them previously).
		/// </summary>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		/// <param name="token">The token for which the recipient must be unregistered.</param>
		/// <typeparam name="TMessage">The type of messages that the recipient wants
		/// to unregister from.</typeparam>
		public virtual void Unregister<TMessage>(object recipient, object token)
		{
			this.Unregister<TMessage>(recipient, token, null);
		}

		/// <summary>
		/// Unregisters a message recipient for a given type of messages and for
		/// a given action. Other message types will still be transmitted to the
		/// recipient (if it registered for them previously). Other actions that have
		/// been registered for the message type TMessage and for the given recipient (if
		/// available) will also remain available.
		/// </summary>
		/// <typeparam name="TMessage">The type of messages that the recipient wants
		/// to unregister from.</typeparam>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		/// <param name="action">The action that must be unregistered for
		/// the recipient and for the message type TMessage.</param>
		public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action)
		{
			Messenger.UnregisterFromLists<TMessage>(recipient, action, this._recipientsStrictAction);
			Messenger.UnregisterFromLists<TMessage>(recipient, action, this._recipientsOfSubclassesAction);
			this.Cleanup();
		}

		/// <summary>
		/// Unregisters a message recipient for a given type of messages, for
		/// a given action and a given token. Other message types will still be transmitted to the
		/// recipient (if it registered for them previously). Other actions that have
		/// been registered for the message type TMessage, for the given recipient and other tokens (if
		/// available) will also remain available.
		/// </summary>
		/// <typeparam name="TMessage">The type of messages that the recipient wants
		/// to unregister from.</typeparam>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		/// <param name="token">The token for which the recipient must be unregistered.</param>
		/// <param name="action">The action that must be unregistered for
		/// the recipient and for the message type TMessage.</param>
		public virtual void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
		{
			Messenger.UnregisterFromLists<TMessage>(recipient, token, action, this._recipientsStrictAction);
			Messenger.UnregisterFromLists<TMessage>(recipient, token, action, this._recipientsOfSubclassesAction);
			this.Cleanup();
		}

		private static void UnregisterFromLists(object recipient, Dictionary<Type, List<Messenger.WeakActionAndToken>> lists)
		{
			if ((recipient == null || lists == null ? false : lists.Count != 0))
			{
				lock (lists)
				{
					foreach (Type key in lists.Keys)
					{
						foreach (Messenger.WeakActionAndToken item in lists[key])
						{
							WeakAction action = item.Action;
							if ((action == null ? false : recipient == action.Target))
							{
								action.MarkForDeletion();
							}
						}
					}
				}
			}
		}

		private static void UnregisterFromLists<TMessage>(object recipient, Action<TMessage> action, Dictionary<Type, List<Messenger.WeakActionAndToken>> lists)
		{
			bool flag;
			Type type = typeof(TMessage);
			if ((recipient == null || lists == null || lists.Count == 0 ? false : lists.ContainsKey(type)))
			{
				lock (lists)
				{
					foreach (Messenger.WeakActionAndToken item in lists[type])
					{
                        if (!(item.Action is WeakAction<TMessage> weakAction) || recipient != weakAction.Target)
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = (action == null ? true : action == weakAction.Action);
                        }
                        if (flag)
						{
							item.Action.MarkForDeletion();
						}
					}
				}
			}
		}

		private static void UnregisterFromLists<TMessage>(object recipient, object token, Action<TMessage> action, Dictionary<Type, List<Messenger.WeakActionAndToken>> lists)
		{
			bool flag;
			Type type = typeof(TMessage);
			if ((recipient == null || lists == null || lists.Count == 0 ? false : lists.ContainsKey(type)))
			{
				lock (lists)
				{
					foreach (Messenger.WeakActionAndToken item in lists[type])
					{
                        if (!(item.Action is WeakAction<TMessage> weakAction) || recipient != weakAction.Target || action != null && !(action == weakAction.Action))
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = (token == null ? true : token.Equals(item.Token));
                        }
                        if (flag)
						{
							item.Action.MarkForDeletion();
						}
					}
				}
			}
		}

		private struct WeakActionAndToken
		{
			public WeakAction Action;

			public object Token;
		}
	}
}