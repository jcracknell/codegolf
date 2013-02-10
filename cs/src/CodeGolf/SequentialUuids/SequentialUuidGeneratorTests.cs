using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace CodeGolf.SequentialUuids {
	public class SequentialUuidGeneratorTests {

		private class NativeMethods
{
   [DllImport("rpcrt4.dll", SetLastError=true)]
   public static extern int UuidCreateSequential(out Guid guid);
}

		[Fact] public void SequentialGuidGenerator_generates_Guids_that_are_sequential() {
			var sequentialUuidGenerator = new SequentialUuidGenerator();

			var generatedIds = new List<Guid>();
			for(var i = 0; i < 10000; i++)
				generatedIds.Add(sequentialUuidGenerator.GenerateGuid());

			generatedIds.Should().BeInAscendingOrder();
		}

		[Fact] public void SequentialGuidGenerator_generates_Guids_that_are_unique() {
			var sequentialUuidGenerator = new SequentialUuidGenerator();

			int iterations = 100000;
			var generatedIds = new Guid[iterations];
			for(var i = 0; i < iterations; i++)
				generatedIds[i] = sequentialUuidGenerator.GenerateGuid();

			var hashSet = new HashSet<Guid>();
			for(var i = 0; i < iterations; i++)
				hashSet.Add(generatedIds[i]).Should().BeTrue();
		}

		[Fact] public void SequentialUuidGenerator_ExtractTimestamp_should_work() {
			var sequentialGuidGenerator = new SequentialUuidGenerator();
			var id = sequentialGuidGenerator.GenerateGuid();
			var timestamp = SequentialUuidGenerator.ExtractTimestamp(id);

			DateTime.Now.ToUniversalTime().Subtract(timestamp).Should().BeLessThan(new TimeSpan(0, 0, 1));
		}

		[Fact] public void SequentialUuidGenerator_IsSequentialUuid_should_return_true_for_value_we_generated() {
			var sequentialUuidGenerator = new SequentialUuidGenerator();

			SequentialUuidGenerator.IsSequentialUuid(sequentialUuidGenerator.GenerateGuid()).Should().BeTrue();
		}

		[Fact] public void SequentialUuidGenerator_IsSequentialUuid_should_return_false_for_NewGuid() {
			SequentialUuidGenerator.IsSequentialUuid(Guid.NewGuid()).Should().BeFalse();
		}
	}
}
