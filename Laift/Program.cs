using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

public class Program
{
    public static void Main(string[] args)
    {
        var liftCount = 0;
        var liftCapacity = 0;
        int floors = 0;

        Console.WriteLine("Please Enter the number of Lifts to Install");
        var liftCountInput = Console.ReadLine();

        while (true)
        {

            if (!int.TryParse(liftCountInput, out liftCount))
            {
                Console.WriteLine("Please Enter a valid numerical value");
                liftCountInput = Console.ReadLine();
                continue;
            }
            if (liftCount > 20)
            {
                Console.WriteLine("Please Enter a numerical value not greater than 20");
                liftCountInput = Console.ReadLine();
                continue;
            }
            else if (liftCount <= 0)
            {
                Console.WriteLine("Please Enter a numerical value above 0");
                liftCountInput = Console.ReadLine();
                continue;
            }
            break;
        }

        Console.WriteLine("Please Enter the maximum people the lifts can hold");
        var liftCapacityInput = Console.ReadLine();
        while (true)
        {

            if (!int.TryParse(liftCapacityInput, out liftCapacity))
            {
                Console.WriteLine("Please Enter a valid number");
                liftCapacityInput = Console.ReadLine();
                continue;
            }
            if (liftCapacity > 20 || liftCapacity < 1)
            {
                Console.WriteLine("Please Enter a figure between 1-20");
                liftCapacityInput = Console.ReadLine();
                continue;
            }
            break;
        }

        Console.WriteLine("Please Enter the number of Floors");
        var floorCountInput = Console.ReadLine();
        while (true)
        {

            if (!int.TryParse(floorCountInput, out floors))
            {
                Console.WriteLine("Please Enter a valid numerical value");
                floorCountInput = Console.ReadLine();
                continue;
            }
            if (floors > 50)
            {
                Console.WriteLine("Please Enter a numerical value not greater than 50");
                floorCountInput = Console.ReadLine();
                continue;
            }
            else if (floors <= 0)
            {
                Console.WriteLine("Please Enter a numerical value above 0");
                floorCountInput = Console.ReadLine();
                continue;
            }
            break;
        }

        LiftService service = new LiftService(liftCount, floors, liftCapacity);

        while (true)
        {
            Console.WriteLine("Enter 1 to request lift or press ENTER to update lift positions. Press CRTL-C to exit");

            string choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                service.UpdateLifts();
                service.Visualize(liftCapacity, service.GetLifts());
            }
            else if (choice == "1")
            {
                int requestedFloor = 0;
                int targetFloor = 0;
                int peopleCount = 0;
                while (true)
                {
                    Console.WriteLine("Enter the floor you are in: ");
                    var requestedFloorInput = Console.ReadLine();
                    if (!int.TryParse(requestedFloorInput, out requestedFloor))
                    {
                        Console.WriteLine("Please enter a valid numerical figure");
                        continue;
                    }
                    if (requestedFloor < 0 || requestedFloor > floors)
                    {
                        Console.WriteLine("Floor not found");
                        continue;
                    }
                    break;
                }

                while (true)
                {
                    Console.WriteLine("Enter the floor you want to go to: ");
                    var targetFloorInput = Console.ReadLine();
                    if (!int.TryParse(targetFloorInput, out targetFloor))
                    {
                        Console.WriteLine("Please enter a valid numerical figure");
                        continue;
                    }
                    if (targetFloor < 0 || targetFloor > floors)
                    {
                        Console.WriteLine("Floor not found");
                        continue;
                    }
                    else if (requestedFloor == targetFloor)
                    {
                        Console.WriteLine("Please Enter a different target floor");
                        continue;
                    }
                    break;
                }

                while (true)
                {
                    Console.WriteLine("How many people are you?");
                    var peopleCountInput = Console.ReadLine();
                    if (!int.TryParse(peopleCountInput, out peopleCount))
                    {
                        Console.WriteLine("Please enter a valid numerical figure");
                        continue;
                    }
                    if (peopleCount < 1 || peopleCount > 20)
                    {
                        Console.WriteLine("Please Enter a figure between 1-20");
                        continue;
                    }

                    var lifts = service.GetLifts();
                    if (lifts.Where(c => liftCapacity - c.PassengerCount >= peopleCount).Count() == 0)
                    {
                        if (lifts.Sum(c => liftCapacity - c.PassengerCount) >= peopleCount)
                        {
                            bool temp = true;
                            while (true)
                            {
                                Console.WriteLine("You cannot all travel in the same lift. Enter 1 to auto-schedule different lifts, or 2 to do it manually");
                                var separateLiftInput = Console.ReadLine();

                                if (!int.TryParse(separateLiftInput, out int separateLift))
                                {
                                    Console.WriteLine("Invalid Input");
                                    continue;
                                }

                                if (separateLift == 1)
                                {

                                    while (peopleCount != 0)
                                    {
                                        int assignedLiftId = service.AssignLift(requestedFloor, targetFloor, peopleCount, out int assignedCount);
                                        peopleCount -= assignedCount;
                                        Console.WriteLine($"{assignedCount} people have been assigned to Lift {assignedLiftId}");
                                    }
                                    break;
                                }
                                else if (separateLift == 2)
                                {
                                    Console.WriteLine("Try to put a lesser number of people");
                                    temp = false;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Input");
                                    continue;
                                }
                            }
                            if (!temp)
                            {
                                continue;
                            }

                        }
                        else
                        {
                            Console.WriteLine("Total capacity in all the lifts cannot handle that number at the moment");
                        }
                    }
                    else
                    {
                        int assignedLiftId = service.AssignLift(requestedFloor, targetFloor, peopleCount, out int assignedCount);
                        if (assignedLiftId == -1)
                            Console.WriteLine("All lifts are full. Please try later");
                        Console.WriteLine($"{assignedCount} people have been assigned to Lift {assignedLiftId}");
                    }






                    break;
                }



            }
            else
            {
                Console.WriteLine($"Invalid Choice");
            }
        }
    }
}

public class LiftRequest
{
    public int PickupFloor { get; set; }
    public int TargetFloor { get; set; }
    public int PassengerCount { get; set; }
    public bool Processed { get; set; }

}

public class Lift
{
    public int Number { get; set; }
    public int CurrentFloor { get; set; }
    public bool? IsMovingUp { get; set; }
    public int PassengerCount { get; set; }
    public List<LiftRequest> TargetFloors { get; set; } = new List<LiftRequest>();
}

public class LiftService
{
    internal List<Lift> lifts;
    internal int floors;
    internal int capacity;

    public LiftService(int liftCount, int _floors, int _capacity)
    {
        floors = _floors;
        capacity = _capacity;
        lifts = new List<Lift>();

        for (int i = 1; i <= liftCount; i++)
        {
            lifts.Add(new Lift { Number = i, CurrentFloor = 0, IsMovingUp = null });
        }
    }
    public List<Lift> GetLifts()
    {
        return lifts;
    }
    public int AssignLift(int pickupFloor, int targetFloor, int groupSize, out int assignedCount)
    {
        Lift bestLift = lifts.First();
        int minDistance = int.MaxValue;

        var liftsToChoose = lifts.Where(c => capacity - c.PassengerCount >= groupSize).ToList();
        if (liftsToChoose.Count() == 0)
            liftsToChoose = lifts.Where(c => capacity != c.PassengerCount).ToList();

        if (liftsToChoose.Count == 0)
        {
            assignedCount = 0;
            return -1;
        }

        foreach (var lift in liftsToChoose)
        {
            int distance = Math.Abs(lift.CurrentFloor - pickupFloor);

            if (lift.IsMovingUp == true && lift.CurrentFloor > pickupFloor)
            {
                var greatestTargetFloor = lift.TargetFloors.OrderByDescending(c => c.TargetFloor).Select(c => c.TargetFloor).FirstOrDefault();
                var greatestPickupFloor = lift.TargetFloors.OrderByDescending(c => c.PickupFloor).Select(c => c.PickupFloor).FirstOrDefault();

                distance += Math.Max(greatestTargetFloor, greatestPickupFloor) * 2;
            }
            else if (lift.IsMovingUp == false)
            {
                var leastTargetFloor = lift.TargetFloors.OrderBy(c => c.TargetFloor).Select(c => c.TargetFloor).FirstOrDefault();
                var leastPickupFloor = lift.TargetFloors.OrderBy(c => c.PickupFloor).Select(c => c.PickupFloor).FirstOrDefault();
                distance += Math.Max(leastTargetFloor, leastPickupFloor) * 2;
            }

            if (distance < minDistance)
            {
                minDistance = distance;
                bestLift = lift;
            }
        }

        bestLift.IsMovingUp = bestLift.IsMovingUp == null ? pickupFloor > bestLift.CurrentFloor ? true : false : bestLift.IsMovingUp;


        //if (bestLift.IsMovingUp == null)
        //{
        //    if (pickupFloor > bestLift.CurrentFloor)
        //    {
        //        bestLift.IsMovingUp = true;
        //    }
        //    else if (pickupFloor < bestLift.CurrentFloor)
        //    {
        //        bestLift.IsMovingUp = false;
        //    }
        //}

        if (capacity - bestLift.PassengerCount >= groupSize)
        {
            assignedCount = groupSize;
        }
        else
        {
            assignedCount = capacity - bestLift.PassengerCount;
        }

        bestLift.TargetFloors.Add(new LiftRequest { PickupFloor = pickupFloor, TargetFloor = targetFloor, PassengerCount = assignedCount });
        bestLift.PassengerCount += assignedCount;

        return bestLift.Number;
    }

    public void UpdateLifts()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"X"); Console.ForegroundColor = ConsoleColor.White; Console.WriteLine($" : Lift Booked to full capacity");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"X"); Console.ForegroundColor = ConsoleColor.White; Console.WriteLine($" : Lift Moving Upwards");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write($"X"); Console.ForegroundColor = ConsoleColor.White; Console.WriteLine($" : Lift Moving Downwards");
        Console.WriteLine($"X : Lift Idle");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"N");
        Console.ForegroundColor = ConsoleColor.White; Console.WriteLine($" : Passenger Waiting");

        foreach (var lift in lifts)
        {
            if (lift.TargetFloors.Count > 0)
            {
                var currentFloorStats = lift.TargetFloors.Where(c => c.Processed && c.TargetFloor == lift.CurrentFloor || c.Processed == false && lift.CurrentFloor == c.PickupFloor).ToList();
                if (currentFloorStats.Count != 0)
                {
                    if (lift.TargetFloors.Where(c => c.Processed && c.TargetFloor == lift.CurrentFloor).Count() != 0)
                    {
                        Console.WriteLine($"This is the {lift.CurrentFloor} floor. You have arrived at your destination.");

                        foreach (var k in lift.TargetFloors.Where(c => c.Processed && c.TargetFloor == lift.CurrentFloor))
                        {
                            lift.PassengerCount -= k.PassengerCount;
                        }
                        lift.TargetFloors.RemoveAll(c => c.Processed && c.TargetFloor == lift.CurrentFloor);
                    }
                    if (lift.TargetFloors.Where(c => c.Processed == false && lift.CurrentFloor == c.PickupFloor).Count() != 0)
                    {
                        Console.WriteLine($"You may board lift {lift.Number}.");
                        foreach (var t in lift.TargetFloors.Where(c => c.Processed == false && lift.CurrentFloor == c.PickupFloor))
                        {
                            t.Processed = true;
                        }
                    }
                }
                else
                {
                    var allPickupsAndDestinations = lift.TargetFloors.Where(c => c.Processed == true).Select(c => c.TargetFloor).ToList();
                    allPickupsAndDestinations.AddRange(lift.TargetFloors.Where(c => c.Processed == false).Select(c => c.PickupFloor).ToList());
                    if (lift.IsMovingUp == true)
                    {
                        if (allPickupsAndDestinations.Any(c => c > lift.CurrentFloor))
                        {
                            lift.CurrentFloor++;
                        }
                        else
                        {
                            lift.IsMovingUp = false;
                            lift.CurrentFloor--;
                        }
                    }
                    else if (lift.IsMovingUp == false)
                    {
                        if (allPickupsAndDestinations.Any(c => c < lift.CurrentFloor))
                        {
                            lift.CurrentFloor--;
                        }
                        else
                        {
                            lift.IsMovingUp = true;
                            lift.CurrentFloor++;
                        }
                    }
                

                }
            }
            else
            {
                lift.IsMovingUp = null;
            }
        }

    }

    public void Visualize(int capacity, List<Lift> lifts)
    {
        Console.WriteLine();



        for (int floor = floors; floor >= 0; floor--)
        {

            if (lifts.SelectMany(c => c.TargetFloors).Any(c => c.Processed == false && c.PickupFloor == floor))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{floor,3} ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"|");
            }
            else
            {
                Console.Write($"{floor,3} |");
            }

            foreach (var lift in lifts)
            {
                if (lift.CurrentFloor == floor)
                {
                    if (lift.PassengerCount == capacity)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (lift.IsMovingUp == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (lift.IsMovingUp == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }

                    Console.Write(" X ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|");
                }
                else
                {
                    Console.Write("   |");
                }
            }

            Console.WriteLine();
        }

        Console.Write("     ");

        for (int i = 0; i < lifts.Count; i++)
        {
            Console.Write($"L{i + 1,2} ");
        }

        Console.WriteLine("\n");
    }
}


