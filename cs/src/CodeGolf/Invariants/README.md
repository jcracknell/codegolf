# Invariants

This in my attempt at a simple library for opt-in implementation of [class invariants](https://en.wikipedia.org/wiki/Class_invariant) implementable as standalone artifacts in a code base.

The `IInvariant<TSubject>` interface is used to declare an invariant which must hold on an instance of `TSubject`.
`InvariantFor<TSubject> : IInvariant<TSubject>` is an abstract base class requiring only the implementation of a satisfaction predicate.

The `InvariantSetFor<TSubject>` class is used to assert the satisfaction of invariants by instances of `TSubject`.
It can (by default) auto-populate itself with implementations of `InvariantOn<TSubject>` declaring a public parameterless constructor.
Calling `InvariantSetFor<TSubject>::AssertSatisfiedBy(TSubject subject):void` throws an `InvariantViolationException` in the event that any invariant in the set is unsatisfied by the subject.

My motivation for this was a scenario in which I wanted to be able to perform a sanity check on an unknown implementation of an interface.
This approach allows me to explicitly declare invariants, and have them automatically loaded and applied at selected points:

```cs
class MyInterfaceUser {
	private static readonly InvariantSetFor<ISomeInterface> _someInterfaceInvariants = new InvariantSetFor<ISomeInterface>();

	public void DoSomething(ISomeInterface arg) {
		_someInterfaceInvariants.AssertSatisfiedBy(arg);

		...
	}
}
```