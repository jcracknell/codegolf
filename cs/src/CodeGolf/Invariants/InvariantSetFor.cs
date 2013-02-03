using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeGolf.Invariants {
	/// <summary>
	/// Invariant set which can be used to assert the satisfaction of invariants for instances, subclasses, or implementors
	/// if <typeparamref name="TSubject"/>.
	/// </summary>
	/// <typeparam name="TSubject">The subject type for the invariant set.</typeparam>
	public class InvariantSetFor<TSubject> : InvariantOn<TSubject> {
		private readonly HashSet<Type> _addedUnparametrizedInvariants = new HashSet<Type>();
		private readonly List<InvariantOn<TSubject>> _invariants = new List<InvariantOn<TSubject>>();

		/// <summary>
		/// Create a new invariant set which can be used to assert the satisfaction of invariants for instances,
		/// subclasses, or implementors of <typeparamref name="TSubject"/>,
		/// instantiating and adding all applicable unparametrized invariant types declared in the assembly of <typeparamref name="TSubject"/>.
		/// </summary>
		public InvariantSetFor() : this(true) { }

		/// <summary>
		/// Create a new invariant set which can be used to assert the satisfaction of invariants for instances,
		/// subclasses, or implementors of <typeparamref name="TSubject"/>,
		/// optionally instantiating and adding all applicable unparametrized invariant types declared in the assembly of <typeparamref name="TSubject"/>.
		/// </summary>
		/// <param name="addFromAssembly">True if all applicable unparametrized invariant types declared in the assembly of <typeparamref name="TSubject"/> should be instantiated and added.</param>
		public InvariantSetFor(bool addFromAssembly) {
			if(addFromAssembly)
				AddFromAssemblyOf<TSubject>();
		}

		/// <summary>
		/// Add the provided <paramref name="invariant"/> to the invariant set.
		/// </summary>
		/// <param name="invariant">The invariant to be added to the invariant set.</param>
		public void Add(InvariantOn<TSubject> invariant) {
			if(null == invariant) throw Xception.Because.ArgumentNull(() => invariant);

			_invariants.Add(invariant);
		}

		/// <summary>
		/// Add the provided <paramref name="invariants"/> to the invariant set.
		/// </summary>
		/// <param name="invariants">The invariants which should be added to the invariant set.</param>
		public void Add(IEnumerable<InvariantOn<TSubject>> invariants) {
			if(null == invariants) throw Xception.Because.ArgumentNull(() => invariants);

			foreach(var invariant in invariants)
				Add(invariant);
		}

		private static bool NoFilter(Type type) { return true; }

		/// <summary>
		/// Instantiate and add all unparametrized invariant types declared in the same assembly as type <typeparamref name="T"/>
		/// which are applicable to <typeparamref name="TSubject"/> to the invariant set.
		/// </summary>
		/// <typeparam name="T">The type for which all unparametrized invariant types declared in the same assembly and applicable to <typeparamref name="TSubject"/> should be added to the invariant set.</typeparam>
		public InvariantSetFor<TSubject> AddFromAssemblyOf<T>() {
			return AddFromAssemblyOf<T>(NoFilter);
		}

		/// <summary>
		/// Instantiate and add all unparametrized invariant types declared in the same assembly as type <typeparamref name="T"/>
		/// which are applicable to <typeparamref name="TSubject"/> and satisfy the provided <paramref name="filter"/> to the invariant set.
		/// </summary>
		/// <typeparam name="T">The type for which all unparametrized invariant types declared in the same assembly and applicable to <typeparamref name="TSubject"/> should be added to the invariant set.</typeparam>
		/// <param name="filter">Filter predicate which must be satisfied by unparametrized invariant types before being instantiated and added to the invariant set.</param>
		public InvariantSetFor<TSubject> AddFromAssemblyOf<T>(Func<Type, bool> filter) {
			return AddFrom(typeof(T).Assembly);
		}

		/// <summary>
		/// Instantiate and add all unparametrized invariant types declared in the provided <paramref name="assembly"/>
		/// which are applicable to <typeparamref name="TSubject"/> to the invariant set.
		/// </summary>
		/// <param name="assembly">The assembly wherein all declared unparametrized invariant types applicable to <typeparamref name="TSubject"/> should be added to the invariant set.</param>
		public InvariantSetFor<TSubject> AddFrom(Assembly assembly) {
			return AddFrom(assembly, NoFilter);
		}

		/// <summary>
		/// Instantiate and add all unparametrized invariant types declared in the provided <paramref name="assembly"/>
		/// which are applicable to <typeparamref name="TSubject"/> to the invariant set.
		/// </summary>
		/// <param name="assembly">The assembly wherein all declared unparametrized invariant types applicable to <typeparamref name="TSubject"/> should be added to the invariant set.</param>
		/// <param name="filter">Filter predicate which must be satisfied by unparametrized invariant types before being instantiated and added to the invariant set.</param>
		public InvariantSetFor<TSubject> AddFrom(Assembly assembly, Func<Type, bool> filter) {
			if(null == assembly) throw Xception.Because.ArgumentNull(() => assembly);

			foreach(var unparametrizedInvariant in InvariantUtils.FindUnparametrizedInvariantsFor<TSubject>(assembly)) {
				// Prevent double addition of invariants
				if(_addedUnparametrizedInvariants.Contains(unparametrizedInvariant))
					continue;

				if(!filter(unparametrizedInvariant))
					continue;

				Add((InvariantOn<TSubject>)Activator.CreateInstance(unparametrizedInvariant));

				_addedUnparametrizedInvariants.Add(unparametrizedInvariant);
			}

			return this;
		}

		/// <summary>
		/// Assert that all loaded invariants hold on the provided <paramref name="subject"/> instance.
		/// </summary>
		/// <param name="subject">The subject instance for which the satisfaction of the loaded invariants should be verified.</param>
		/// <exception cref="InvariantViolationException"/>
		public void AssertSatisifiedBy(TSubject subject) {
			foreach(var invariant in _invariants)
				if(!invariant.IsSatisfiedBy(subject))
					throw new InvariantViolationException(invariant, subject);
		}

		public bool IsSatisfiedBy(TSubject subject) {
			return _invariants.All(invariant => invariant.IsSatisfiedBy(subject));
		}
	}
}
