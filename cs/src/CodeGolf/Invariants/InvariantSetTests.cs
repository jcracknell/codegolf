using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CodeGolf.Invariants {
	public class InvariantSetTests {
		[Fact] public void InvariantSet_Assert_on_car_with_three_wheels_should_throw_InvariantViolationException_for_CarHasFourWheelsInvariant() {
			var subject = new Car { Wheels = new object[3] };

			new InvariantSetFor<Car>()
			.Invoking(x => x.AssertSatisifiedBy(subject))
			.ShouldThrow<InvariantViolationException>()
			.And.Invariant.GetType().Should().Be(typeof(CarHasFourWheelsInvariant));
		}

		[Fact] public void InvariantSet_Assert_on_car_with_negative_weight_should_throw_InvariantViolationException_for_VehicleHasNonNegativeWeightInvariant() {
			var subject = new Car { Weight = -100 };

			new InvariantSetFor<Car>()
			.Invoking(x => x.AssertSatisifiedBy(subject))
			.ShouldThrow<InvariantViolationException>()
			.And.Invariant.GetType().Should().Be(typeof(VehicleHasNonNegativeWeightInvariant));
		}
	}
}
