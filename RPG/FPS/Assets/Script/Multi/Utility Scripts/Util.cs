using UnityEngine;

public class Util
{

    //changes the layer of all the chils in the game object and the game object itself
    // Warning this method is recursive, try not calling it too often as it might use plenty of ressources.
    public static void SetLayerRecursively(GameObject _obj, int _newLayer)
    {
        if (_obj == null)
            return;

        _obj.layer = _newLayer;

        foreach (Transform _child in _obj.transform)
        {
            if (_child == null)
                continue;

            SetLayerRecursively(_child.gameObject, _newLayer);
        }
    }

}
