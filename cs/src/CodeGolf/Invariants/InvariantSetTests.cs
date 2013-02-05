using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CodeGolf.Invariants {
	public class InvariantSetTests {
		[Fact] public void InvariantSet_Assert_on_car_with_three_wheels_should_throw_InvariantViolationException_for_CarHasFourWheelsInvariant() {
			var subject = new Car { Wheels = new Wheel[] { new Wheel(), new Wheel(), new Wheel() } };

			new InvariantSetFor<Car>()
			.Invoking(x => x.AssertSatisfiedBy(subject))
			.ShouldThrow<InvariantViolationException>()
			.And.Invariant.GetType().Should().Be(typeof(CarHasFourWheelsInvariant));
		}

		[Fact] public void InvariantSet_Assert_on_car_with_negative_weight_should_throw_InvariantViolationException_for_VehicleHasNonNegativeWeightInvariant() {
			var subject = new Car {
				Weight = -100,
				Wheels = new Wheel[] { new Wheel(), new Wheel(), new Wheel(), new Wheel() }
			};

			new InvariantSetFor<Car>()
			.Invoking(x => x.AssertSatisfiedBy(subject))
			.ShouldThrow<InvariantViolationException>()
			.And.Invariant.GetType().Should().Be(typeof(VehicleHasNonNegativeWeightInvariant));
		}

		[Fact] public void InvariantSet_Assert_on_car_with_wheel_with_negative_mileage_should_fail() {
			var subject = new Car {
				Wheels = new Wheel[] { 
					new Wheel(),
					new Wheel { Mileage = -100 },
					new Wheel(),
					new Wheel()
				}
			};

			new InvariantSetFor<Car>()
			.Invoking(x => x.AssertSatisfiedBy(subject))
			.ShouldThrow<InvariantViolationException>()
			.And.Invariant.GetType().Should().Be(typeof(VehicleWheelsInvariant));
		}

		[Fact] public void InvariantSet_Assert_on_car_with_wheel_with_negative_mileage_should_report_InvariantViolation_for_descendant_invariant() {
			var subject = new Car {
				Wheels = new Wheel[] { 
					new Wheel(),
					new Wheel { Mileage = -100 },
					new Wheel(),
					new Wheel()
				}
			};

			new InvariantSetFor<Car>().AssertSatisfiedBy(subject);
			new InvariantSetFor<Car>()
			.Invoking(x => x.AssertSatisfiedBy(subject))
			.ShouldThrow<InvariantViolationException>()
			.And.InnerException.Invariant.GetType().Should().Be(typeof(WheelHasPositiveMileageInvariant));
		}
	}
}
