using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGolf.Invariants {
	/// <summary>
	/// Interface defining an invariant condition which must hold on instances, subclasses, or implementations of the
	/// class or interface specified by <typeparamref name="TSubject"/>.
	/// Invariants defining a public parameterless constructor are "unparametrized invariants", which can be automatically loaded and applied.
	/// </summary>
	/// <typeparam name="TSubject">Type or interface to whose instances, subclasses or implementations the invariant applies.</typeparam>
	public interface IInvariant<in TSubject> {
		/// <summary>
		/// Asserts that the invariant is satisfied by the provided <paramref name="subject"/>, throwing an <see cref="InvariantViolationException"/>
		/// in the event that the invariant is not satisfied.
		/// </summary>
		/// <param name="subject">The <typeparamref name="TSubject"/> for which the satisfaction of the invariant should be asserted.</param>
		/// <exception cref="InvariantViolationException"/>
		void AssertSatisfiedBy(TSubject subject);
	}
}
