#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

// IMPORTANTE: este archivo debe estar dentro de una carpeta llamada "Editor"
public static class EnemyCreator
{
    [MenuItem("GameObject/AI/Simple Enemy", false, 10)]
    static void CreateEnemy(MenuCommand cmd)
    {
        // Material (se guarda como asset para que no se pierda)
        Material bodyMat = GetOrCreateMaterial("Assets/EnemyBody.mat", new Color(0.2f, 0.8f, 0.2f));
        Material darkMat = GetOrCreateMaterial("Assets/EnemyDark.mat", new Color(0.1f, 0.1f, 0.1f));
        Material eyeMat  = GetOrCreateMaterial("Assets/EnemyEye.mat",  new Color(1f, 0.1f, 0.1f));

        // Raiz
        GameObject root = new GameObject("Enemy");
        root.transform.position = SceneView.lastActiveSceneView != null
            ? SceneView.lastActiveSceneView.pivot
            : Vector3.zero;

        // Cuerpo
        GameObject body = Part(PrimitiveType.Capsule, "Body", root.transform, new Vector3(0, 1f, 0), new Vector3(0.9f, 0.8f, 0.9f), bodyMat);

        // Cabeza
        Part(PrimitiveType.Sphere, "Head", root.transform, new Vector3(0, 1.95f, 0), new Vector3(0.65f, 0.65f, 0.65f), bodyMat);

        // Ojos (miran hacia +Z, que es el frente)
        Part(PrimitiveType.Sphere, "EyeL", root.transform, new Vector3(-0.15f, 2.0f, 0.27f), new Vector3(0.15f, 0.15f, 0.15f), eyeMat);
        Part(PrimitiveType.Sphere, "EyeR", root.transform, new Vector3( 0.15f, 2.0f, 0.27f), new Vector3(0.15f, 0.15f, 0.15f), eyeMat);

        // Brazos
        Part(PrimitiveType.Cube, "ArmL", root.transform, new Vector3(-0.6f, 1.2f, 0.15f), new Vector3(0.2f, 0.2f, 0.7f), darkMat);
        Part(PrimitiveType.Cube, "ArmR", root.transform, new Vector3( 0.6f, 1.2f, 0.15f), new Vector3(0.2f, 0.2f, 0.7f), darkMat);

        // Collider principal
        CapsuleCollider col = root.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0, 1f, 0);
        col.height = 2f;
        col.radius = 0.45f;

        // NavMeshAgent
        NavMeshAgent agent = root.AddComponent<NavMeshAgent>();
        agent.height = 2f;
        agent.radius = 0.45f;

        // Logica
        EnemyAI ai = root.AddComponent<EnemyAI>();
        ai.bodyRenderer = body.GetComponent<Renderer>();

        Undo.RegisterCreatedObjectUndo(root, "Create Simple Enemy");
        Selection.activeGameObject = root;
    }

    static GameObject Part(PrimitiveType type, string name, Transform parent, Vector3 localPos, Vector3 scale, Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        Object.DestroyImmediate(go.GetComponent<Collider>()); // el collider va en la raiz
        return go;
    }

    static Material GetOrCreateMaterial(string path, Color color)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        Shader shader = GraphicsSettings.currentRenderPipeline != null
            ? Shader.Find("Universal Render Pipeline/Lit")
            : Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Standard");

        mat = new Material(shader);
        mat.color = color;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }
}
#endif
