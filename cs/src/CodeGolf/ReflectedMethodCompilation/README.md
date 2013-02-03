# ReflectedMethodComplation

This is an experimental facility for easy compilation of `C#` lambda expressions from a `MethodInfo` you got from who knows where. Given a function of the form:

```cs
class Clazz {
	int SomeFunc(string s, int i) { ... }
}
```

Calling:

```cs
var compiled = ReflectedMethodCompilation.CompileFunction(typeof(Clazz).GetMethod("SomeFunc"));
```

will get you a compiled lambda expression, the implementation of which looks something like:

```cs
function object SomeFuncWrapper(object site, object[] args) {
	return ((Clazz)site).SomeFunc((string)args[0], (int)args[1]);
}
```

This works for methods that are any combination of generic/static/void, but not for methods with ref/out params.
Experimentation suggests that the performance gains offset the cost of compi