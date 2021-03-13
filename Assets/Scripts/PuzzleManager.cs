using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    // Material do bloco destacado
    [SerializeField] private Material highlightMaterial = null;
    // Material de um bloco quebrado erroneamente
    [SerializeField] private Material brokenMaterial = null;
    // Material default
    [SerializeField] private Material defaultMaterial = null;

    // Prefab do bloco
    [SerializeField] private GameObject blockPrefab = null;

    private Puzzle3D puzzle; // Puzzle a ser resolvido
    private Vector3 puzzleOrigin; // Ponto central do puzzle;
    private int blocksLeftToBreak; // A quantidade de blocos restantes a serem quebrados

    // Evento acionado sempre que a origem (centro) do puzzle se alterar
    public delegate void OnPuzzleOriginChanged(Vector3 newOrigin);
    public static event OnPuzzleOriginChanged PuzzleOriginChanged;

    public void BuildPuzzleFromModel(bool highlightCorrectBlocks = false)
    {
        float minHeight, minWidth, minDepth, maxHeight, maxWidth, maxDepth;

        // Recupera todos os blocos do puzzle
        List<Transform> blocks = new List<Transform>(transform.childCount);

        // Atualiza as altura, larguras e profundidades mínimas e máximas
        // e calcula as dimensões do puzzle
        PuzzleDimensions puzzleDimensions = CalculatePuzzleDimensions(blocks, out minHeight, out minWidth, out minDepth,
                                                            out maxHeight, out maxWidth, out maxDepth);

        // Calcula o centro do puzzle e invoca o evento
        puzzleOrigin = CalculatePuzzleOrigin(minHeight, minWidth, minDepth, maxHeight, maxWidth, maxDepth);
        PuzzleOriginChanged?.Invoke(puzzleOrigin);

        // Aloca a matriz de solução do puzzle
        puzzle = new Puzzle3D(puzzleDimensions);

        // Preenche a matriz de solução do puzzle, criando os blocos extras e 
        // atualizando a quantidade de blocos a serem quebrados
        FillOutSolutionMatrix(blocks, maxHeight, minWidth, maxDepth, highlightCorrectBlocks);

        PrintMatrixLayer(0);
    }

    public void BuildModelFromPuzzle(Puzzle3D puzzle, bool highlightCorrectBlocks = false)
    {
        this.puzzle = puzzle;

        // Deleta todos os blocos pre-existentes
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Cria todos os blocos necessários e destaca os que fazem parte da solução
        CreateBlocks(highlightCorrectBlocks);

        // Calcula o centro do puzzle e invoca o evento
        puzzleOrigin = CalculatePuzzleOrigin(-puzzle.Dimensions.lines + 1, 0, -puzzle.Dimensions.layers + 1, 0, puzzle.Dimensions.columns - 1, 0);
        PuzzleOriginChanged?.Invoke(puzzleOrigin);
    }

    private void OnEnable()
    {
        Block.CorrectBlockBroken += DecreaseBlocksLeft;
        Block.WrongBlockBroken += MistakeMade;
        Block.BlockPainted += PaintBlock;
        Block.BlockUnpainted += UnpaintBlock;
    }

    private void OnDisable()
    {
        Block.CorrectBlockBroken -= DecreaseBlocksLeft;
        Block.WrongBlockBroken -= MistakeMade;
        Block.BlockPainted -= PaintBlock;
        Block.BlockUnpainted -= UnpaintBlock;
    }

    // Calcula as dimensões do puzzle
    private PuzzleDimensions CalculatePuzzleDimensions(List<Transform> blocks, out float minHeight, out float minWidth, out float minDepth,
                                                                out float maxHeight, out float maxWidth, out float maxDepth)
    {
        minHeight = minWidth = minDepth = maxHeight = maxWidth = maxDepth = 0;

        for (int i = 0; i < blocks.Capacity; i++)
        {
            blocks.Add(transform.GetChild(i));

            if (blocks[i].position.y > maxHeight) maxHeight = blocks[i].position.y;
            if (blocks[i].position.y < minHeight) minHeight = blocks[i].position.y;
            if (blocks[i].position.x > maxWidth) maxWidth = blocks[i].position.x;
            if (blocks[i].position.x < minWidth) minWidth = blocks[i].position.x;
            if (blocks[i].position.z > maxDepth) maxDepth = blocks[i].position.z;
            if (blocks[i].position.z < minDepth) minDepth = blocks[i].position.z;
        }

        return new PuzzleDimensions((int)(maxDepth - minDepth + 1), (int)(maxHeight - minHeight + 1), (int)(maxWidth - minWidth + 1));
    }

    private Vector3 CalculatePuzzleOrigin(float minHeight, float minWidth, float minDepth,
                                            float maxHeight, float maxWidth, float maxDepth)
    {
        return new Vector3((maxWidth + minWidth) / 2, (maxHeight + minHeight) / 2, (maxDepth + minDepth) / 2);
    }

    // Atribui cada um dos blocos pré-existentes a uma posição da matriz tridimensional e os torna parte da solução
    private void FillOutSolutionMatrix(List<Transform> blocks, float maxHeight, float minWidth, float maxDepth, bool highlightCorrectBlocks)
    {
        blocksLeftToBreak = 0;

        // Ordena a lista de blocos decrescentemente por coordenada z
        // blocos com mesmo z são ordenados decrescentemente por coordenada z
        // blocos com mesmo y são ordenados crescentemente por coordenada x
        blocks.Sort(delegate (Transform t1, Transform t2)
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
        float previousY = blocks[0].position.y, previousZ = blocks[0].position.z;

        // Associa cada bloco da solução a uma posição da matriz de solução
        // e marca o bloco como parte da solução
        for (int c = 0; c < blocks.Count; c++) // C++ rsrsrs
        {
            //// Quando o z se alterar, muda de linha e atualiza o z anterior
            //if (blocks[c].position.y != previousY)
            //{
            //    k += (int)(previousZ - blocks[c].position.z);
            //    previousZ = blocks[c].position.z;
            //}
            //// Quando o y se alterar, muda de linha e atualiza o y anterior
            //if (blocks[c].position.y != previousY)
            //{
            //    i += (int)(previousY - blocks[c].position.y);
            //    previousY = blocks[c].position.y;
            //}

            // Calcula a posição e adiciona o bloco à solução
            i = (int)(maxHeight - blocks[c].position.y);
            int j = (int)(blocks[c].position.x - minWidth);
            k = (int)(maxDepth - blocks[c].position.z);
            // Debug.Log(blocks[c].gameObject.name + " - i: " + i + ", j: " + j + ", k: " + k);
            puzzle.SolutionMatrix[k][i][j] = 1;
            blocks[c].GetComponent<Block>().SetPartOfSolution(true);

            if (highlightCorrectBlocks)
            {
                blocks[c].GetComponent<MeshRenderer>().material = highlightMaterial;
            }
        }

        // Cria um novo bloco para cada posição da matriz de solução
        // não pertencente a solução final e ajusta sua posição no puzzle
        for (k = 0; k < puzzle.Dimensions.layers; k++)
        {
            for (i = 0; i < puzzle.Dimensions.lines; i++)
            {
                for (int j = 0; j < puzzle.Dimensions.columns; j++)
                {
                    if (puzzle.SolutionMatrix[k][i][j] == 0)
                    {
                        Vector3 position = new Vector3(j + minWidth, maxHeight - i, maxDepth - k);
                        GameObject newBlock = Instantiate(blockPrefab, position, Quaternion.identity);
                        newBlock.GetComponent<Block>().SetPartOfSolution(false);
                        blocksLeftToBreak++;
                    }
                }
            }
        }
    }

    private void CreateBlocks(bool highlightCorrectBlocks)
    {
        blocksLeftToBreak = 0;

        // Instancia um bloco para cada posição da matriz de solução
        for (int k = 0; k < puzzle.Dimensions.layers; k++)
        {
            for (int i = 0; i < puzzle.Dimensions.lines; i++)
            {
                for (int j = 0; j < puzzle.Dimensions.columns; j++)
                {
                    Vector3 position = new Vector3(j, -i, -k);
                    GameObject newBlock = Instantiate(blockPrefab, position, Quaternion.identity);

                    // Adiciona o bloco como parte da solução, se ele for
                    if (puzzle.SolutionMatrix[k][i][j] == 1)
                    {
                        newBlock.GetComponent<Block>().SetPartOfSolution(true);
                        if (highlightCorrectBlocks)
                            newBlock.GetComponent<MeshRenderer>().material = highlightMaterial;
                    }
                    else
                    {
                        newBlock.GetComponent<Block>().SetPartOfSolution(false);
                        blocksLeftToBreak++;
                    }

                    // Torna o novo bloco filho do puzzle
                    newBlock.transform.SetParent(transform);
                }
            }
        }
    }

    private void PrintMatrixLayer(int layer)
    {
        string str = "[";
        for (int i = 0; i < puzzle.Dimensions.lines; i++)
        {
            for (int j = 0; j < puzzle.Dimensions.columns; j++)
            {
                str += puzzle.SolutionMatrix[layer][i][j];
                if (j < puzzle.Dimensions.columns - 1)
                {
                    str += ", ";
                }
                else
                {
                    if (i < puzzle.Dimensions.lines - 1)
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

    private void DecreaseBlocksLeft()
    {
        blocksLeftToBreak--;
        if (blocksLeftToBreak == 0)
        {
            Debug.Log("Venceu!! :)");
        }
    }

    public Vector3 GetPuzzleOrigin()
    {
        return puzzleOrigin;
    }

    public void PaintBlock(GameObject cube)
    {
        cube.GetComponent<MeshRenderer>().material = brokenMaterial;
    }

    public void UnpaintBlock(GameObject cube)
    {
        cube.GetComponent<MeshRenderer>().material = defaultMaterial;
    }
}
