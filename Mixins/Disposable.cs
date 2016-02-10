﻿using System;

namespace Mixins
{
    // we can attach custom action on dispose
    public interface MDisposable : Mixin { } 

	public static partial class Extensions
	{
        private static partial class SystemFields
        {
            public const string Lifetime = "#lifetime";
        }

        internal class Lifetime<T>
		{
			private Action<T> action;
			private readonly T self;

			public Lifetime(Action<T> action, T self)
			{
				this.action = action;
				this.self = self;
			}

			public void Reset(Action<T> action)
			{
				this.action = action;
			}

			~Lifetime()
			{
				if (action != null) action(self);
			}
		}

		public static void OnDispose<T>(this T self, Action<T> action) where T : MDisposable 
		{
			var state = self.GetStateInternal();
			object old;
            if (state.TryGetValue(SystemFields.Lifetime, out old))
			{
				((Lifetime<T>)old).Reset(action); // reset old action if any
			}
			else
			{
                state[SystemFields.Lifetime] = new Lifetime<T>(action, self);	
			}
		}
    }
}
