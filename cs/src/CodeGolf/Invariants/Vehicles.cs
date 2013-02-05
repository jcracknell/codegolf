using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGolf.Invariants {
	class Wheel {
		public int Mileage { get; set; }
	}

	abstract class Vehicle {
		public string LicensePlate { get; set; }
		public IEnumerable<Wheel> Wheels { get; set; }
		public int Weight { get; set; }
	}

	class Car : Vehicle {
		public override string ToString() { return "das auto"; }
	}

	class Sedan : Car { }

	class Truck : Vehicle { }

	class WheelHasPositiveMileageInvariant : InvariantFor<Wheel> {
		public override bool IsSatisfiedBy(Wheel subject) {
			return subject.Mileage >= 0;
		}
	}

	class VehicleWheelsInvariant : IInvariant<Vehicle> {
		private static readonly InvariantSetFor<Wheel> _wheelInvariants = new InvariantSetFor<Wheel>();

		public void AssertSatisfiedBy(Vehicle subject) {
			foreach(var wheel in subject.Wheels)
				_wheelInvariants.AssertSatisfiedBy(wheel);
		}
	}

	class VehicleHasNonNegativeWeightInvariant : InvariantFor<Vehicle> {
		public override bool IsSatisfiedBy(Vehicle subject) {
			return subject.Weight >= 0;
		}
	}

	class VehicleMaximumWeightInvariant : InvariantFor<Vehicle> {
		readonly int _maximumWeight;

		public VehicleMaximumWeightInvariant(int maximumWeight) {
			_maximumWeight = maximumWeight;
		}

		public override bool IsSatisfiedBy(Vehicle subject) {
			return subject.Weight <= _maximumWeight;
		}
	}

	class CarHasFourWheelsInvariant : InvariantFor<Car> {
		public override bool IsSatisfiedBy(Car subject) {
			return 4 == subject.Wheels.Count();
		}
	}
}
