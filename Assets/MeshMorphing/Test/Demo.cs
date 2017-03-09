using UnityEngine;
using System.Collections;

public class Demo : MonoBehaviour
{
    public MeshMorphing meshMorphing;
    int currentIndex = 0;
    int oldIndex = -1;
    float curWeight = 0;

    public void Update ()
    {
        curWeight += 0.01f;
        if (curWeight > 1) {
            meshMorphing.ChangeWeight (currentIndex, 1);
            curWeight = 0;
            oldIndex = currentIndex;
            currentIndex += 1;
            if (currentIndex > 3) {
                currentIndex = 0;
            }
        } else {
            if (oldIndex != -1) {
                var w = meshMorphing.GetWeight (oldIndex) - 0.01f;
                if (w < 0) {
                    w = 0;
                }

                meshMorphing.ChangeWeight (oldIndex, w);
            }
            meshMorphing.ChangeWeight (currentIndex, curWeight);
        }
    }

}
