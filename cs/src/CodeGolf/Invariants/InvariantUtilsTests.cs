using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CodeGolf.Invariants {
	public class InvariantUtilsTests {
		[Fact] public void InvariantUtils_FindUnparametrizedInvariantsFor_Vehicle_should_contain_VehicleHasNonNegativeWeightInvariant() {
			InvariantUtils.FindUnparametrizedInvariantsFor<Vehicle>(typeof(Vehicle).Assembly)
			.Should().Contain(typeof(VehicleHasNonNegativeWeightInvariant), "because it is an unparametrized invariant applicable to Vehicle");
		}

		[Fact] public void InvariantUtils_FindUnparametrizedInvariantsFor_Vehicle_should_not_contain_VehicleMaximumWeightInvariant() {
			InvariantUtils.FindUnparametrizedInvariantsFor<Vehicle>(typeof(Vehicle).Assembly)
			.Should().NotContain(typeof(VehicleMaximumWeightInvariant), "because it is a parametrized invariant");
		}

		[Fact] public void InvariantUtils_FindUnparametrizedInvariantsFor_Car_should_contain_CarHasFourWheelsInvariant() {
			InvariantUtils.FindUnparametrizedInvariantsFor<Car>(typeof(Vehicle).Assembly)
			.Should().Contain(typeof(CarHasFourWheelsInvariant), "because it is an unparametrized invariant applicable to Car");
		}

		[Fact] public void InvariantUtils_FindUnparametrizedInvariantsFor_Car_should_contain_VehicleHasNonNegativeWeightInvariant() {
			InvariantUtils.FindUnparametrizedInvariantsFor<Car>(typeof(Vehicle).Assembly)
			.Should().Contain(typeof(VehicleHasNonNegativeWeightInvariant), "because it is an unparametrized invariant applicable to Car");
		}

		[Fact] public void InvariantUtils_FindUnparametrizedInvariantsFor_Truck_should_not_contain_CarHasFourWheelsInvariant() {
			InvariantUtils.FindUnparametrizedInvariantsFor<Truck>(typeof(Vehicle).Assembly)
			.Should().NotContain(typeof(CarHasFourWheelsInvariant), "because it is not applicable to trucks");
		}

		[Fact] public void InvariantUtils_OverridesToString_Car_should_return_true() {
			InvariantUtils.OverridesToString(typeof(Car)).Should().BeTrue();
		}

		[Fact] public void InvariantUtils_OverridesToString_Truck_should_return_false() {
			InvariantUtils.OverridesToString(typeof(Truck)).Should().BeFalse();
		}

		[Fact] public void InvariantUtils_OverridesToString_Sedan_should_return_true() {
			InvariantUtils.OverridesToString(typeof(Sedan)).Should().BeTrue("because it inherits the override implemented by Car");
		}
	}
}
