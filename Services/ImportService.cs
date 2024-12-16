using Newtonsoft.Json;
using Testament_App.Models;

namespace Testament_App.Services
{
    public class ImportService
    {
        private class FamilyTreeJson
        {
            public List<Testator> Testators { get; set; } = new();
            public List<Person> Heirs { get; set; } = new();
        }

        public static void ImportFamilyTreeFromJson(string json)
        {
            try
            {
                // Deserialize the JSON into FamilyTreeJson
                var familyTree = JsonConvert.DeserializeObject<FamilyTreeJson>(json);

                if (familyTree == null)
                    throw new Exception("Invalid JSON format.");

                // Clear existing data
                ClearInheritance();

                // Create a temporary dictionary to track all persons by ID
                Dictionary<int, Person> allPersons = new();

                // Add Testators
                foreach (var testator in familyTree.Testators)
                {
                    Inheritance.AddTestator(testator);
                    allPersons[testator.Id] = testator; // Track by ID
                }

                // Add Heirs
                foreach (var heir in familyTree.Heirs)
                {
                    Inheritance.AddHeir(heir);
                    allPersons[heir.Id] = heir; // Track by ID
                }

                // Establish relationships
                foreach (var person in allPersons.Values)
                {
                    // Add children
                    foreach (var childId in person.ChildrenIds)
                    {
                        if (allPersons.TryGetValue(childId, out var child))
                        {
                            person.AddChild(child);
                        }
                    }

                    // Add parents
                    foreach (var parentId in person.ParentIds)
                    {
                        if (allPersons.TryGetValue(parentId, out var parent))
                        {
                            person.AddParent(parent);
                        }
                    }
                }

                Console.WriteLine("Family tree imported successfully!");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing family tree: {ex.Message}");
            }
        }

        private static void ClearInheritance()
        {
            foreach (var testator in Inheritance.GetTestators())
            {
                Inheritance.DeleteTestator(testator);
            }

            foreach (var heir in Inheritance.GetHeirs())
            {
                Inheritance.DeleteHeir(heir);
            }
        }
    }
}
