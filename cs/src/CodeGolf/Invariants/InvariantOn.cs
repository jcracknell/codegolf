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
	public interface InvariantOn<in TSubject> {
		/// <summary>
		/// Returns true if the invariant is satisfied by the provided <paramref name="subject"/> implementation of <typeparamref name="TSubject"/>.
		/// </summary>
		/// <param name="subject">The <typeparamref name="TSubject"/> instance for which the satisfaction of the invariant should be verified.</param>
		bool IsSatisfiedBy(TSubject subject);
	}
}
