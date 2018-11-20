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
                db.SubmitChanges();

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

            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

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

        internal static List<Animal> GetPendingAdoptions()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalFromDb = db.Animals.Where(a => a.AdoptionStatus.ToLower() == "pending");
            List<Animal> pendingAdoptions = new List<Animal>(animalFromDb);
            return pendingAdoptions;
        }

        internal static List<Animal> SearchForAnimalByMultipleTraits()
        {
            Console.WriteLine("Please select the corresponding values, separated by a comma, to search our animals.");
            Console.WriteLine("By Name: 1, Animal Type: 2, Demeanor: 3, Kid Friendly: 4, Pet Friendly: 5, Gender: 6, Adoption Status: 7");
            List<string> searchCriteria = (Console.ReadLine().Split(',').ToList());
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animals = db.Animals.ToList();
            foreach (string s in searchCriteria)
            {
                int searchValue = Int32.Parse(s);
                switch (searchValue)
                {
                    case 1:
                        Console.WriteLine("Please enter the name you are looking to find.");
                        string nameToSearch = Console.ReadLine().ToLower();
                        animals.Where(a => a.Name.ToLower() == nameToSearch);
                        break;
                    case 2:
                        Console.WriteLine("Please enter 1 for Dog, 2 for Cat, 3 for Ferret, 4 for Rabbit, or 5 for Bird");
                        int animalSpecies = Int32.Parse(Console.ReadLine());
                        animals.Where(a => a.CategoryId == animalSpecies);
                        break;
                    case 3:
                        Console.WriteLine("Enter demeanor");
                        string demeanorToSearch = Console.ReadLine().ToLower();
                        animals.Where(a => a.Demeanor.ToLower() == demeanorToSearch);
                        break;
                    case 4:
                        Console.WriteLine("Kid Friendly? Y or N");
                        bool searchKidFriendly = (Console.ReadLine().ToLower() == "y");
                        animals.Where(a => a.KidFriendly == searchKidFriendly);
                        break;
                    case 5:
                        Console.WriteLine("Pet Friendly? Y or N");
                        bool searchPetFriendly = (Console.ReadLine().ToLower() == "y");
                        animals.Where(a => a.PetFriendly == searchPetFriendly);
                        break;
                    case 6:
                        Console.WriteLine("Enter gender");
                        string genderToSearch = Console.ReadLine().ToLower();
                        animals.Where(a => a.Gender.ToLower() == genderToSearch);
                        break;
                    case 7:
                        Console.WriteLine("Enter Adoption Status");
                        string adoptionStatusSearch = Console.ReadLine().ToLower();
                        animals.Where(a => a.AdoptionStatus.ToLower() == adoptionStatusSearch);
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
            return animals;
        }
        

        internal static void UpdateAdoption(bool decision, Adoption adoption)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            if (decision == true)
            {
                var acceptedAdoption = db.Animals.Where(a => a.AnimalId == adoption.AnimalId).FirstOrDefault();
                acceptedAdoption.AdoptionStatus = "Adopted";
                db.SubmitChanges();
            }
            else if (decision == false)
            {
                var declinedAdoption = db.Animals.Where(a => a.AnimalId == adoption.AnimalId).FirstOrDefault();
                declinedAdoption.AdoptionStatus = "Available";
                db.SubmitChanges();
            }    
        }

        internal static List<AnimalShot> GetShots(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalsWithShots = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).ToList();
            return animalsWithShots;
        }

        internal static void UpdateShot(string booster, Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            DateTime todaysDate = DateTime.Today;
            var animalShot = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).Single() ;
            animalShot.DateReceived = todaysDate;
            db.SubmitChanges();

        }
    }
}