using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace CodeGolf {
	public class ReflectedMethodCompilationTests {
		class Clazz {
			public string InstanceField = "foo";
			public static string StaticField;

			static Clazz() { Reset(); }
			public static void Reset() { StaticField = "foo"; }

			public int BenchmarkFunction() { return 42; }

			public string InstanceFunction(int i, int j) {
				return string.Concat(this.InstanceField,(i + 2 * j) * 3);
			}

			public string GenericInstanceFunction<T>() {
				return InstanceField + typeof(T).Name;
			}

			public static string StaticFunction(char c, int r) {
				Reset();
				return StaticField + new string(c, r);
			}

			public static IEnumerable<T> GenericStaticFunction<T>(T item) {
				Reset();
				return new T[] { item };
			}

			public void InstanceAction(int i, int j) {
				InstanceField = "bar" + (i * j);
			}

			public void GenericInstanceAction<T>() {
				InstanceField = typeof(T).Name;
			}

			public static void StaticAction(string s, int i) {
				Reset();
				StaticField = s.ToString() + (2 * i).ToString();
			}

			public static void GenericStaticAction<T>() {
				Reset();
				StaticField = typeof(T).Name;
			}
		}

		private static readonly MethodInfo InstanceFunction = typeof(Clazz).GetMethod("InstanceFunction");
		private static readonly MethodInfo GenericInstanceFunction = typeof(Clazz).GetMethod("GenericInstanceFunction");
		private static readonly MethodInfo StaticFunction = typeof(Clazz).GetMethod("StaticFunction");
		private static readonly MethodInfo GenericStaticFunction = typeof(Clazz).GetMethod("GenericStaticFunction");
		private static readonly MethodInfo InstanceAction = typeof(Clazz).GetMethod("InstanceAction");
		private static readonly MethodInfo GenericInstanceAction = typeof(Clazz).GetMethod("GenericInstanceAction");
		private static readonly MethodInfo StaticAction = typeof(Clazz).GetMethod("StaticAction");
		private static readonly MethodInfo GenericStaticAction = typeof(Clazz).GetMethod("GenericStaticAction");

		[Fact] public void ReflectedMethodCompilation_CompileFunction_should_work_for_instance_function() {
			var compiled = ReflectedMethodCompilation.CompileFunction(InstanceFunction);
			compiled(new Clazz(), new object[] { 1, 2 }).Should().Be("foo15");
		}

		[Fact] public void ReflectedMethodCompilation_CompileFunction_should_work_for_generic_function() {
			var compiled = ReflectedMethodCompilation.CompileFunction(GenericInstanceFunction, new Type[] { typeof(List<DateTime>) });
			compiled(new Clazz(), new object[0]).Should().Be("fooList`1");
		}

		[Fact] public void ReflectedMethodCompilation_CompileFunction_should_work_for_static_function() {
			var compiled = ReflectedMethodCompilation.CompileFunction(StaticFunction);
			compiled(null, new object[] { 'x', 6 }).Should().Be("fooxxxxxx");
		}

		[Fact] public void ReflectedMethodCompilation_CompileFunction_should_work_for_generic_static_function() {
			var compiled = ReflectedMethodCompilation.CompileFunction(GenericStaticFunction, new Type[] { typeof(string) });
			var result = compiled(null, new object[] { "fizz" }) as IEnumerable<string>;
			result.Should().BeEquivalentTo(new string[] { "fizz" });
		}

		[Fact] public void ReflectedMethodCompilation_CompileAction_should_work_for_instance_action() {
			var instance = new Clazz();
			var compiled = ReflectedMethodCompilation.CompileAction(InstanceAction);
			compiled(instance, new object[] { 2, 7 });

			instance.InstanceField.Should().Be("bar14");
		}

		[Fact] public void ReflectedMethodCompilation_CompileAction_should_work_for_generic_instance_action() {
			var instance = new Clazz();
			var compiled = ReflectedMethodCompilation.CompileAction(GenericInstanceAction, new Type[] { typeof(string) });

			compiled(instance, new object[0]);

			instance.InstanceField.Should().Be("String");
		}

		[Fact] public void ReflectedMethodCompilation_CompileAction_should_work_for_static_action() {
			var compiled = ReflectedMethodCompilation.CompileAction(StaticAction);
			compiled(null, new object[] { "bar", 4 });

			Clazz.StaticField.Should().Be("bar8");
		}

		[Fact] public void ReflectedMethodCompilation_CompileAction_should_work_for_generic_static_action() {
			var compiled = ReflectedMethodCompilation.CompileAction(GenericStaticAction, new Type[] { typeof(IEnumerable<string>) });
			compiled(null, new object[0]);

			Clazz.StaticField.Should().Be("IEnumerable`1");
		}

		[Fact] public void ReflectedMethodCompilation_benchmark() {
			for(var i = 10; i <= 1000000; i = i * 10)
				BenchmarkIterations(i);
		}

		private static void BenchmarkIterations(int iterations) {
			var benchmarkFunction = typeof(Clazz).GetMethod("BenchmarkFunction");
			var site = new Clazz();
			var args = new object[0];

			int i = 0;
			var stopwatch = new System.Diagnostics.Stopwatch();

			stopwatch.Reset();
			stopwatch.Start();

			for(i = 0; i != iterations; i++)
				benchmarkFunction.Invoke(site, args);

			stopwatch.Stop();

			var uncompiledRuntime = stopwatch.ElapsedTicks;

			stopwatch.Reset();
			stopwatch.Start();

			var compiled = ReflectedMethodCompilation.CompileFunction(benchmarkFunction);

			stopwatch.Stop();

			var compilationTime = stopwatch.ElapsedTicks;

			stopwatch.Reset();
			stopwatch.Start();

			for(i = 0; i != iterations; i++)
				compiled(site, args);

			stopwatch.Stop();

			var compiledRuntime = stopwatch.ElapsedTicks;

			double relative = ((double)compiledRuntime) / ((double)uncompiledRuntime) * 100d;
			double increase = 100d / relative;

			Console.WriteLine("=== " + iterations + " iterations ===");
			Console.WriteLine("Compilation: " + compilationTime);
			Console.WriteLine("Uncompiled: " + uncompiledRuntime);
			Console.WriteLine("Compiled: " + compiledRuntime);
			Console.WriteLine("Runtime Change: " + ((double)uncompiledRuntime / (double)(compilationTime + compiledRuntime)) * 100d + "%");
		}
	}
}
