using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CodeGolf {
	/// <summary>
	/// <para>
	/// Delegate type representing a reflected function which has been compiled for high performance invocation.
	/// </para>
	/// </summary>
	/// <param name="site">The site (instance) at which the compiled function should be invoked. Unused by static methods.</param>
	/// <param name="args">The arguments with which the compiled function should be invoked.</param>
	public delegate object CompiledReflectedFunction(object site, object[] args);

	/// <summary>
	/// Delegate type representing a reflected action which has been compiled for high performance invocation.
	/// </summary>
	/// <param name="site">The site (instance) at which the compiled action should be invoked. Unused by static methods.</param>
	/// <param name="args">The arguments with which the compiled action should be invoked.</param>
	public delegate void CompiledReflectedAction(object site, object[] args);

	/// <summary>
	/// <para>
	/// Static class providing facilities for the compilation of reflected methods for high performance invocation.
	/// </para>
	/// <para>
	/// Compilation generally takes less than a millisecond, with performance gains generally recouping the cost
	/// of compilation in less than 1000 invocations.
	/// </para>
	/// </summary>
	public static class ReflectedMethodCompilation {

		/// <summary>
		/// Compile the provided <paramref name="function"/> to a delegate.
		/// </summary>
		/// <param name="function">The function to be compiled to a delegate.</param>
		public static CompiledReflectedFunction CompileFunction(MethodInfo function) {
			if(null == function) throw Xception.Because.ArgumentNull(() => function);
			if(typeof(void) == function.ReturnType) throw Xception.Because.Argument(() => function, "has a void return type");

			var siteParameter = Expression.Parameter(typeof(object), "o");
			var argsParameter = Expression.Parameter(typeof(object[]), "arg");

			return Expression.Lambda<CompiledReflectedFunction>(
					Expression.Convert(MakeBodyExpression(function, siteParameter, argsParameter), typeof(object)),
					new ParameterExpression[] { siteParameter, argsParameter }
				)
				.Compile();
		}

		/// <summary>
		/// Compile the provided generic <paramref name="function"/> to a delegate using the provided <paramref name="typeParameters"/>.
		/// </summary>
		/// <param name="function">The generic function to be compiled to a delegate.</param>
		/// <param name="typeParameters">The type parameters with which the generic function should be compiled.</param>
		public static CompiledReflectedFunction CompileFunction(MethodInfo function, IEnumerable<Type> typeParameters) {
			if(null == function) throw Xception.Because.ArgumentNull(() => function);
			if(null == typeParameters) throw Xception.Because.ArgumentNull(() => typeParameters);
			if(typeof(void) == function.ReturnType) throw Xception.Because.Argument(() => function, "has a void return type");

			return CompileFunction(MakeGenericMethod(function, typeParameters));
		}

		/// <summary>
		/// Compile the provided <paramref name="action"/> to a delegate.
		/// </summary>
		/// <param name="action">The action to be compiled to a delegate.</param>
		public static CompiledReflectedAction CompileAction(MethodInfo action) {
			if(null == action) throw Xception.Because.ArgumentNull(() => action);
			if(typeof(void) != action.ReturnType) throw Xception.Because.Argument(() => action, "does not have a void return type");

			var siteParameter = Expression.Parameter(typeof(object), "o");
			var argsParameter = Expression.Parameter(typeof(object[]), "arg");
			var body = MakeBodyExpression(action, siteParameter, argsParameter);
			var expression = Expression.Lambda<CompiledReflectedAction>(body, new ParameterExpression[] { siteParameter, argsParameter });

			return expression.Compile();
		}

		/// <summary>
		/// Compile the provided generic <paramref name="action"/> to a delegate using the provided <paramref name="typeParameters"/>.
		/// </summary>
		/// <param name="action">The generic action which should be compiled to a delegate.</param>
		/// <param name="typeParameters">The type parameters with which the generic action should be compiled.</param>
		public static CompiledReflectedAction CompileAction(MethodInfo action, IEnumerable<Type> typeParameters) {
			if(null == action) throw Xception.Because.ArgumentNull(() => action);
			if(null == typeParameters) throw Xception.Because.ArgumentNull(() => typeParameters);
			if(typeof(void) != action.ReturnType) throw Xception.Because.Argument(() => action, "does not have a void return type");

			return CompileAction(MakeGenericMethod(action, typeParameters));
		}

		private static MethodInfo MakeGenericMethod(MethodInfo method, IEnumerable<Type> typeParameters) {
			if(!method.IsGenericMethodDefinition)
				method = method.GetGenericMethodDefinition();

			return method.MakeGenericMethod(typeParameters.ToArray());
		}

		private static Expression MakeBodyExpression(MethodInfo method, ParameterExpression siteParameter, ParameterExpression argsParameter) {
			var argumentExpressions = new LinkedList<Expression>();
			foreach(var methodParameter in method.GetParameters()) {
				Expression argumentExpression = Expression.ArrayIndex(argsParameter, Expression.Constant(methodParameter.Position));

				if(!typeof(object).Equals(methodParameter.ParameterType))
					argumentExpression = Expression.Convert(argumentExpression, methodParameter.ParameterType);

				argumentExpressions.AddLast(argumentExpression);
			}

			var siteExpression = method.IsStatic ? null : Expression.Convert(siteParameter, method.DeclaringType);

			return Expression.Call(siteExpression, method, argumentExpressions);
		}
	}
}