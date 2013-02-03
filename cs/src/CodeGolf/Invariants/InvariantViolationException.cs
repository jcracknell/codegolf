using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGolf.Invariants {
	/// <summary>
	/// Exception thrown to notify callers of the violation of an invariant.
	/// </summary>
	public class InvariantViolationException : Exception {

		public InvariantViolationException(object invariant, object subject)
			: base("Invariant " + SafeToString(invariant) + " violated by subject " + SafeToString(subject))
		{
			Invariant = invariant;
			Subject = subject;
		}

		/// <summary>
		/// The invariant which was violated.
		/// </summary>
		public object Invariant { get; private set; }

		/// <summary>
		/// The subject instance which violated the invariant.
		/// </summary>
		public object Subject { get; private set; }

		private static string SafeToString(object o) {
			if(null == o) return "<NULL>";
			try { return o.ToString(); }
			catch { return o.GetType().FullName; }
		}
	}
}
