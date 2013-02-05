using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGolf.Invariants {
	/// <summary>
	/// Abstract base implementation of an invariant which must hold on instances, subclasses, or implementations of the
	/// class or interface specified by <typeparamref name="TSubject"/>.
	/// Invariants defining a public parameterless constructor are "unparametrized invariants", which can be automatically loaded and applied.
	/// </summary>
	/// <typeparam name="TSubject">Type or interface to whose instances, subclasses or implementations the invariant applies.</typeparam>
	public abstract class InvariantFor<TSubject> : IInvariant<TSubject> {
		/// <summary>
		/// Returns true if the invariant is satisfied by the provided <paramref name="subject"/>.
		/// </summary>
		/// <param name="subject">The <typeparamref name="TSubject"/> for which the satisfaction of the invariant should be verified.</param>
		public abstract bool IsSatisfiedBy(TSubject subject);

		public virtual bool AppliesTo(TSubject subject) {
			return true;
		}

		public void AssertSatisfiedBy(TSubject subject) {
			if(AppliesTo(subject) && !IsSatisfiedBy(subject))
				throw new InvariantViolationException(this, subject);
		}
	}
}
