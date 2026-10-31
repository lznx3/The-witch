using UnityEngine;
using UnityEditor;

// Curso do Jogo de Terror (Unity 5.6): aplica no Terrain o mapa de pintura do jogo original.
public static class AplicarPinturaOriginal
{
    const string CAMINHO = "Assets/Editor/pintura-terreno-fase1.bytes";
    const int RESOLUCAO = 512;

    [MenuItem("Curso Terror/Aplicar pintura original do terreno")]
    static void Aplicar()
    {
        Terrain terreno = null;
        if (Selection.activeGameObject != null)
            terreno = Selection.activeGameObject.GetComponent<Terrain>();
        if (terreno == null)
            terreno = Terrain.activeTerrain;
        if (terreno == null)
        {
            EditorUtility.DisplayDialog("Curso Terror", "Nao achei nenhum Terrain na cena. Selecione o Terrain na Hierarchy e tente de novo.", "OK");
            return;
        }

        TerrainData dados = terreno.terrainData;
        if (dados.alphamapLayers != 2)
        {
            EditorUtility.DisplayDialog("Curso Terror", "O Terrain precisa ter exatamente 2 texturas (GrassHillAlbedo e GrassRockyAlbedo, nessa ordem). Agora ele tem " + dados.alphamapLayers + ".", "OK");
            return;
        }
        if (dados.alphamapResolution != RESOLUCAO)
        {
            EditorUtility.DisplayDialog("Curso Terror", "A Control Texture Resolution do Terrain precisa ser " + RESOLUCAO + ". Agora ela e " + dados.alphamapResolution + ".", "OK");
            return;
        }

        TextAsset arquivo = AssetDatabase.LoadAssetAtPath(CAMINHO, typeof(TextAsset)) as TextAsset;
        if (arquivo == null || arquivo.bytes.Length != RESOLUCAO * RESOLUCAO * 2)
        {
            EditorUtility.DisplayDialog("Curso Terror", "Nao achei o arquivo " + CAMINHO + " (ou ele esta com o tamanho errado). Importe o pacote da Aula 1 de novo.", "OK");
            return;
        }

        byte[] b = arquivo.bytes;
        float[,,] mapa = new float[RESOLUCAO, RESOLUCAO, 2];
        for (int y = 0; y < RESOLUCAO; y++)
        {
            for (int x = 0; x < RESOLUCAO; x++)
            {
                int i = (y * RESOLUCAO + x) * 2;
                mapa[y, x, 0] = b[i] / 255f;
                mapa[y, x, 1] = b[i + 1] / 255f;
            }
        }

        Undo.RegisterCompleteObjectUndo(dados, "Aplicar pintura original");
        dados.SetAlphamaps(0, 0, mapa);
        EditorUtility.SetDirty(dados);
        Debug.Log("Curso Terror: pintura original aplicada no Terrain.");
    }
}
