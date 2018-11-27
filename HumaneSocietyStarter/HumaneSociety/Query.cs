using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {

        internal static List<USState> GetStates()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddAnimal(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = zipCode;
                newAddress.USStateId = stateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges(); // this is where a primary key gets generated

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            // find corresponding Client from Db
            Client clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = clientAddress.Zipcode;
                newAddress.USStateId = clientAddress.USStateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).First();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static List<Adoption> GetPendingAdoptions()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus.ToLower() == "pending").ToList();
            return pendingAdoptions; 
        }

        internal static void EnterAnimalUpdate(Animal animal, Dictionary<int, string> animalInfo)
        {
            animal = new Animal();
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            animal.CategoryId = Int32.Parse(animalInfo[1]);
            animal.Name = animalInfo[2];
            animal.Age = Int32.Parse(animalInfo[3]);
            animal.Demeanor = animalInfo[4];
            animal.KidFriendly = bool.Parse(animalInfo[5]);
            animal.PetFriendly = bool.Parse(animalInfo[6]);
            animal.Weight = Int32.Parse(animalInfo[7]);
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static List<Animal> SearchForAnimalByMultipleTraits()
        {
            Console.WriteLine("Please select the corresponding values, separated by a comma, to search our animals.");
            Console.WriteLine("By Name: 1, Animal Type: 2, Demeanor: 3, Kid Friendly: 4, Pet Friendly: 5, Gender: 6, Adoption Status: 7");
            List<string> searchCriteria = (Console.ReadLine().Split(',').ToList());
            foreach (string s in searchCriteria)
            {
                switch (s)
                {
                    case "1":
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                    case "4":
                        break;
                    case "5":
                        break;
                    case "6":
                        break;
                    case "7":
                        break;
                    default:
                        Console.WriteLine("Please enter a valid response");
                        Console.ReadLine();
                        SearchForAnimalByMultipleTraits();
                        break;
                }
            }
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animals = db.Animals.ToList();
            List<Animal> filteredAnimals = animals;
            foreach (string s in searchCriteria)
            {
                int searchValue = Int32.Parse(s);
                switch (searchValue)
                {
                    case 1:
                        Console.WriteLine("Please enter the name you are looking to find.");
                        string nameToSearch = Console.ReadLine().ToLower();
                        filteredAnimals = db.Animals.Where(a => a.Name.ToLower() == nameToSearch).ToList();
                        break;
                    case 2:
                        Console.WriteLine("Please enter 1 for Dog, 2 for Cat, 3 for Ferret, 4 for Rabbit, or 5 for Bird");
                        int animalSpecies = Int32.Parse(Console.ReadLine());
                        filteredAnimals = db.Animals.Where(a => a.CategoryId == animalSpecies).ToList();
                        break;
                    case 3:
                        Console.WriteLine("Enter demeanor");
                        string demeanorToSearch = Console.ReadLine().ToLower();
                        filteredAnimals = db.Animals.Where(a => a.Demeanor.ToLower() == demeanorToSearch).ToList();
                        break;
                    case 4:
                        Console.WriteLine("Kid Friendly? Y or N");
                        bool searchKidFriendly = (Console.ReadLine().ToLower() == "y");
                        filteredAnimals = db.Animals.Where(a => a.KidFriendly == searchKidFriendly).ToList();
                        break;
                    case 5:
                        Console.WriteLine("Pet Friendly? Y or N");
                        bool searchPetFriendly = (Console.ReadLine().ToLower() == "y");
                        filteredAnimals = db.Animals.Where(a => a.PetFriendly == searchPetFriendly).ToList();
                        break;
                    case 6:
                        Console.WriteLine("Enter gender");
                        string genderToSearch = Console.ReadLine().ToLower();
                        filteredAnimals = db.Animals.Where(a => a.Gender.ToLower() == genderToSearch).ToList();
                        break;
                    case 7:
                        Console.WriteLine("Enter Adoption Status");
                        string adoptionStatusSearch = Console.ReadLine().ToLower();
                        filteredAnimals = db.Animals.Where(a => a.AdoptionStatus.ToLower() == adoptionStatusSearch).ToList();
                        break;
                    default:
                        Console.WriteLine("No animals match your search.");
                        Console.WriteLine("Would you like to try again? Y or N?");
                        var searchAgain = Console.ReadLine().ToLower();
                        if (searchAgain == "y")
                        {
                            SearchForAnimalByMultipleTraits();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("No bananas are sold here.");
                            Console.ReadLine();
                            break;
                        }
                }
            }
            animals = filteredAnimals;
            return animals;
        }

        internal static void UpdateAdoption(bool decision, Adoption adoption)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            if (decision == true)
            {
                var acceptedAdoption = db.Animals.Where(a => a.AnimalId == adoption.AnimalId).FirstOrDefault();
                acceptedAdoption.AdoptionStatus = "Adopted";
                adoption.ApprovalStatus = "Adopted";
                db.SubmitChanges();
            }
            else if (decision == false)
            {
                var declinedAdoption = db.Animals.Where(a => a.AnimalId == adoption.AnimalId).FirstOrDefault();
                declinedAdoption.AdoptionStatus = "Available";
                adoption.ApprovalStatus = "Available";
                db.SubmitChanges();
            }    
        }

        internal static List<AnimalShot> GetShots(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            List<AnimalShot> animalsWithShots = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).ToList();
            return animalsWithShots;
        }

        internal static void UpdateShot(int booster, Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            //var animalShot = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).SingleOrDefault();
          
            AnimalShot shot = new AnimalShot();
            shot.AnimalId = animal.AnimalId;
            shot.ShotId = booster;
            shot.DateReceived = DateTime.Now;
            db.AnimalShots.InsertOnSubmit(shot);

            db.SubmitChanges();
        }

        internal static int GetCategoryId()
        {
            Console.WriteLine("Please select the corresponding value, to assign an animal to a category id.");
            Console.WriteLine("Dog, Cat, Ferret, Rabbit, Bird");
            string animalType = Console.ReadLine();
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            int id = db.Categories.Where(c => c.Name == animalType).Select(c => c.CategoryId).FirstOrDefault();
            return id;
        }

        internal static int GetDietPlanId()
        {
            Console.WriteLine("Please select the corresponding name, to assign an animal to a diet plan id.");
            Console.WriteLine("Large breed dog, Small breed dog, Cat, Rabbit, Ferret, Bird");
            string dietPlanType = Console.ReadLine();
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            
            DietPlan newDietPlan = db.DietPlans.Where(d => d.Name == dietPlanType).FirstOrDefault();
            if (newDietPlan == null)
            {
                DietPlan newPlan = new DietPlan();
                Console.WriteLine("What is the foodtype of the diet?");
                newPlan.FoodType = Console.ReadLine();
                Console.WriteLine("Enter the food amount in cups.");
                newPlan.FoodAmountInCups = Int32.Parse(Console.ReadLine());
                newPlan.Name = dietPlanType;

                db.DietPlans.InsertOnSubmit(newPlan);
                db.SubmitChanges();
                return newPlan.DietPlanId;
            }
            else
            {
                return newDietPlan.DietPlanId;
            }
           
        }   
        internal static void RunEmployeeQueries(Employee employee, string queryAction)
        {

        }

        internal static Room GetRoom(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Room animalRoom = db.Rooms.Where(r => r.AnimalId == animal.AnimalId).FirstOrDefault();
            
            return animalRoom;
        }

        internal static void RemoveAnimal(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Animal theAnimal = db.Animals.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();
            db.Animals.DeleteOnSubmit(theAnimal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int ID)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Animal animal = db.Animals.Where(a => a.AnimalId == ID).Select(a => a).Single();
            return animal;
        }

        internal static void Adopt(Animal animal, Client client)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalAdoption = db.Adoptions.Where(a => a.AnimalId == animal.AnimalId).Single();
            animalAdoption.ClientId = client.ClientId;
            animal = db.Animals.Where(a => (a.Name == animal.Name) && (animal.AdoptionStatus.ToLower() == "available")).Select(a => a).FirstOrDefault();
            animal.AdoptionStatus = "Pending";
            db.SubmitChanges();
            Animal animalToAdopt = animal;
            Client currentClient = client;
            Adoption adoption = new Adoption();
            animalToAdopt = db.Animals.Where(a => a.Name == animalToAdopt.Name).Select(a => a).FirstOrDefault();
            if (animalToAdopt.AdoptionStatus.ToLower() == "available")
            {
                db.Adoptions.InsertOnSubmit(adoption);
                animalToAdopt.AdoptionStatus = "Pending";
                adoption.ClientId = client.ClientId;
                db.SubmitChanges();
            }
            else
            {
                Console.WriteLine("This animal is not available for adoption");
                Console.ReadLine();               
            }
        }

        internal static void SetRoom(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Random random = new Random();
            Room room = db.Rooms.Where(r => r.RoomId == random.Next(15)).FirstOrDefault();
            if (room.AnimalId == null)
            {
                room.AnimalId = animal.AnimalId;
            }
            else
            {
                SetRoom(animal);
            }
            db.SubmitChanges();
        }

        internal static void CheckRoomAvailability()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            List<Room> availableRooms = new List<Room>();
            foreach (Room a in db.Rooms)
            {
                availableRooms = db.Rooms.Where(r => r.AnimalId == null).Select(r=>r).ToList();
            }
            if (availableRooms.Count == 0)
            {
                Console.WriteLine("Unfortunately, the Humane Society is unable to accept any new animals at this time.");
                Console.ReadLine();
                Console.Clear();
                PointOfEntry.Run();
            }
        }
    }
}