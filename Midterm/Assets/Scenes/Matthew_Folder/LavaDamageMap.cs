using UnityEngine;
using UnityEngine.Tilemaps;

public class LavaDamageMap : MonoBehaviour
{
    public Tilemap lavaTilemap; // Reference to the Tilemap containing lava tiles
    public int damageAmount = 1; // Amount of damage to apply
    private GameObject player;

    private void start(){
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void update()
    {
        if (player != null)
        {
            if (IsPlayerOnTile(player))
            {
                DamagePlayer(player);
            }
        }
    }

    private bool IsPlayerOnTile(GameObject player)
    {
        Vector3 playerPosition = player.transform.position;
        Vector3Int cellPosition = lavaTilemap.WorldToCell(playerPosition);
        return lavaTilemap.HasTile(cellPosition);
    }
    private void DamagePlayer(GameObject player)
    {
        // Assuming the player has a DamageIFrame component to manage health
        DamageIFrame damageIFrame = player.GetComponent<DamageIFrame>();
        if (damageIFrame != null)
        {
            damageIFrame.damageLogic();
        }
    }
}