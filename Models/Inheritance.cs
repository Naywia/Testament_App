namespace Testament_App.Models;

public static class Inheritance
{
    private static List<Asset> assets = new ();
    private static List<Testator> _testators = new();
    private static List<Person> _heirs = new();
    //private static List<Beneficiary> _testators = new();

    public static void AddTestator(Testator testator)
    {
        if (_testators.Count < 2)
        {
            testator.Id = GetNextId();
            _testators.Add(testator);
            FamilyTree.AddMember(testator);
        }
    }

    public static Testator[] GetTestators()
    {
        return _testators.ToArray();
    }

    public static void DeleteTestator(Testator testator)
    {
        _testators.Remove(testator);
        FamilyTree.RemoveMember(testator);
    }
    
    public static void AddHeir(Person heir)
    {
        heir.Id = GetNextId();

        _heirs.Add(heir);
        FamilyTree.AddMember(heir);
    }
    
    public static Person[] GetHeirs()
    {
        return _heirs.ToArray();
    }

    public static void DeleteHeir(Person heir)
    {
        _heirs.Remove(heir);
        FamilyTree.RemoveMember(heir);
    }

    private static int GetNextId()
    {
        // Get all used IDs from both lists
        var usedIds = _testators.Select(t => t.Id)
                                .Union(_heirs.Select(h => h.Id))
                                .ToHashSet();

        // Find the next unused ID starting from 1
        int nextId = 1;
        while (usedIds.Contains(nextId))
        {
            nextId++;
        }

        return nextId;
    }
    
    public static void AddAsset(Asset asset)
    {
        assets.Add(asset);
    }
    
    public static Asset[] GetAssets()
    {
        return assets.ToArray();
    }

    public static void DeleteAsset(Asset asset)
    {
        assets.Remove(asset);
    }
}