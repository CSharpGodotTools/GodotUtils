using System.Collections.Generic;

namespace GodotUtils;

public class ComponentHost
{
    private readonly List<Component> _components = [];

    public void Add(Component component)
    {
        _components.Add(component);
    }

    public void SetActive(bool active)
    {
        for (int i = 0; i < _components.Count; i++)
        {
            _components[i].SetActive(active);
        }
    }
}
