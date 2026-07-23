using System;
using System.Collections.Generic;

public enum IDType
{
    Invalid,
    Bird,
}

[Serializable]
public struct ID : IEquatable<ID>
{
    public IDType type;
    public int index;
    public int version;

    bool IEquatable<ID>.Equals(ID other)
    {
        return
            type == other.type &&
            index == other.index &&
            version == other.version;
    }

    public static bool operator ==(in ID a, in ID b)
    {
        return
            a.type == b.type &&
            a.index == b.index &&
            a.version == b.version;
    }

    public static bool operator !=(in ID a, in ID b)
    {
        return
            a.type != b.type ||
            a.index != b.index ||
            a.version != b.version;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(type, index, version);
    }

    public override bool Equals(object obj)
    {
        // avoid this at all costs, it boxes structs unnessarily
        // try to use IEquatable<ID>.Equals() as an alternative
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"{type}|{index}:{version}";
    }
}

public interface IPoolFuncs<TElement, TSpawnData>
{
    public abstract TElement Instantiate(ID id, Main main, TSpawnData spawnData);
    public abstract void SetActive(ID id, bool isActive, ref TElement t, TSpawnData spawnData);
}

[Serializable]
public struct Pool<TElement, TPoolFuncs, TSpawnData>
    where TPoolFuncs : struct, IPoolFuncs<TElement, TSpawnData>
    where TSpawnData : struct
{
    // all have the same lengths
    public List<TElement> elements;
    public List<int> versions;
    public List<bool> isUsing;
    public IDType type;

    #region Interface
    // enumerator over using indices
    public VersionedPoolUsingEnumerator<TElement, TPoolFuncs, TSpawnData> GetEnumerator()
    {
        return new VersionedPoolUsingEnumerator<TElement, TPoolFuncs, TSpawnData>(this);
    }

    public ref TElement this[int index]
    {
        get => ref elements.AsSpan()[index];
    }
    #endregion

    public ID Spawn(Main main, TSpawnData spawnData)
    {
        int findUnusedIndex = isUsing.FindIndex(0, (isUsingX) => !isUsingX);
        if (0 <= findUnusedIndex)
        {
            ID id = new ID
            {
                type = type,
                index = findUnusedIndex,
                version = versions[findUnusedIndex],
            };
            isUsing[findUnusedIndex] = true;
            new TPoolFuncs().SetActive(id, true, ref elements.AsSpan()[findUnusedIndex], spawnData);
            return id;
        }
        else
        {
            ID id = new ID
            {
                type = type,
                index = elements.Count,
                version = 0,
            };
            versions.Add(0);
            isUsing.Add(true);
            elements.Add(new TPoolFuncs().Instantiate(id, main, spawnData));
            return id;
        }
    }

    public bool TryDespawn(in ID id)
    {
#if UNITY_EDITOR
        if (id.type != type)
        {
            UnityEngine.Debug.LogError("Wrong id type, probably using wrong variable");
            return false;
        }
#endif

        if (IsValidID(id))
        {
            versions[id.index]++;
            isUsing[id.index] = false;
            new TPoolFuncs().SetActive(id, false, ref elements.AsSpan()[id.index], new TSpawnData());
            return true;
        }
        return false;
    }

    public bool IsValidID(in ID id)
    {
        return id.type == type && IsValidIndex(id.index) && versions[id.index] == id.version;
    }

    public bool IsValidIndex(int index)
    {
        return index < elements.Count && index < versions.Count && index < isUsing.Count;
    }

    public ID GetCurrentID(int index)
    {
        return new ID
        {
            type = type,
            index = index,
            version = versions[index]
        };
    }

    public int CountUsing()
    {
        int count = 0;
        foreach (var x in isUsing)
        {
            if (x)
                count++;
        }
        return count;
    }

    public void Clear()
    {
        elements.Clear();
        versions.Clear();
        isUsing.Clear();
    }

    public bool Validate()
    {
        return elements.Count == versions.Count && elements.Count == isUsing.Count;
    }
}

public struct VersionedPoolUsingEnumerator<TElement, TPoolFuncs, TSpawnData>
    where TPoolFuncs : struct, IPoolFuncs<TElement, TSpawnData>
    where TSpawnData : struct
{
    public int index;
    public Pool<TElement, TPoolFuncs, TSpawnData> pool;

    public VersionedPoolUsingEnumerator(in Pool<TElement, TPoolFuncs, TSpawnData> pool)
    {
        index = -1;
        this.pool = pool;
    }

    public ID Current
    {
        get => pool.GetCurrentID(index);
    }

    public bool MoveNext()
    {
        while (true)
        {
            index++;

            if (pool.isUsing.Count <= index)
                return false;

            if (pool.isUsing[index])
                return true;
        }
    }
    public void Reset()
    {
        index = -1;
    }
}