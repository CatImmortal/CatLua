using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLua;
public class Entry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TextAsset main = Resources.Load<TextAsset>("Main");
        BinaryChunk chunk = new BinaryChunk();
        chunk.Undump(main.bytes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
