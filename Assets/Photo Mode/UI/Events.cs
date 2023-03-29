using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

interface EventSubscription
{
	public void Unsubscribe();
}

class EventSubscriptions
{
	List<EventSubscription> subscriptions = new List<EventSubscription>();

	public void Add(EventSubscription subscription)
	{
		subscriptions.Add(subscription);
	}

	public void Subscribe<T>(Action<T> target, Action<T> func)
	{
		Add(EventSubscription<T>.Subscribe(target, func));
	}

	public void Subscribe<T>(UnityEvent<T> target, UnityAction<T> func)
	{
		Add(UnityEventSubscription<T>.Subscribe(target, func));
	}

	public void Unsubscribe()
	{
		foreach (EventSubscription subscription in subscriptions)
		{
			subscription.Unsubscribe();
		}
		subscriptions.Clear();
	}
}

class EventSubscription<T> : EventSubscription
{
	Action<T> target;
	Action<T> func;

	public EventSubscription(Action<T> target, Action<T> func)
	{
		this.target = target;
		this.func = func;
	}

	public void Unsubscribe()
	{
		target -= func;
	}

	public static EventSubscription Subscribe<T>(Action<T> target, Action<T> func)
	{
		target += func;
		return new EventSubscription<T>(target, func);
	}
}

class UnityEventSubscription<T> : EventSubscription
{
	UnityEvent<T> target;
	UnityAction<T> func;

	public UnityEventSubscription(UnityEvent<T> target, UnityAction<T> func)
	{
		this.target = target;
		this.func = func;
	}

	public void Unsubscribe()
	{
		target.RemoveListener(func);
	}

	public static EventSubscription Subscribe<T>(UnityEvent<T> target, UnityAction<T> func)
	{
		target.AddListener(func);
		return new UnityEventSubscription<T>(target, func);
	}
}

