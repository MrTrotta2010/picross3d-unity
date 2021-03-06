using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    // Destaca os blocos que fazem parte da solução do puzzle
    public bool highlightCorrectBlocks = false;

    // Material do bloco destacado
    [SerializeField] private Material highlightMaterial = null;
    // Material de um bloco quebrado erroneamente
    [SerializeField] private Material brokenMaterial = null;
    // Material default
    [SerializeField] private Material defaultMaterial = null;

    // Prefab do cubo
    [SerializeField] private GameObject cubePrefab = null;

    private int[][][] puzzleSolutionMatrix; // Matriz de solução do puzzle
    private Vector3 puzzleDimensions; // Dimensões em x, y e z do puzzle
    private Vector3 puzzleOrigin; // Ponto central do puzzle;
    private int cubesLeftToBreak; // A quantidade de cubos restantes a serem quebrados

    private void Awake()
    {
        float minHeight, minWidth, minDepth, maxHeight, maxWidth, maxDepth;

        // Recupera todos os cubos do puzzle
        List<Transform> cubes = new List<Transform>(transform.childCount);

        // Atualiza as altura, larguras e profundidades mínimas e máximas
        // e calcula as dimensões do puzzle
        puzzleDimensions = CalculatePuzzleDimensions(cubes, out minHeight, out minWidth, out minDepth,
                                                            out maxHeight, out maxWidth, out maxDepth);

        // Calcula o centro do puzzle
        puzzleOrigin = CalculatePuzzleOrigin(minHeight, minWidth, minDepth, maxHeight, maxWidth, maxDepth);
        Debug.Log("Origem: " + puzzleOrigin);

        // Aloca a matriz de solução do puzzle
        puzzleSolutionMatrix = AllocatePuzzleMatrix(puzzleDimensions);

        // Preenche a matriz de solução do puzzle, criando os cubos extras e 
        // atualizando a quantidade de cubos a serem quebrados
        FillOutSolutionMatrix(cubes, maxHeight, minWidth, maxDepth);

        PrintMatrixLayer(0);
    }

    private void OnEnable()
    {
        Cube.CorrectCubeBroken += DecreaseCubesLeft;
        Cube.WrongCubeBroken += MistakeMade;
        Cube.CubePainted += PaintCube;
        Cube.CubeUnpainted += UnpaintCube;
    }

    private void OnDisable()
    {
        Cube.CorrectCubeBroken -= DecreaseCubesLeft;
        Cube.WrongCubeBroken -= MistakeMade;
        Cube.CubePainted -= PaintCube;
        Cube.CubeUnpainted -= UnpaintCube;
    }

    // Calcula as dimensões do puzzle
    private Vector3 CalculatePuzzleDimensions(List<Transform> cubes, out float minHeight, out float minWidth, out float minDepth,
                                                                out float maxHeight, out float maxWidth, out float maxDepth)
    {
        minHeight = minWidth = minDepth = maxHeight = maxWidth = maxDepth = 0;

        for (int i = 0; i < cubes.Capacity; i++)
        {
            cubes.Add(transform.GetChild(i));

            if (cubes[i].position.y > maxHeight) maxHeight = cubes[i].position.y;
            if (cubes[i].position.y < minHeight) minHeight = cubes[i].position.y;
            if (cubes[i].position.x > maxWidth) maxWidth = cubes[i].position.x;
            if (cubes[i].position.x < minWidth) minWidth = cubes[i].position.x;
            if (cubes[i].position.z > maxDepth) maxDepth = cubes[i].position.z;
            if (cubes[i].position.z < minDepth) minDepth = cubes[i].position.z;
        }

        return new Vector3(maxWidth - minWidth + 1, maxHeight - minHeight + 1, maxDepth - minDepth + 1);
    }

    private Vector3 CalculatePuzzleOrigin(float minHeight, float minWidth, float minDepth,
                                            float maxHeight, float maxWidth, float maxDepth)
    {
        return new Vector3((maxWidth + minWidth) / 2, (maxHeight + minHeight) / 2, (maxDepth + minDepth) / 2);
    }

    // Aloca a matriz tridimensional que contém a solução do puzzle
    private int[][][] AllocatePuzzleMatrix(Vector3 puzzleDimensions)
    {
        int[][][] matrix = new int[(int)puzzleDimensions.z][][];
        for (int i = 0; i < puzzleDimensions.z; i++)
        {
            matrix[i] = new int[(int)puzzleDimensions.y][];
            for (int j = 0; j < puzzleDimensions.y; j++)
            {
                matrix[i][j] = new int[(int)puzzleDimensions.x];
                for (int k = 0; k < puzzleDimensions.x; k++)
                {
                    matrix[i][j][k] = 0;
                }
            }
        }
        return matrix;
    }

    // Atribui cada um dos cubos pré-existentes a uma posição da matriz tridimensional e os torna parte da solução
    private void FillOutSolutionMatrix(List<Transform> cubes, float maxHeight, float minWidth, float maxDepth)
    {
        cubesLeftToBreak = 0;

        // Ordena a lista de cubos decrescentemente por coordenada z
        // Cubos com mesmo z são ordenados decrescentemente por coordenada z
        // Cubos com mesmo y são ordenados crescentemente por coordenada x
        cubes.Sort(delegate (Transform t1, Transform t2)
        {
            var index1 = t1.position.z.CompareTo(t2.position.z);
            if (index1 == 0)
            {
                var index2 = t1.position.y.CompareTo(t2.position.y);
                if (index2 == 0)
                {
                    return t1.position.x.CompareTo(t2.position.x);
                }
                else
                {
                    return -index2;
                }
            }
            else
            {
                return -index1;
            }
        });

        int i = 0, k = 0;
        float previousY = cubes[0].position.y, previousZ = cubes[0].position.z;

        // Associa cada cubo da solução a uma posição da matriz de solução
        // e marca o cubo como parte da solução
        for (int c = 0; c < cubes.Count; c++) // C++ rsrsrs
        {
            //// Quando o z se alterar, muda de linha e atualiza o z anterior
            //if (cubes[c].position.y != previousY)
            //{
            //    k += (int)(previousZ - cubes[c].position.z);
            //    previousZ = cubes[c].position.z;
            //}
            //// Quando o y se alterar, muda de linha e atualiza o y anterior
            //if (cubes[c].position.y != previousY)
            //{
            //    i += (int)(previousY - cubes[c].position.y);
            //    previousY = cubes[c].position.y;
            //}

            // Calcula a posição e adiciona o cubo à solução
            i = (int)(maxHeight - cubes[c].position.y);
            int j = (int)(cubes[c].position.x - minWidth);
            k = (int)(maxDepth - cubes[c].position.z);
            // Debug.Log(cubes[c].gameObject.name + " - i: " + i + ", j: " + j + ", k: " + k);
            puzzleSolutionMatrix[k][i][j] = 1;
            cubes[c].GetComponent<Cube>().SetPartOfSolution(true);

            if (highlightCorrectBlocks)
            {
                cubes[c].GetComponent<MeshRenderer>().material = highlightMaterial;
            }
        }

        // Cria um novo cubo para cada posição da matriz de solução
        // não pertencente a solução final e ajusta sua posição no puzzle
        for (k = 0; k < puzzleDimensions.z; k++)
        {
            for (i = 0; i < puzzleDimensions.y; i++)
            {
                for (int j = 0; j < puzzleDimensions.x; j++)
                {
                    if (puzzleSolutionMatrix[k][i][j] == 0)
                    {
                        Vector3 position = new Vector3(j + minWidth, maxHeight - i, maxDepth - k);
                        GameObject newCube = Instantiate(cubePrefab, position, Quaternion.identity);
                        newCube.GetComponent<Cube>().SetPartOfSolution(false);
                        cubesLeftToBreak++;
                    }
                }
            }
        }
    }

    private void PrintMatrixLayer(int layer)
    {
        string str = "[";
        for (int i = 0; i < puzzleDimensions.y; i++)
        {
            for (int j = 0; j < puzzleDimensions.x; j++)
            {
                str += puzzleSolutionMatrix[layer][i][j];
                if (j < puzzleDimensions.x - 1)
                {
                    str += ", ";
                }
                else
                {
                    if (i < puzzleDimensions.y - 1)
                    {
                        str += "]\n[";
                    }
                    else
                    {
                        str += "]";
                    }
                }
            }
        }
        Debug.Log(str);
    }

    private void MistakeMade(GameObject cube)
    {
        Debug.Log("Penalidade! :(");
        cube.GetComponent<MeshRenderer>().material = brokenMaterial;
    }

    private void DecreaseCubesLeft()
    {
        cubesLeftToBreak--;
        if (cubesLeftToBreak == 0)
        {
            Debug.Log("Venceu!! :)");
        }
    }

    public Vector3 GetPuzzleOrigin()
    {
        return puzzleOrigin;
    }

    public void PaintCube(GameObject cube)
    {
        cube.GetComponent<MeshRenderer>().material = brokenMaterial;
    }

    public void UnpaintCube(GameObject cube)
    {
        cube.GetComponent<MeshRenderer>().material = defaultMaterial;
    }
}
