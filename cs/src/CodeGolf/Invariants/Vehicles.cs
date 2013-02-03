using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGolf.Invariants {
	abstract class Vehicle {
		public string LicensePlate { get; set; }
		public IEnumerable<object> Wheels { get; set; }
		public int Weight { get; set; }
	}

	class Car : Vehicle { }

	class Truck : Vehicle { }

	class VehicleHasNonNegativeWeightInvariant : InvariantOn<Vehicle> {
		public bool IsSatisfiedBy(Vehicle subject) {
			return subject.Weight >= 0;
		}
	}

	class VehicleMaximumWeightInvariant : InvariantOn<Vehicle> {
		readonly int _maximumWeight;

		public VehicleMaximumWeightInvariant(int maximumWeight) {
			_maximumWeight = maximumWeight;
		}

		public bool IsSatisfiedBy(Vehicle subject) {
			return subject.Weight <= _maximumWeight;
		}
	}

	class CarHasFourWheelsInvariant : InvariantOn<Car> {
		public bool IsSatisfiedBy(Car subject) {
			return 4 == subject.Wheels.Count();
		}
	}
}
