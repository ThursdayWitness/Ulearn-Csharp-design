using System;
using System.Globalization;
using System.Net;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
	// In real aplication it would be the place where database is used to find driver by its Id.
	// But in this exercise it is just a mock to simulate database
	public class DriversRepository
	{
		public static Driver FillDriverToOrder(int driverId)
		{
			if(driverId != 15)
				throw new Exception("Unknown driver id " + driverId);
			var car = new Car("Baklazhan", "Lada sedan", "A123BT 66"); 
			var driver = new Driver(driverId, new PersonName("Drive", "Driverson"), car);
			return driver;
		}
	}

	public class TaxiApi : ITaxiApi<TaxiOrder>
	{
		private readonly Func<DateTime> _currentTime;
		private int _idCounter;

		public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
		{
			_currentTime = currentTime;
		}

		public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street, string building)
		{
			return new TaxiOrder(_idCounter++, new PersonName(firstName, lastName), 
				new Address(street, building), _currentTime());
		}

		public void UpdateDestination(TaxiOrder order, string street, string building)
		{
			order.UpdateDestination(new Address(street, building));
		}

		public void AssignDriver(TaxiOrder order, int driverId)
		{
			DriversRepository.FillDriverToOrder(driverId);
			order.AssignDriver(driverId, _currentTime());
		}

		public void UnassignDriver(TaxiOrder order)
		{
			order.UnassignDriver();
		}

		public string GetDriverFullInfo(TaxiOrder order)
		{
			return order.GetDriverFullInfo();
		}

		public string GetShortOrderInfo(TaxiOrder order)
		{
			return order.GetShortOrderInfo();
		}

		public void Cancel(TaxiOrder order)
		{
			order.Cancel(_currentTime());
		}

		public void StartRide(TaxiOrder order)
		{
			order.StartRide(_currentTime());
		}

		public void FinishRide(TaxiOrder order)
		{
			order.FinishRide(_currentTime());
		}
	}

	public class TaxiOrder: Entity<int>
	{
		public int orderId { get; }
		public PersonName ClientName { get; }
		public Address Start { get; }
		public Address Destination { get; private set; }
		public Driver Driver { get; private set; }
		private TaxiOrderStatus Status { get; set; }
		private DateTime CreationTime { get; }
		private DateTime DriverAssignmentTime { get; set; }
		private DateTime CancelTime { get; set; }
		private DateTime StartRideTime { get; set; }
		private DateTime FinishRideTime { get; set; }

		public TaxiOrder(int orderId, PersonName clientName, Address start, DateTime creationTime) : base(orderId)
		{
			this.orderId = orderId;
			ClientName = clientName;
			Start = start;
			CreationTime = creationTime;
			Destination = new Address(null, null);
			Driver = new Driver(0,null,null);
			Status = TaxiOrderStatus.WaitingForDriver;
		}

		private DateTime GetLastProgressTime()
		{
			switch (Status)
			{
				case TaxiOrderStatus.WaitingForDriver:
					return CreationTime;
				case TaxiOrderStatus.WaitingCarArrival:
					return DriverAssignmentTime;
				case TaxiOrderStatus.InProgress:
					return StartRideTime;
				case TaxiOrderStatus.Finished:
					return FinishRideTime;
				case TaxiOrderStatus.Canceled:
					return CancelTime;
				default:
					throw new NotSupportedException(Status.ToString());
			}
		}
		
		public string GetShortOrderInfo()
		{
			var order = $"OrderId: {orderId}";
			var status = $"Status: {Status}";
			var client = $"Client: {ClientName.FirstName} {ClientName.LastName}";
			var driver = $"Driver: {Driver.FirstName} {Driver.LastName}";
			var from = $"From: {Start.Street} {Start.Building}";
			var to = $"To: {Destination.Street} {Destination.Building}";
			var lastProgressTime = GetLastProgressTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
				
			if(Driver.FirstName == null)
				driver = $"Driver: ";
			if(Destination.Street == null)
				to = $"To: ";
			return string.Join(" ", order, status, client, driver, from, to, $"LastProgressTime: {lastProgressTime}");
		}
		
		public void UpdateDestination(Address address)
		{
			Destination = address;
		}
		
		public void AssignDriver(int driverId, DateTime currentTime)
		{
			if(Status != TaxiOrderStatus.WaitingForDriver)
				throw new InvalidOperationException($"Current status is {Status}");
			Driver = DriversRepository.FillDriverToOrder(driverId);
			DriverAssignmentTime = currentTime;
			Status = TaxiOrderStatus.WaitingCarArrival;
		}

		public void UnassignDriver()
		{
			if (Status != TaxiOrderStatus.WaitingCarArrival || Status == TaxiOrderStatus.InProgress)
				throw new InvalidOperationException($"Current status is {Status}");
			Driver = new Driver(0,null,null);
			Status = TaxiOrderStatus.WaitingForDriver;
		}

		public string GetDriverFullInfo()
		{
			if (Status == TaxiOrderStatus.WaitingForDriver) return null;
			var driverId = $"Id: {Driver.driverClassId}";
			var driver = $"DriverName: {Driver.FirstName} {Driver.LastName}";
			var carColor = $"Color: {Driver.Car.Color}";
			var carModel = $"CarModel: {Driver.Car.Model}";
			var plateNumber = $"PlateNumber: {Driver.Car.PlateNumber}";
			if(Driver.FirstName == null)
			{
				driver = $"Driver: ";
				carColor = $"Color: ";
				carModel = $"CarModel: ";
				plateNumber = $"PlateNumber: ";
			}
			return string.Join(" ", driverId, driver, carColor, carModel, plateNumber);
		}

		public void Cancel(DateTime currentTime)
		{
			if (Status == TaxiOrderStatus.InProgress)
				throw new InvalidOperationException($"Current status is {Status}");
			Status = TaxiOrderStatus.Canceled;
			CancelTime = currentTime;
		}

		public void StartRide(DateTime currentTime)
		{
			if (Status != TaxiOrderStatus.WaitingCarArrival)
				throw new InvalidOperationException($"Current status is {Status}");
			Status = TaxiOrderStatus.InProgress;
			StartRideTime = currentTime;
		}

		public void FinishRide(DateTime currentTime)
		{
			if (Status != TaxiOrderStatus.InProgress)
				throw new InvalidOperationException($"Current status is {Status}");
			Status = TaxiOrderStatus.Finished;
			FinishRideTime = currentTime;
		}
	}
	
	public class Driver : Entity<int>
	{
		public int driverClassId { get; }
		public string FirstName { get; }
		public string LastName { get; }
		public Car Car { get; }

		public Driver(int id, PersonName person, Car taxiCar) : base(id)
		{
			driverClassId = id;
			if (person == null)
			{
				FirstName = null;
				LastName = null;
			}
			else
			{
				FirstName = person.FirstName;
				LastName = person.LastName;
			}
			Car = taxiCar;
		}
	}
	
	public class Car: ValueType<Car>
	{
		public string Color;
		public string Model;
		public string PlateNumber;

		public Car(string color, string model, string plateNumber)
		{
			Color = color;
			Model = model;
			PlateNumber = plateNumber;
		}
	}
}