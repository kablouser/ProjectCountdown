using UnityEngine;

[System.Flags]
public enum CategorySet
{
    None = 0,
    References = 1 << 0,
    Settings = 1 << 1,
}

public class CategoryAttribute : PropertyAttribute
{
    public CategorySet categorySet;

    public CategoryAttribute(CategorySet categorySet)
    {
        this.categorySet = categorySet;
    }
}
