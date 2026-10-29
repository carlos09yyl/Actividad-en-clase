#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Herramienta de una sola vez: arma la escena Piton_Recinto con la jerarquía vacía.
/// Se usa desde el menú Piton > Crear escena del recinto. Después de usarla se puede borrar.
/// No crea carpetas, no toca capas ni otras escenas y no sobrescribe nada que ya exista.
/// </summary>
public static class PitonCrearRecinto
{
    const string NombreEscena = "Piton_Recinto";

    [MenuItem("Piton/Crear escena del recinto")]
    static void Crear()
    {
        // Ubica la carpeta raíz del grupo a partir de este mismo script (…/Scripts/PitonCrearRecinto.cs).
        string[] guids = AssetDatabase.FindAssets("PitonCrearRecinto t:MonoScript");
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Pitón", "No encontré el script PitonCrearRecinto.", "OK");
            return;
        }
        string rutaScript = AssetDatabase.GUIDToAssetPath(guids[0]);
        string carpetaScripts = System.IO.Path.GetDirectoryName(rutaScript).Replace('\\', '/');
        string carpetaRaiz = System.IO.Path.GetDirectoryName(carpetaScripts).Replace('\\', '/');
        string carpetaEscenas = carpetaRaiz + "/Escenas";

        if (!AssetDatabase.IsValidFolder(carpetaEscenas))
        {
            EditorUtility.DisplayDialog("Pitón",
                "No existe la carpeta:\n" + carpetaEscenas +
                "\n\nEste script debe estar dentro de la carpeta Scripts del grupo, junto a la carpeta Escenas.", "OK");
            return;
        }

        string rutaEscena = carpetaEscenas + "/" + NombreEscena + ".unity";
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(rutaEscena) != null)
        {
            EditorUtility.DisplayDialog("Pitón", "La escena ya existe y no se va a sobrescribir:\n" + rutaEscena, "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var gestion = Vacio("--- GESTION ---", null);
        Vacio("PitonGameManager", gestion);

        var camara = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        camara.tag = "MainCamera";
        camara.transform.position = new Vector3(0f, 0f, -10f);
        var cam = camara.GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        var grid = new GameObject("Grid", typeof(Grid));
        TilemapVacio("Tilemap_Piso", grid.transform);
        TilemapVacio("Tilemap_Agua", grid.transform);
        TilemapVacio("Tilemap_Paredes", grid.transform);
        TilemapVacio("Tilemap_Decoracion", grid.transform);

        var objetos = Vacio("Objetos", null);
        Vacio("Tronco", objetos);
        Vacio("PlataformaPiedras", objetos);
        Vacio("Rejillas", objetos);
        Vacio("LamparaCalor", objetos);
        Vacio("Rociador", objetos);
        Vacio("Letrero_QR", objetos);

        Vacio("Piton", null);
        Vacio("Guia", null);
        Vacio("UI", null);
        Vacio("Audio", null);

        EditorSceneManager.SaveScene(escena, rutaEscena);
        EditorUtility.DisplayDialog("Pitón", "Escena creada:\n" + rutaEscena, "OK");
    }

    static Transform Vacio(string nombre, Transform padre)
    {
        var go = new GameObject(nombre);
        if (padre != null)
            go.transform.SetParent(padre, false);
        return go.transform;
    }

    static void TilemapVacio(string nombre, Transform grid)
    {
        var go = new GameObject(nombre, typeof(Tilemap), typeof(TilemapRenderer));
        go.transform.SetParent(grid, false);
    }
}
#endif
