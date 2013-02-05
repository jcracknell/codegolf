using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeGolf.Invariants {
	public static class InvariantUtils {
		/// <summary>
		/// Core implementation of invariant satisfaction logic.
		/// </summary>
		public static void AssertSatisfied<TSubject>(IInvariant<TSubject> invariant, TSubject subject) {
			if(null == invariant) throw Xception.Because.ArgumentNull(() => invariant);

			try { invariant.AssertSatisfiedBy(subject); }
			catch(InvariantViolationException ive) {
				// if an IVE was thrown and the claimed invariant was not the one we just tested, then the violated
				// invariant was certainly a child of the current one, and we should wrap the exception
				if(null != ive.Invariant && !object.ReferenceEquals(invariant, ive.Invariant))
					throw new InvariantViolationException(invariant, subject, ive);

				throw;
			}
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
				&& typeof(IInvariant<TSubject>).IsAssignableFrom(type);
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
