using System.Collections;
using System.Collections.Generic;
using System.Linq;

#region Monster
[System.Serializable]
public class MonsterPool
{
    public int size;
    public List<MonsterRawData> data;
    public int GetSize()
    {
        return size;
    }
    public List<MonsterRawData> GetAllMonsters()
    {
        return data;
    }
    public MonsterRawData GetMonsterById(string id)
    {
        MonsterRawData a = data.Where(b => b.id == id).FirstOrDefault();
        return a;
    }
    public MonsterRawData GetMonsterByNumber(int number)
    {
        MonsterRawData tmp = null;
        if (number < data.Count())
        {
            tmp = data[number];
        }
        return tmp;
    }
    public override string ToString()
    {
        string output = "";
        output += "------------------------\n";
        output += data[0].ToString() + "\n";
        //foreach (MonsterRawData a in data)
        //{
        //    output += a.ToString() + "\n";
        //}
        return output;
    }
}

[System.Serializable]
public class MonsterRawData
{
    public string id;
    public string userId;
    public string name;
    public string asset;
    public MonsterStausRawData status;
    public string mType;
    public string level;
    public string isAlive;
    public MonsterEvolve evolve;

    public override string ToString()
    {
        string output = "";
        output += "id: " + id + " | ";
        output += "userId: " + userId + " | ";
        output += "name: " + name + " | ";
        output += "asset: " + asset + " | ";
        output += "Status{ " + status + " } ";
        return output;
    }
}
[System.Serializable]
public class MonsterStausRawData
{
    public MonsterStausValue hungry;
    public MonsterStausValue cleanliness;
    public MonsterStausValue healthy;
    public MonsterStausValue experience;
    public MonsterStausValue happiness;

    public override string ToString()
    {
        string output = "";
        output += "Hungry: " + hungry.value + " | ";
        output += "cleanliness: " + cleanliness.value + " | ";
        output += "happiness: " + happiness.value + " | ";
        output += "experience: " + experience.value + " | ";
        output += "healthy: " + healthy.value + " | ";
        return output;
    }
}
[System.Serializable]
public class MonsterStausValue
{
    public int value;
    public int maxValue;
    public int timestamp;
}

[System.Serializable]
public class MonsterAsset
{
    public string material;
    public string model;

    public MonsterAsset(string material, string model)
    {
        this.material = material;
        this.model = model;
    }

    public override string ToString()
    {
        string output = "";
        output += "material: " + material + " | ";
        output += "model: " + model + " | ";
        return output;
    }
}
[System.Serializable]
public class MonsterEvolve
{
    public bool canEvolve;
    public string nextType;
    public string nextLevel;
}

#endregion
#region wallet
[System.Serializable]
public class WalletPool
{
    public int size;
    public List<WalletRawData> data;
    public WalletRawData GetWalletById(string id)
    {
        WalletRawData a = data.Where(b => b.id == id).FirstOrDefault();
        return a;
    }
    public WalletRawData GetFirstWallet()
    {
        return data[0];
    }
    public override string ToString()
    {
        string output = "";
        output += "------------------------\n";
        foreach (WalletRawData a in data)
        {
            output += a.ToString() + "\n";
        }
        return output;
    }
}

[System.Serializable]
public class WalletRawData
{
    public string id;
    public string userId;
    public List<CurrenciesRawData> currencies;

    public override string ToString()
    {
        string output = "";
        output += id + " / " + currencies[0].value;
        return output;
    }
}
[System.Serializable]
public class CurrenciesRawData
{
    public string cType;
    public int value;
}

#endregion
#region item
[System.Serializable]
public class ItemPool
{
    public int size;
    public List<ItemRawData> data;
    public ItemRawData GetItemById(string id)
    {
        ItemRawData a = data.Where(b => b.id == id).FirstOrDefault();
        return a;
    }
    public ItemRawData GetItemByNumber(int number)
    {
        ItemRawData tmp = null;
        if (number < data.Count())
        {
            tmp = data[number];
        }
        return tmp;
    }
    public override string ToString()
    {
        string output = "";
        output += "------------------------\n";
        foreach (ItemRawData a in data)
        {
            output += a.ToString() + "\n";
        }
        return output;
    }
}

[System.Serializable]
public class ItemRawData
{
    public string id;
    public string name;
    public string asset;
    public string iType;
    public int quantity;
    public List<PriceRawData> price;
    public string status;
    public string owner;
    public List<EffectRawData> effect;

    public override string ToString()
    {
        string output = "";
        output += id + " / " + name;
        return output;
    }
}
[System.Serializable]
public class PriceRawData
{
    public string cType;
    public int value;
}
[System.Serializable]
public class EffectRawData
{
    public string status;
    public int effect;
}
#endregion

#region Data sending
[System.Serializable]
public class MonsterCare
{
    public MonsterCareActivities activities;
    public int experience;

    public MonsterCare(MonsterCareActivities act, int experience)
    {
        this.activities = act;
        this.experience = experience;
    }
}
[System.Serializable]
public class MonsterCareActivities
{
    public string status;
    public int effect;
    
    public MonsterCareActivities(string status, int effect)
    {
        this.status = status;
        this.effect = effect;
    }
}
[System.Serializable]
public class CreatingMonster
{
    public string userId;
    public string name;
    public string asset;

    public CreatingMonster(string userId, string name, string asset)
    {
        this.userId = userId;
        this.name = name;
        this.asset = asset;
    }
}
[System.Serializable]
public class SignInForm
{
    public string username;
    public string password;

    public SignInForm(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
[System.Serializable]
public class SignInResponse
{
    public string id;
    public string username;
    public string token;

    public SignInResponse(string id, string username, string token)
    {
        this.id = id;
        this.username = username;
        this.token = token;
    }
}
#endregion
