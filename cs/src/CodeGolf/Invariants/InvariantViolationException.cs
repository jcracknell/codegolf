using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGolf.Invariants {
	/// <summary>
	/// Exception thrown to notify callers of the violation of an invariant.
	/// </summary>
	public class InvariantViolationException : Exception {
		private readonly object _invariant;
		private readonly object _subject;
		private readonly InvariantViolationException _inner;

		public InvariantViolationException(object invariant, object subject) 
			: this(invariant, subject, null) { }

		public InvariantViolationException(object invariant, object subject, InvariantViolationException inner)
			: base("Invariant " + SafeToString(invariant) + " violated by subject: " + SafeToString(subject), inner)
		{
			_invariant = invariant;
			_subject = subject;
			_inner = inner;
		}

		/// <summary>
		/// The invariant which was violated.
		/// </summary>
		public object Invariant { get { return _invariant; } }

		/// <summary>
		/// The subject instance which violated the invariant.
		/// </summary>
		public object Subject { get { return _invariant; } }

		public new InvariantViolationException InnerException { get { return _inner; } }

		private static string SafeToString(object o) {
			if(null == o) return "<NULL>";

			if(InvariantUtils.OverridesToString(o.GetType()))
				try { return o.ToString(); }
				catch { }

			return o.GetType().ToString();
		}
	}
}
