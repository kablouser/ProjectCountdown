using System.Collections.Generic;

[System.Serializable]
public struct VersionedList<T>
{
    public IDType type;
    public List<T> list;
    public List<int> versions;
    public List<bool> isUsing;

    public ID Add(in T t)
    {
        int notUsingIndex = isUsing.FindIndex((x) => !x);
        if (notUsingIndex == -1)
        {
            list.Add(t);
            versions.Add(0);
            isUsing.Add(true);
            return new ID { type = type, index = list.Count - 1, version = 0 };
        }
        else
        {
            isUsing[notUsingIndex] = true;
            list[notUsingIndex] = t;
            return new ID { type = type, index = notUsingIndex, version = versions[notUsingIndex] };
        }
    }

    public bool Remove(ID id)
    {
        if (IsValidID(id))
        {
            versions[id.index]++;
            isUsing[id.index] = false;
            return true;
        }
        return false;
    }

    public bool IsValidID(ID id)
    {
        return
            id.type == type &&
            id.index < versions.Count &&
            versions[id.index] == id.version;
    }

    public VersionedListEnumerator<T> GetEnumerator()
    {
        return new VersionedListEnumerator<T>(this);
    }

    public ref T this[int index]
    {
        get => ref list.AsSpan()[index];
    }
    public ref T this[ID id]
    {
        get => ref list.AsSpan()[id.index];
    }
}

public struct VersionedListEnumerator<T>
{
    public int index;
    public VersionedList<T> versionedList;

    public VersionedListEnumerator(in VersionedList<T> versionedList)
    {
        index = -1;
        this.versionedList = versionedList;
    }

    public ref T Current
    {
        get => ref versionedList.list.AsSpan()[index];
    }

    public bool MoveNext()
    {
        while (true)
        {
            index++;

            if (versionedList.isUsing.Count <= index)
                return false;

            if (versionedList.isUsing[index])
                return true;
        }
    }
    public void Reset()
    {
        index = -1;
    }
}
