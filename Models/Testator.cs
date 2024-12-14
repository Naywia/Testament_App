namespace Testament_App.Models;

public class Testator : Person
{
    private List<Asset> separateAssets = new();

    public void AddAsset(Asset asset)
    {
        separateAssets.Add(asset);
    }

    public List<Asset> GetAssets()
    {
        return separateAssets;
    }

    public void DeleteAsset(Asset asset)
    {
        separateAssets.Remove(asset);
    }
}