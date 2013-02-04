using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeGolf.Invariants {
	public static class InvariantUtils {
		/// <summary>
		/// Assert that all of the provided <paramref name="invariants"/> are satisfied by provided <paramref name="subject"/> instance.
		/// </summary>
		/// <typeparam name="TSubject">The subject type to which the invariants apply.</typeparam>
		/// <param name="invariants">The invariants for which satisfaction should be asserted.</param>
		/// <param name="subject">The <typeparamref name="TSubject"/> instance which must satisfy the provided assertions.</param>
		/// <exception cref="InvariantViolationException"/>
		public static void AssertAllSatisfiedBy<TSubject>(IEnumerable<InvariantOn<TSubject>> invariants, TSubject subject) {
			if(null == invariants) throw Xception.Because.ArgumentNull(() => invariants);

			foreach(var invariant in invariants)
				if(!invariant.IsSatisfiedBy(subject))
					throw new InvariantViolationException(invariant, subject);
		}

		/// <summary>
		/// Returns all types in the provided <paramref name="assembly"/> which are invariants applicable to <typeparamref name="TSubject"/>
		/// declaring a public parameterless constructor.
		/// </summary>
		public static IEnumerable<Type> FindUnparametrizedInvariantsFor<TSubject>(Assembly assembly) {
			if(null == assembly) throw Xception.Because.ArgumentNull(() => assembly);

			return assembly.GetTypes().Where(IsUnparametrizedInvariantFor<TSubject>);
		}

		/// <summary>
		/// Returns true if the provided <paramref name="type"/> is an invariant applicable to <typeparamref name="TSubject"/>
		/// with a public parameterless constructor.
		/// </summary>
		public static bool IsUnparametrizedInvariantFor<TSubject>(Type type) {
			if(null == type) throw Xception.Because.ArgumentNull(() => type);

			return IsInvariantFor<TSubject>(type)
				&& !type.IsGenericType
				&& null != type.GetConstructor(new Type[0]);
		}

		/// <summary>
		/// Returns all types in the provided <paramref name="assembly"/> which are invariants applicable to <typeparamref name="TSubject"/>.
		/// </summary>
		public static IEnumerable<Type> FindInvariantsFor<TSubject>(Assembly assembly) {
			if(null == assembly) throw Xception.Because.ArgumentNull(() => assembly);

			return assembly.GetTypes().Where(IsInvariantFor<TSubject>);
		}

		/// <summary>
		/// Returns true if the provided <paramref name="type"/> is an invariant applicable to <typeparamref name="TSubject"/>.
		/// </summary>
		public static bool IsInvariantFor<TSubject>(Type type) {
			if(null == type) throw Xception.Because.ArgumentNull(() => type);

			return !type.IsAbstract
				&& !type.IsInterface
				&& typeof(InvariantOn<TSubject>).IsAssignableFrom(type);
		}

		/// <summary>
		/// Returns true if the provided <paramref name="type"/> overrides the default parameterless ToString implementation.
		/// </summary>
		public static bool OverridesToString(Type type) {
			var toString = type.GetMethod("ToString", new Type[0]);

			return null != toString && !typeof(object).Equals(toString.DeclaringType);
		}
	}
}
