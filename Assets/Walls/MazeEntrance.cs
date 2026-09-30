using UnityEngine;

// Crea una entrada de madera (dos postes + viga) para el laberinto.
// 1) Pon este script en un GameObject vacío colocado donde quieres la entrada.
// 2) Arrastra tu material de madera al campo "Wood Material".
// 3) Clic derecho sobre el nombre del componente -> "Build Entrance".
// Las piezas quedan como hijos, así que puedes moverlas o editarlas después.
public class MazeEntrance : MonoBehaviour
{
    public Material woodMaterial;
    public float width = 4f;          // ancho interior de la puerta
    public float height = 3.5f;       // altura de los postes
    public float postThickness = 0.5f;
    public float beamHeight = 0.6f;
    public float depth = 0.6f;
    public bool extraTopBeam = true;  // segunda viga decorativa arriba

    [ContextMenu("Build Entrance")]
    public void Build()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        float half = width / 2f + postThickness / 2f;
        MakePart("PosteIzq", new Vector3(-half, height / 2f, 0), new Vector3(postThickness, height, depth));
        MakePart("PosteDer", new Vector3(half, height / 2f, 0), new Vector3(postThickness, height, depth));
        MakePart("Viga", new Vector3(0, height + beamHeight / 2f, 0),
                 new Vector3(width + postThickness * 3f, beamHeight, depth * 1.2f));
        if (extraTopBeam)
            MakePart("VigaSuperior", new Vector3(0, height + beamHeight * 1.5f, 0),
                     new Vector3(width + postThickness * 4f, beamHeight * 0.6f, depth * 1.4f));
    }

    void MakePart(string partName, Vector3 localPos, Vector3 scale)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = partName;
        go.transform.SetParent(transform, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = scale;
        if (woodMaterial != null)
            go.GetComponent<Renderer>().sharedMaterial = woodMaterial;
    }
}
